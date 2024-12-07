using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Windows.UI.Xaml;
using Galaga.Model;
using Windows.UI.Xaml.Controls;
using Galaga.View;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using System.Linq;
using System.Windows.Input;
using Windows.Storage;
using Galaga.Command;

namespace Galaga.ViewModel
{
    /// <summary>
    /// Game manager view model.
    /// </summary>
    public class GameManagerViewModel : INotifyPropertyChanged
    {
        #region Data Members and Properties

        private const string HighScoreFileName = "HighScores.xml";  

        private GameManager gameManager;

        private Canvas gameCanvas;

        private String livesTextBlock;

        /// <summary>
        /// Gets or sets the lives text block.
        /// </summary>
        /// <value>
        /// The lives text block.
        /// </value>
        public String LivesTextBlock
        {
            get => this.livesTextBlock;
            
            set => this.SetField(ref this.livesTextBlock, value);
        }

        
        private String scoreTextBlock;

        /// <summary>
        /// Gets or sets the score text block.
        /// </summary>
        /// <value>
        /// The score text block.
        /// </value>
        public String ScoreTextBlock
        {
            get => this.scoreTextBlock;
            
            set => this.SetField(ref this.scoreTextBlock, value);
        }

        private String levelTextBlock;
        /// <summary>
        /// Gets or sets the Level text block.
        /// </summary>
        /// <value>
        /// The Level text block.
        /// </value>
        public String LevelTextBlock
        {
            get => this.levelTextBlock;
            set => this.SetField(ref this.levelTextBlock, value);
        }

        private Visibility gameOverVisibility;

        /// <summary>
        /// Gets or sets the game over visibility.
        /// </summary>
        /// <value>
        /// The game over visibility.
        /// </value>
        public Visibility GameOverVisibility
        {
            get => this.gameOverVisibility;
            
            set => this.SetField(ref this.gameOverVisibility, value);
        }

        private Visibility gameWonVisibility;

        /// <summary>
        /// Gets or sets the game won visibility.
        /// </summary>
        /// <value>
        /// The game won visibility.
        /// </value>
        public Visibility GameWonVisibility
        {
            get => this.gameWonVisibility;
            
            set => this.SetField(ref this.gameWonVisibility, value);
        }
        
        private ObservableCollection<HighScore> highScores;

        /// <summary>
        /// Gets or sets the high scores.
        /// </summary>
        /// <value>
        /// The high scores.
        /// </value>
        public ObservableCollection<HighScore> HighScores
        {
            get => this.highScores;
            
            set => this.SetField(ref this.highScores, value);
        }

        /// <summary>
        /// Gets the sort by score name Level command.
        /// </summary>
        public ICommand SortByScoreNameLevelCommand { get; }

        /// <summary>
        /// Gets the sort by name score Level command.
        /// </summary>
        public ICommand SortByNameScoreLevelCommand { get; }

        /// <summary>
        /// Gets the sort by Level score name command.
        /// </summary>
        public ICommand SortByLevelScoreNameCommand { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GameManagerViewModel"/> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public GameManagerViewModel(Canvas canvas)
        {
            this.gameCanvas = canvas;

            this.GameOverVisibility = Visibility.Collapsed;

            this.GameWonVisibility = Visibility.Collapsed;

            this.LivesTextBlock = "Lives: 3";

            this.ScoreTextBlock = "Score: 0";

            this.LevelTextBlock = "Level: 1";

            this.highScores = new ObservableCollection<HighScore>();

            _ = this.loadHighScores();

            this.SortByScoreNameLevelCommand = new RelayCommand(this.SortByScoreNameLevel);
            this.SortByNameScoreLevelCommand = new RelayCommand(this.SortByNameScoreLevel);
            this.SortByLevelScoreNameCommand = new RelayCommand(this.SortByLevelScoreName);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Displays the start screen.
        /// </summary>
        public async Task DisplayStartScreen()
        {
            var dialog = new ContentDialog()
            {
                Title = "Welcome to Galaga",
                Content = "Choose Start to play game or Highscores to see top 10 scores!",
                PrimaryButtonText = "Start",
                SecondaryButtonText = "Highscores",
                CloseButtonText = "Exit"
            };

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                this.gameManager = new GameManager(this.gameCanvas);
                this.initializeGame();
            }
            else if (result == ContentDialogResult.Secondary)
            {
                await this.displayHighScoreBoardDialog(dialog);
            }
            else if (result == ContentDialogResult.None)
            {
                Application.Current.Exit();
            }
        }

        private async Task displayHighScoreBoardDialog(ContentDialog dialog)
        {
            dialog.Hide();
            HighScoreDialog highscores = new HighScoreDialog
            {
                DataContext = this
            };
            _ = await highscores.ShowAsync();
            await this.DisplayStartScreen();
        }

        private async Task loadHighScores()
        {
            var folder = ApplicationData.Current.LocalFolder;
            try
            {
                var file = await folder.GetFileAsync(HighScoreFileName);
                using (var inStream = await file.OpenStreamForReadAsync())
                {
                    var deserializer = new DataContractSerializer(typeof(ObservableCollection<HighScore>));
                    var data = (ObservableCollection<HighScore>)deserializer.ReadObject(inStream);
                    this.HighScores = data ?? new ObservableCollection<HighScore>();
                }
            }
            catch (FileNotFoundException)
            {
                
                this.HighScores = new ObservableCollection<HighScore>();
            }

        }

        private async Task saveHighScores()
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(HighScoreFileName, CreationCollisionOption.ReplaceExisting);
            using (var outStream = await file.OpenStreamForWriteAsync())
            {
                var serializer = new DataContractSerializer(typeof(ObservableCollection<HighScore>));
                serializer.WriteObject(outStream, this.HighScores);
            }

        }

