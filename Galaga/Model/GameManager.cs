﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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
        private readonly double canvasHeight;
        private readonly double canvasWidth;
        private const int maxLevels = 4;
        private bool isPoweredUp = false;
        private int maxPlayerBullets;

        /// <summary>
        /// Checks if player can shoot
        /// </summary>
        public bool CanShoot = true;

        /// <summary>
        /// list of active bullets
        /// </summary>
        public List<Bullet> activeBullets;

        /// <summary>
        /// EnemyManager
        /// </summary>
        public EnemyManager enemyManager;

        /// <summary>
        /// Player Manager
        /// </summary>
        public PlayerManager playerManager;

        /// <summary>
        /// Collision Manager
        /// </summary>
        public CollisionManager collisionManager;

        /// <summary>
        /// Sound Manager
        /// </summary>
        public SoundManager soundManager;

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
        public EnemyManager EnemyManager => this.enemyManager;
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
            this.canvasHeight = canvas.Height;
            this.canvasWidth = canvas.Width;

            this.Level = 1;
            this.initializeGame();
            this.maxPlayerBullets = 3;
            
        }

        #endregion

        #region Private Methods

        private void initializeGame()
        {
            this.createAndPlacePlayer();
            this.collisionManager = new CollisionManager(this, this.Player, this.SecondPlayer, this.activeBullets);
            this.activeBullets = new List<Bullet>();
            this.placeEnemies();
            this.soundManager = new SoundManager();
        }
        /// <summary>
        /// Places the enemies.
        /// </summary>
        public void placeEnemies()
        {
            this.enemyManager = new EnemyManager(this.canvas, this, this.collisionManager);
            this.enemyManager.PlaceEnemies();
        }

        /// <summary>
        /// Starts the boss round.
        /// </summary>
        public void StartBossRound()
        {
            this.enemyManager.PlaceEnemiesForBossRound();
        }

        private void createAndPlacePlayer()
        {
            
            this.playerManager = new PlayerManager(this.canvas);
            this.playerManager.CreateAndPlacePlayer();
            this.Player = this.playerManager.Player;
            this.SecondPlayer = this.playerManager.SecondPlayer;
            
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Player shoot a bullet.
        /// </summary>
        /// <returns>Task waiting to see if player can shoot again</returns>
        public async Task PlayerShoot()
        {
            if (!this.CanShoot || this.activeBullets.Count >= this.maxPlayerBullets)
            {
                return;
            }

            this.soundManager.PlayPlayerFireSound();
            this.CanShoot = false;

            var movementPerStep = 20;

            if (this.isPoweredUp)
            {

                this.PlayerPowerUpBullets(this.Player, movementPerStep);
            }
            else
            {
                this.FireBulletFromPlayer(this.Player, movementPerStep);
            }

            if (this.playerManager.isDoubleShipActive && this.SecondPlayer != null)
            {
                if (this.isPoweredUp)
                {
                    this.PlayerPowerUpBullets(this.SecondPlayer, movementPerStep);
                }
                else
                {
                    this.FireBulletFromPlayer(this.SecondPlayer, movementPerStep);
                }
            }

            await Task.Delay(300);
            this.CanShoot = true;
        }

        private void FireBulletFromPlayer(Player player, int movementPerStep)
        {
            var bullet = new Bullet
            {
                IsShooting = true,
                X = player.X + movementPerStep,
                Y = player.Y,
            };

            this.canvas.Children.Add(bullet.Sprite);
            this.activeBullets.Add(bullet);
            this.collisionManager.StartPlayerBulletMovement(bullet, this.canvas);
        }

        private void PlayerPowerUpBullets(Player player, int movementPerStep)
        {
            for (int i = -1; i <= 1; i++)
            {
                var horizontalOffset = 15;
                var bullet = new Bullet
                {
                    IsShooting = true,
                    X = player.X + movementPerStep + (i * horizontalOffset),
                    Y = player.Y,
                };

                this.canvas.Children.Add(bullet.Sprite);
                this.activeBullets.Add(bullet);
                this.collisionManager.StartPlayerBulletMovement(bullet, this.canvas);
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
            if (this.Level < maxLevels)
            {
                this.Level++;
                this.initializeNextLevel(this.Level);
            } else
            {
                this.OnGameWon();
            }
        }

        private bool isLastLevel()
        {
            return this.Level >= maxLevels;
        }

        private void initializeNextLevel(int level)
        {
            this.LevelChanged?.Invoke(this, EventArgs.Empty);
            this.enemyManager.BonusEnemyActive = false;
        }

        /// <summary>
        /// Players the power up. Let's player shoot all three bullets at the same time
        /// </summary>
        public async void playerPowerUp()
        {
            if (this.isPoweredUp) return;

            this.isPoweredUp = true;
            this.maxPlayerBullets = 3 * this.maxPlayerBullets;
            
            await Task.Delay(10000);

            this.isPoweredUp = false;
            this.maxPlayerBullets = this.maxPlayerBullets / 3;
        }

        /// <summary>
        /// Activates the double player ship.
        /// </summary>
        public void ActivateDoublePlayerShip()
        {
            this.playerManager.isDoubleShipActive = true;
            this.maxPlayerBullets = 6;
            this.playerManager.createSecondShip();
        }
        #endregion

    }

       
}
        
        
    


   
