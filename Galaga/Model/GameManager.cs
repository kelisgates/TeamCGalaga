using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    /// Manages the Galaga game play
    /// </summary>
    public class GameManager
    {
        #region Data members

        private const double PlayerOffsetFromBottom = 30;
        private readonly Canvas canvas;
        private const int MaxLevels = 4;
        private bool isPoweredUp;

        /// <summary>
        /// The maximum player bullets
        /// </summary>
        public int MaxPlayerBullets;

        /// <summary>
        /// Checks if player can shoot
        /// </summary>
        public bool CanShoot = true;

        /// <summary>
        /// list of active bullets
        /// </summary>
        public List<Bullet> ActiveBullets;

        /// <summary>
        /// EnemyManagerDataMember
        /// </summary>
        public EnemyManager ManagerEnemy;

        /// <summary>
        /// Player Manager
        /// </summary>
        public PlayerManager PlayerManager;

        /// <summary>
        /// Collision Manager
        /// </summary>
        public CollisionManager CollisionManager;

        /// <summary>
        /// Sound Manager
        /// </summary>
        public SoundManager SoundManager;

        /// <summary>
        /// Number of levels
        /// </summary>
        public int Level;
        
        #endregion

        #region Events

        /// <summary>
        /// Occurs when [enemy killed].
        /// </summary>
        public event EventHandler EnemyKilled;

        /// <summary>
        /// Occurs when [game won].
        /// </summary>
        public event EventHandler GameWon;

        /// <summary>
        /// Occurs when [game over].
        /// </summary>
        public event EventHandler GameOver;

        /// <summary>
        /// Occurs when [player hit].
        /// </summary>
        public event EventHandler PlayerHit;

        /// <summary>
        /// Occurs when [Level changed].
        /// </summary>
        public event EventHandler LevelChanged;
        #endregion

        #region Properties

        /// <summary>
        /// player object
        /// </summary>
        /// <returns>Player object</returns>
        public Player Player { get; private set; }
        /// <summary>
        /// player object
        /// </summary>
        /// <returns>Player object</returns>
        public Player SecondPlayer { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is Player bullet active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is Player bullet active; otherwise, <c>false</c>.
        /// </value>
        /// <returns>bool value if player bullet is active</returns>
        public bool IsPlayerBulletActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [was collision].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [was collision]; otherwise, <c>false</c>.
        /// </value>
        /// <returns>bool value if collision occurred</returns>
        public bool WasCollision { get; set; }
        /// <summary>
        /// Gets the enemy manager.
        /// </summary>
        /// <value>
        /// The enemy manager.
        /// </value>
        public EnemyManager EnemyManagerDataMember => this.ManagerEnemy;
        /// <summary>
        /// Canvas
        /// </summary>
        public Canvas Canvas => this.canvas;


        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameManager"/> class.
        /// </summary>
        public GameManager(Canvas canvas)
        {
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));

            this.canvas = canvas;

            this.Level = 1;
            this.initializeGame();
            this.MaxPlayerBullets = 3;
            
        }

        #endregion

        #region Private and Public Helper Methods

        private void initializeGame()
        {
            this.createAndPlacePlayer();
            this.CollisionManager = new CollisionManager(this, this.Player, this.SecondPlayer, this.ActiveBullets);
            this.ActiveBullets = new List<Bullet>();
            this.placeEnemies();
            this.SoundManager = new SoundManager();
        }

        /// <summary>
        /// Places the enemies.
        /// </summary>
        public void placeEnemies()
        {
            this.ManagerEnemy = new EnemyManager(this.canvas, this, this.CollisionManager);
            this.ManagerEnemy.PlaceEnemies();
        }

        /// <summary>
        /// Starts the boss round.
        /// </summary>
        public void StartBossRound()
        {
            this.ManagerEnemy.PlaceEnemiesForBossRound();
        }

        private void createAndPlacePlayer()
        {
            
            this.PlayerManager = new PlayerManager(this.canvas);
            this.PlayerManager.CreateAndPlacePlayer();
            this.Player = this.PlayerManager.Player;
            this.SecondPlayer = this.PlayerManager.SecondPlayer;
            
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Player shoot a bullet.
        /// </summary>
        /// <returns>Task waiting to see if player can shoot again</returns>
        public async Task PlayerShoot()
        {
            if (!this.CanShoot || this.ActiveBullets.Count >= this.MaxPlayerBullets)
            {
                return;
            }

            this.SoundManager.PlayPlayerFireSound();
            this.CanShoot = false;

            var movementPerStep = 20;

            if (this.isPoweredUp && this.ActiveBullets.Count <= (this.MaxPlayerBullets - 3))
            {

                this.playerPowerUpBullets(this.Player, movementPerStep);
            }
            else if (!this.isPoweredUp) 
            {
                this.fireBulletFromPlayer(this.Player, movementPerStep);
            }

            if (this.PlayerManager.IsDoubleShipActive && this.SecondPlayer != null)
            {
                if (this.isPoweredUp)
                {
                    this.playerPowerUpBullets(this.SecondPlayer, movementPerStep);
                }
                else
                {
                    this.fireBulletFromPlayer(this.SecondPlayer, movementPerStep);
                }
            }

            await Task.Delay(300);
            this.CanShoot = true;
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
            this.CollisionManager.StartPlayerBulletMovement(bullet, this.canvas);
        }

        private void playerPowerUpBullets(Player player, int movementPerStep)
        {
            for (int i = -1; i <= 1; i++)
            {
                var horizontalOffset = 15;
                var verticalOffset = 10;
                var bullet = new Bullet
                {
                    IsShooting = true,
                    X = player.X + movementPerStep + (i * horizontalOffset),
                    Y = player.Y,
                };


                this.canvas.Children.Add(bullet.Sprite);
                this.ActiveBullets.Add(bullet);
                this.CollisionManager.StartPlayerBulletMovement(bullet, this.canvas, i);
            }
        }

        /// <summary>
        /// Called when [enemy killed].
        /// </summary>
        public void OnEnemyKilled()
        {
            this.EnemyKilled?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when [game won].
        /// </summary>
        public void OnGameWon()
        {
            if (this.isLastLevel())
            {
                this.GameWon?.Invoke(this, EventArgs.Empty);
            } else
            {
                this.changeLevel();
            }
        }

        /// <summary>
        /// Called when [game over].
        /// </summary>
        public void OnGameOver()
        {
            this.CanShoot = false;
            Debug.WriteLine("game manager onGame invoked");
            this.GameOver?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when [player hit].
        /// </summary>
        public void OnPlayerHit()
        {
            this.PlayerHit?.Invoke(this, EventArgs.Empty);
        }

        private void changeLevel()
        {
            if (this.Level < MaxLevels)
            {
                this.Level++;
                this.CanShoot = false;
                this.initializeNextLevel();
            } else
            {
                this.OnGameWon();
            }
        }

        private bool isLastLevel()
        {
            return this.Level >= MaxLevels;
        }

        private async void initializeNextLevel()
        {
            foreach(var bullet in this.ActiveBullets)
            {
                this.canvas.Children.Remove(bullet.Sprite);
            }
            this.ActiveBullets.Clear();
            this.LevelChanged?.Invoke(this, EventArgs.Empty);
            this.ManagerEnemy.BonusEnemyActive = false;
        }

        /// <summary>
        /// Players the power up. Let's player shoot all three bullets at the same time
        /// </summary>
        public void playerPowerUp()
        {
            if (this.isPoweredUp)
            {
                return;
            }

            this.isPoweredUp = true;
            this.MaxPlayerBullets = 3 * this.MaxPlayerBullets;

            this.isPoweredUp = false;
            this.MaxPlayerBullets /= 3;
        }

        /// <summary>
        /// Activates the double player ship.
        /// </summary>
        public void ActivateDoublePlayerShip()
        {
            this.PlayerManager.IsDoubleShipActive = true;
            this.MaxPlayerBullets *= 2;
            this.PlayerManager.createSecondShip();
        }
        #endregion

    }

       
}
        
        
    


   
