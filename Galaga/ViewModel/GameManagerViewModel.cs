using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Galaga.Model;
using Windows.UI.Xaml.Controls;
using Galaga.View;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;

namespace Galaga.ViewModel
{
    /// <summary>
    /// Game manager view model.
    /// </summary>
    public class GameManagerViewModel : INotifyPropertyChanged
    {
        #region Data Members and Properties

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
            HighScoreDialog highscores = new HighScoreDialog();
            _ = await highscores.ShowAsync();
            await this.DisplayStartScreen();
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
        }

        private void CoreWindowOnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Left:
                    this.gameManager.Player.IsMovingLeft = true;
                    break;
                case VirtualKey.Right:
                    this.gameManager.Player.IsMovingRight = true;
                    break;
                case VirtualKey.Space:
                    this.gameManager.Player.IsShooting = true;
                    break;
            }
        }

        private void CoreWindowOnKeyUp(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Left:
                    this.gameManager.Player.IsMovingLeft = false;
                    break;
                case VirtualKey.Right:
                    this.gameManager.Player.IsMovingRight = false;
                    break;
                case VirtualKey.Space:
                    this.gameManager.Player.IsShooting = false;
                    break;
            }
        }

        private async Task gameMovementLoop()
        {
            if (this.gameManager.Player.IsMovingLeft)
            {
                this.gameManager.playerManager.MovePlayerLeft();
            }

            if (this.gameManager.Player.IsMovingRight)
            {
                this.gameManager.playerManager.MovePlayerRight();
            }

            if (this.gameManager.Player.IsShooting)
            {
                await this.gameManager.PlayerShoot();
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

        #endregion

        #region Events

        private void onPlayerHit(object sender, EventArgs e)
        {
            if (this.gameManager.Player.Lives == 0)
            {
                this.LivesTextBlock = $"Lives: {this.gameManager.Player.Lives}";
                this.GameOverVisibility = Visibility.Visible;
            }
            else
            {
                this.LivesTextBlock = $"Lives: {this.gameManager.Player.Lives}";
            }
        }

        private async void onGameWon(object sender, EventArgs e)
        {
            this.gameManager.collisionManager.StopAllTimers();
            this.GameWonVisibility = Visibility.Visible;
            var dialog = new ContentDialog()
            {
                Title = "You Won!",
                Content = "Congratulations, you have killed all the enemies!",
                CloseButtonText = "Exit"
            };

            dialog.CloseButtonClick += (_, _) => { Application.Current.Exit(); };

            await dialog.ShowAsync();
        }

        private async void onGameOver(object sender, EventArgs e)
        {
            this.gameManager.collisionManager.StopAllTimers();
            this.GameOverVisibility = Visibility.Visible;
            var dialog = new ContentDialog()
            {
                Title = "Game Over!",
                Content = "You have been killed by the enemy.",
                CloseButtonText = "Exit"
            };
            dialog.CloseButtonClick += (_, _) => { Application.Current.Exit(); };
            await dialog.ShowAsync();
        }

        private void onEnemyKilled(object sender, EventArgs e)
        {
            this.ScoreTextBlock = $"Score: {this.gameManager.Player.Score}";
        }

        #endregion

    }
}
