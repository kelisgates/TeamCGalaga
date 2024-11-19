using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    /// player manager
    /// </summary>
    public class PlayerManager
    {
        private const double PlayerOffsetFromBottom = 30;
        public readonly Player player;
        private Canvas canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerManager"/> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public PlayerManager(Canvas canvas)
        {
            this.player = new Player();
            this.canvas = canvas;
        }

        /// <summary>
        /// Moves the Player left.
        /// </summary>
        public void MovePlayerLeft()
        {
            var leftCanvasBarrier = 0;
            if (this.player.X - this.player.SpeedX >= leftCanvasBarrier)
            {
                this.player.MoveLeft();
            }
        }

        /// <summary>
        /// Moves the Player right.
        /// </summary>
        public void MovePlayerRight()
        {
            var canvasWidth = this.canvas.Width;
            if (this.player.X + this.player.SpeedX + this.player.Width <= canvasWidth)
            {
                this.player.MoveRight();
            }

        }

        /// <summary>
        /// Creates and place player.
        /// </summary>
        public void CreateAndPlacePlayer()
        {
            
            this.canvas.Children.Add(this.player.Sprite);

            this.placePlayerNearBottomOfBackgroundCentered();
        }

        private void placePlayerNearBottomOfBackgroundCentered()
        {
            var half = 2;
            var canvasWidth = this.canvas.Width;
            var canvasHeight = this.canvas.Height;
            this.player.X = canvasWidth / half - this.player.Width / half;
            this.player.Y = canvasHeight - this.player.Height - PlayerOffsetFromBottom;
        }

    }
}
