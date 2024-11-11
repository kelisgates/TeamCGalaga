using System;
using System.Threading.Tasks;
using Galaga.Model;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using CompositionTarget = Windows.UI.Xaml.Media.CompositionTarget;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Galaga.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameCanvas 
    {
        #region Data Members

        private readonly GameManager gameManager;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GameCanvas"/> class.
        /// </summary>
        public GameCanvas()
        {
            this.InitializeComponent();
            this.setWindowTitle("Galaga by Gates A3");
            
            Width = this.canvas.Width;
            Height= this.canvas.Height;
            ApplicationView.PreferredLaunchViewSize = new Size { Width = Width, Height = Height };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(Width, Height));

            this.gameManager = new GameManager(this.canvas);

            this.initializeGameComponents();
        }

        #endregion
        
        #region Events

        private void onPlayerHit(object sender, EventArgs e)
        {
            this.gameManager.Player.Lives--;
            if (this.gameManager.Player.Lives <= 0)
            {
                this.livesTextBlock.Text = $"Lives: {this.gameManager.Player.Lives}";
                this.canvas.Children.Clear();
                this.canvas.Children.Add(this.gameOverTextBlock);
                this.canvas.Children.Add(this.scoreTextBlock);
                this.gameOverTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                this.livesTextBlock.Text = $"Lives: {this.gameManager.Player.Lives}";
            }
        }

        private void onGameWon(object sender, EventArgs e)
        {
            this.gameWonTextBlock.Visibility = Visibility.Visible;
        }

        private void onGameOver(object sender, EventArgs e)
        {
            this.gameOverTextBlock.Visibility = Visibility.Visible;
        }

        private void onEnemyKilled(object sender, EventArgs e)
        {
            this.scoreTextBlock.Text = $"Score: {this.gameManager.Player.Score}";
        }

        #endregion

        #region Methods

        private void initializeGameComponents()
        {
            Window.Current.CoreWindow.KeyDown += this.coreWindowOnKeyDown;
            Window.Current.CoreWindow.KeyUp += this.coreWindowOnKeyUp;
            CompositionTarget.Rendering += async (s, e) => await this.gameMovementLoop();


            this.gameManager.EnemyKilled += this.onEnemyKilled;
            this.gameManager.PlayerHit += this.onPlayerHit;
            this.gameManager.GameOver += this.onGameOver;
            this.gameManager.GameWon += this.onGameWon;
        }

        private void setWindowTitle(string title)
        {
            var appView = ApplicationView.GetForCurrentView();
            appView.Title = String.Empty;
            appView.Title = title;
        }

        private void coreWindowOnKeyDown(CoreWindow sender, KeyEventArgs args)
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

        private void coreWindowOnKeyUp(CoreWindow sender, KeyEventArgs args)
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
                this.gameManager.MovePlayerLeft();
            }

            if (this.gameManager.Player.IsMovingRight)
            {
                this.gameManager.MovePlayerRight();
            }

            if (this.gameManager.Player.IsShooting)
            {
                await this.gameManager.PlayerShoot();
            }
        }

        #endregion
    }
}
