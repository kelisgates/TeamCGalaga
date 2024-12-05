using Galaga.View.Sprites;
using System.Runtime.CompilerServices;
using Windows.UI.Composition;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    /// player manager
    /// </summary>
    public class PlayerManager
    {
        private const double PlayerOffsetFromBottom = 30;
        public Player Player;
        public Player SecondPlayer;
        private Canvas canvas;
        public bool isDoubleShipActive;

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

        /// <summary>
        /// Moves the Player left.
        /// </summary>
        public void MovePlayerLeft()
        {
            var leftCanvasBarrier = 0;
            if (this.Player.X - this.Player.SpeedX >= leftCanvasBarrier)
            {
                this.Player.MoveLeft();
                if (isDoubleShipActive)
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
            if (isDoubleShipActive)
            {
                rightBorder = this.Player.X + this.Player.SpeedX + 2 * this.Player.Width <= canvasWidth;
            }
            
            if (rightBorder)
            {
                this.Player.MoveRight();
                if (isDoubleShipActive)
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
            if (isDoubleShipActive)
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

        public void createSecondShip()
        {
            if (!isDoubleShipActive) return;
            
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
            if (isDoubleShipActive)
            {
                this.canvas.Children.Remove(player.Sprite);
                this.isDoubleShipActive = false;
                this.SecondPlayer.Sprite.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

        }
    }
}