        private void initializeGame()
        {
            Window.Current.CoreWindow.KeyDown += this.CoreWindowOnKeyDown;
            Window.Current.CoreWindow.KeyUp += this.CoreWindowOnKeyUp;
            Windows.UI.Xaml.Media.CompositionTarget.Rendering += async (_, _) => await this.gameMovementLoop();

            this.gameManager.EnemyKilled += this.onEnemyKilled;
            this.gameManager.PlayerHit += this.onPlayerHit;
            this.gameManager.GameOver += this.onGameOver;
            this.gameManager.GameWon += this.onGameWon;
            this.gameManager.LevelChanged += this.onLevelChanged;
        }

        private void CoreWindowOnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Left:
                    this.gameManager.PlayerManager.Player.IsMovingLeft = true;
                    break;
                case VirtualKey.Right:
                    this.gameManager.PlayerManager.Player.IsMovingRight = true;
                    break;
                case VirtualKey.Space:
                    this.gameManager.PlayerManager.Player.IsShooting = true;
                    break;
            }
        }

        private void CoreWindowOnKeyUp(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Left:
                    this.gameManager.PlayerManager.Player.IsMovingLeft = false;
                    break;
                case VirtualKey.Right:
                    this.gameManager.PlayerManager.Player.IsMovingRight = false;
                    break;
                case VirtualKey.Space:
                    this.gameManager.PlayerManager.Player.IsShooting = false;
                    break;
            }
        }

        private async Task gameMovementLoop()
        {
            if (this.gameManager.PlayerManager.Player.IsMovingLeft)
            {
                this.gameManager.PlayerManager.MovePlayerLeft();
            }

            if (this.gameManager.PlayerManager.Player.IsMovingRight)
            {
                this.gameManager.PlayerManager.MovePlayerRight();
            }

            if (this.gameManager.PlayerManager.Player.IsShooting)
            {
                await this.gameManager.PlayerManager.PlayerShoot();
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// sorts the high scores by score, name and Level.
        /// </summary>
        public void SortByScoreNameLevel()
        {
            this.HighScores = new ObservableCollection<HighScore>(
                this.HighScores.OrderByDescending(h => h.Score)
                    .ThenBy(h => h.PlayerName)
                    .ThenByDescending(h => h.Level));
            this.updateSortedHighScores();
        }

        /// <summary>
        /// sorts the high scores by name, score and Level.
        /// </summary>
        public void SortByNameScoreLevel()
        {
            this.HighScores = new ObservableCollection<HighScore>(
                this.HighScores.OrderBy(h => h.PlayerName)
                    .ThenByDescending(h => h.Score)
                    .ThenByDescending(h => h.Level));
            this.updateSortedHighScores();
        }

        /// <summary>
        /// sorts the high scores by Level, score and name.
        /// </summary>
        public void SortByLevelScoreName()
        {
            this.HighScores = new ObservableCollection<HighScore>(
                this.HighScores.OrderByDescending(h => h.Level)
                    .ThenByDescending(h => h.Score)
                    .ThenBy(h => h.PlayerName));
            this.updateSortedHighScores();
        }

        /// <summary>
        /// Updates the sorted high scores.
        /// </summary>
        private void updateSortedHighScores()
        {
            this.OnPropertyChanged(nameof(this.HighScores));
        }

        #endregion

        #region Events

        private void onPlayerHit(object sender, EventArgs e)
        {
            this.gameManager.SoundManager.PlayPlayerDeathSound();
            if (this.gameManager.PlayerManager.Player.Lives == 0)
            {
                this.LivesTextBlock = $"Lives: {this.gameManager.PlayerManager.Player.Lives}";
                this.GameOverVisibility = Visibility.Visible;
            }
            else
            {
                this.LivesTextBlock = $"Lives: {this.gameManager.PlayerManager.Player.Lives}";
            }
        }

        private async void onGameWon(object sender, EventArgs e)
        {
            this.gameManager.SoundManager.PlayGameWonSound();
            this.gameManager.CollisionManager.StopAllTimers();
            this.GameWonVisibility = Visibility.Visible;
            await this.checkAndAddHighScore(this.gameManager.PlayerManager.Player.Score, this.gameManager.Level);
            var dialog = new ContentDialog()
            {
                Title = "You Won!",
                Content = "Congratulations, you have killed all the enemies!",
                PrimaryButtonText = "Check High scores Leaderboard",
                CloseButtonText = "Exit"
            };

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                dialog.Hide();
                HighScoreDialog highscores = new HighScoreDialog
                {
                    DataContext = this
                };
                _ = await highscores.ShowAsync();
                await dialog.ShowAsync();
            }
            else if (result == ContentDialogResult.None)
            {
                Application.Current.Exit();
            }


        }

        private async Task checkAndAddHighScore(int score, int level)
        {
            if (this.HighScores.Count < 10 || score > this.HighScores.Min(h => h.Score))
            {
                var inputTextBox = new TextBox { PlaceholderText = "Enter your name" };
                var dialog = new ContentDialog
                {
                    Title = "New High Score!",
                    Content = inputTextBox,
                    PrimaryButtonText = "OK",
                    CloseButtonText = "Cancel"
                };

                if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                {
                    string playerName = inputTextBox.Text;
                    this.HighScores.Add(new HighScore { PlayerName = playerName, Score = score, Level = level});
                    this.HighScores = new ObservableCollection<HighScore>(this.HighScores.OrderByDescending(h => h.Score).Take(10));
                    await this.saveHighScores();
                }
            }

            HighScoreDialog highscores = new HighScoreDialog
            {
                DataContext = this
            };
            _ = await highscores.ShowAsync();
        }

        private async void onGameOver(object sender, EventArgs e)
        {
            this.gameManager.SoundManager.PlayGameOverSound();
            this.gameManager.CollisionManager.StopAllTimers();
            await this.checkAndAddHighScore(this.gameManager.PlayerManager.Player.Score, this.gameManager.Level);
            var dialog = new ContentDialog()
            {
                Title = "Game Over!",
                Content = "You have been killed by the enemy.",
                PrimaryButtonText = "Check High scores Leaderboard",
                CloseButtonText = "Exit"
            };
            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                dialog.Hide();
                HighScoreDialog highscores = new HighScoreDialog
                {
                    DataContext = this
                };
                _ = await highscores.ShowAsync();
                await dialog.ShowAsync();
            }
            else if (result == ContentDialogResult.None)
            {
                Application.Current.Exit();
            }
        }

        private void onEnemyKilled(object sender, EventArgs e)
        {
            this.gameManager.SoundManager.PlayEnemyHitSound();
            this.ScoreTextBlock = $"Score: {this.gameManager.PlayerManager.Player.Score}";
            this.LivesTextBlock = $"Lives: {this.gameManager.PlayerManager.Player.Lives}";
        }

        private async void onLevelChanged(object sender, EventArgs e)
        {
            this.gameManager.ManagerEnemy.BonusShipTimer?.Stop();
            this.gameManager.ManagerEnemy.Enemies.Clear();
            var levelDialog = new ContentDialog
            {
                Title = "Level Up!",
                Content = $"You are now on Level {this.gameManager.Level}. Get ready! Next level Starts in 5 seconds.",
            };
            levelDialog.ShowAsync();
            await Task.Delay(5000);
            levelDialog.Hide();
            
            if (this.gameManager.Level < 4)
            {
                this.gameManager.PlaceEnemies();
                this.LevelTextBlock = $"Level: {this.gameManager.Level}";
            } else if (this.gameManager.Level == 4)
            {
                this.gameManager.StartBossRound();
                this.LevelTextBlock = $"Level: {this.gameManager.Level}";
            }
            this.gameManager.PlayerManager.PlayerCanShoot = true;
        }

        #endregion

    }
}
