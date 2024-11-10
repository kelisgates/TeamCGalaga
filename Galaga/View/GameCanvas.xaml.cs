using System;
using Galaga.Model;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CompositionTarget = Windows.UI.Xaml.Media.CompositionTarget;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Galaga.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameCanvas 
    {
        private readonly GameManager gameManager;
        public readonly AttackEnemy AttackEnemy;



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
            
            Window.Current.CoreWindow.KeyDown += this.coreWindowOnKeyDown;
            Window.Current.CoreWindow.KeyUp += this.coreWindowOnKeyUp;
            CompositionTarget.Rendering += this.gameMovementLoop;
            
            this.gameManager = new GameManager(this.canvas);
            this.AttackEnemy = new AttackEnemy();
            this.gameManager.EnemyKilled += this.onEnemyKilled;
            this.gameManager.PlayerHit += this.onPlayerHit;
            this.gameManager.GameOver += this.onGameOver;
            this.gameManager.GameWon += this.onGameWon;
        }

        private void onPlayerHit(object sender, EventArgs e)
        {
            this.gameManager.Player.Lives--;
            if (this.gameManager.Player.Lives <= 0)
            {
                this.livesTextBlock.Text = $"Lives: {this.gameManager.Player.Lives}";
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


        private void gameMovementLoop(object sender, object e)
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
                this.gameManager.PlayerShoot();
            }
        }
    }
}
