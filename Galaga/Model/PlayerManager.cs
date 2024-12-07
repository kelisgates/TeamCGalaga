using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly Canvas canvas;
        private readonly GameManager manager;
        /// <summary>
        /// Checks if the player can shoot
        /// </summary>
        public bool PlayerCanShoot = true;

        /// <summary>
        /// The maximum player bullets
        /// </summary>
        public int MaxPlayerBullets;

        /// <summary>
        /// list of active bullets
        /// </summary>
        public List<Bullet> ActiveBullets;

        /// <summary>
        /// Temporary power is active for player
        /// </summary>
        public bool PlayerPoweredUp;

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
        /// <param name="gameManager"></param>
        public PlayerManager(Canvas canvas, GameManager gameManager)
        {
            this.Player = new Player();
            this.manager = gameManager;
            this.SecondPlayer = new Player();
            this.canvas = canvas;
            this.ActiveBullets = new List<Bullet>();
            this.MaxPlayerBullets = 3;
            this.CreateAndPlacePlayer();
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
                this.CreateSecondShip();
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
        /// Player shoot a bullet.
        /// </summary>
        /// <returns>Task waiting to see if player can shoot again</returns>
        public async Task PlayerShoot()
        {
            if (!this.PlayerCanShoot || this.ActiveBullets.Count >= this.MaxPlayerBullets)
            {
                return;
            }

            this.manager.SoundManager.PlayPlayerFireSound();
            this.PlayerCanShoot = false;

            var movementPerStep = 20;

            this.properMaximumBullets(this.Player, this.PlayerPoweredUp, movementPerStep);

            if (this.IsDoubleShipActive && this.SecondPlayer != null)
            {
                this.properMaximumBullets(this.SecondPlayer, this.PlayerPoweredUp, movementPerStep);
            }

            await Task.Delay(300);
            this.PlayerCanShoot = true;
        }

        /// <summary>
        /// Creates the second ship.
        /// </summary>
        public void CreateSecondShip()
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
        public void RemovePlayer(Player player)
        {
            if (this.IsDoubleShipActive)
            {
                this.canvas.Children.Remove(player.Sprite);
                this.IsDoubleShipActive = false;
                this.SecondPlayer.Sprite.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

        }

        private void properMaximumBullets(Player player, bool isPoweredUp, int movementPerStep)
        {
            if (isPoweredUp && this.ActiveBullets.Count <= (this.MaxPlayerBullets - 3))
            {
                this.playerPowerUpBullets(player, movementPerStep);
            }
            else if (!isPoweredUp)
            {
                this.fireBulletFromPlayer(player, movementPerStep);
            }
        }

        private void fireBulletFromPlayer(Player player, int movementPerStep)
        {
            var bullet = new Bullet
            {
                IsShooting = true,
                X = player.X + movementPerStep,
                Y = player.Y,
            };

            this.canvas.Children.Add(bullet.Sprite);
            this.ActiveBullets.Add(bullet);
            this.manager.CollisionManager.StartPlayerBulletMovement(bullet, this.canvas);
        }

        private void playerPowerUpBullets(Player player, int movementPerStep)
        {
            for (var i = -1; i <= 1; i++)
            {
                const int horizontalOffset = 15;
                var bullet = new Bullet
                {
                    IsShooting = true,
                    X = player.X + movementPerStep + (i * horizontalOffset),
                    Y = player.Y,
                };


                this.canvas.Children.Add(bullet.Sprite);
                this.ActiveBullets.Add(bullet);
                this.manager.CollisionManager.StartPlayerBulletMovement(bullet, this.canvas, i);
            }
        }


        #endregion

    }
}
