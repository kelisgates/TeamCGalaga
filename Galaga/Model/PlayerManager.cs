using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    /// player manager
    /// </summary>
    public class PlayerManager
    {
        private const double PlayerOffsetFromBottom = 30;
        public readonly Player Player;
        private Canvas canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerManager"/> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public PlayerManager(Canvas canvas)
        {
            this.Player = new Player();
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
            }
        }

        /// <summary>
        /// Moves the Player right.
        /// </summary>
        public void MovePlayerRight()
        {
            var canvasWidth = this.canvas.Width;
            if (this.Player.X + this.Player.SpeedX + this.Player.Width <= canvasWidth)
            {
                this.Player.MoveRight();
            }

        }

        /// <summary>
        /// Creates and place player.
        /// </summary>
        public void CreateAndPlacePlayer()
        {
            
            this.canvas.Children.Add(this.Player.Sprite);

            this.placePlayerNearBottomOfBackgroundCentered();
        }

        private void placePlayerNearBottomOfBackgroundCentered()
        {
            var half = 2;
            var canvasWidth = this.canvas.Width;
            var canvasHeight = this.canvas.Height;
            this.Player.X = canvasWidth / half - this.Player.Width / half;
            this.Player.Y = canvasHeight - this.Player.Height - PlayerOffsetFromBottom;
        }

    }
}
