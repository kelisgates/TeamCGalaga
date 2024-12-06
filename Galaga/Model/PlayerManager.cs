using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    /// player manager
    /// </summary>
    public class PlayerManager
    {
        #region Data Members

        private const double PlayerOffsetFromBottom = 30;
        private Canvas canvas;

        /// <summary>
        /// The player
        /// </summary>
        public Player Player;

        /// <summary>
        /// The second player
        /// </summary>
        public Player SecondPlayer;

        /// <summary>
        /// The is double ship active
        /// </summary>
        public bool IsDoubleShipActive;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerManager"/> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public PlayerManager(Canvas canvas)
        {
            this.Player = new Player();
            this.SecondPlayer = new Player();
            this.canvas = canvas;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Moves the Player left.
        /// </summary>
        public void MovePlayerLeft()
        {
            var leftCanvasBarrier = 0;
            if (this.Player.X - this.Player.SpeedX >= leftCanvasBarrier)
            {
                this.Player.MoveLeft();
                if (this.IsDoubleShipActive)
                {
                    this.SecondPlayer.X -= this.Player.SpeedX;
                }
            }
        }

        /// <summary>
        /// Moves the Player right.
        /// </summary>
        public void MovePlayerRight()
        {
            var canvasWidth = this.canvas.Width;
            var rightBorder = this.Player.X + this.Player.SpeedX + this.Player.Width <= canvasWidth;
            if (this.IsDoubleShipActive)
            {
                rightBorder = this.Player.X + this.Player.SpeedX + 2 * this.Player.Width <= canvasWidth;
            }
            
            if (rightBorder)
            {
                this.Player.MoveRight();
                if (this.IsDoubleShipActive)
                {
                    this.SecondPlayer.X += this.Player.SpeedX;
                }
            }

        }

        /// <summary>
        /// Creates and place player.
        /// </summary>
        public void CreateAndPlacePlayer()
        {
            this.canvas.Children.Add(this.Player.Sprite);

            this.placePlayerNearBottomOfBackgroundCentered();
            if (this.IsDoubleShipActive)
            {
                this.createSecondShip();
            }
        }

        private void placePlayerNearBottomOfBackgroundCentered()
        {
            var half = 2;
            var canvasWidth = this.canvas.Width;
            var canvasHeight = this.canvas.Height;
            this.Player.X = canvasWidth / half - this.Player.Width / half;
            this.Player.Y = canvasHeight - this.Player.Height - PlayerOffsetFromBottom;
        }

        /// <summary>
        /// Creates the second ship.
        /// </summary>
        public void createSecondShip()
        {
            if (!this.IsDoubleShipActive)
            {
                return;
            }

            this.SecondPlayer.X = this.Player.X + this.Player.Width;
            this.SecondPlayer.Y = this.Player.Y;
            
            if (!this.canvas.Children.Contains(this.SecondPlayer.Sprite))
            {
                this.canvas.Children.Add(this.SecondPlayer.Sprite);
            } 
            this.SecondPlayer.Sprite.Visibility = Windows.UI.Xaml.Visibility.Visible;
           

        }

        /// <summary>
        /// Removes the player.
        /// </summary>
        /// <param name="player">The player.</param>
        public void removePlayer(Player player)
        {
            if (this.IsDoubleShipActive)
            {
                this.canvas.Children.Remove(player.Sprite);
                this.IsDoubleShipActive = false;
                this.SecondPlayer.Sprite.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

        }

        #endregion

    }
}
