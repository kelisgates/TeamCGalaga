using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;

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

        public bool canShoot = true;

        public List<Bullet> activeBullets;
        public EnemyManager enemyManager;
        public PlayerManager playerManager;
        public CollisionManager collisionManager;
        public SoundManager soundManager;

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

        #endregion

        #region Properties

        /// <summary>
        /// player object
        /// </summary>
        /// <returns>Player object</returns>
        public Player Player { get; private set; }

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

        public EnemyManager EnemyManager => this.enemyManager;
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



            this.initializeGame();
        }

        #endregion

        #region Private Methods

        private void initializeGame()
        {
            this.createAndPlacePlayer();
            this.collisionManager = new CollisionManager(this, this.Player, this.activeBullets);
            this.activeBullets = new List<Bullet>();
            this.placeEnemies();
            this.soundManager = new SoundManager();
            
        }

        private void placeEnemies()
        {
            this.enemyManager = new EnemyManager(this.canvas, this, this.collisionManager);
            this.enemyManager.PlaceEnemies();
        }

        private void createAndPlacePlayer()
        {
            
            this.playerManager = new PlayerManager(this.canvas);
            this.playerManager.CreateAndPlacePlayer();
            this.Player = this.playerManager.player;
            
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Player shoot a bullet.
        /// </summary>
        /// <returns>Task waiting to see if player can shoot again</returns>
        public async Task PlayerShoot()
        {
            if (!this.canShoot || this.activeBullets.Count >= 3)
            {
                return;
            }
            this.soundManager.PlayPlayerFireSound();
            this.canShoot = false;

            var movementPerStep = 20;

            var bullet = new Bullet
            {
                IsShooting = true,
                X = this.Player.X + movementPerStep,
                Y = this.Player.Y,
            };

            this.canvas.Children.Add(bullet.Sprite);
            this.activeBullets.Add(bullet);
            this.collisionManager.StartPlayerBulletMovement(bullet, this.canvas);
            await Task.Delay(300);
            this.canShoot = true;

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
            this.GameWon?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when [game over].
        /// </summary>
        public void OnGameOver()
        {
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

        #endregion

    }

       
}
        
        
    


   
