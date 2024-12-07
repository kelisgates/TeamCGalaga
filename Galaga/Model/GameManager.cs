using System;
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

        private readonly Canvas canvas;
        private const int MaxLevels = 4;

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
        /// Gets or sets a value indicating whether [was collision].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [was collision]; otherwise, <c>false</c>.
        /// </value>
        /// <returns>bool value if collision occurred</returns>
        public bool WasCollision { get; set; }

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
            
        }

        #endregion

        #region Private and Public Helper Methods

        private void initializeGame()
        {
            this.PlayerManager = new PlayerManager(this.canvas, this);
            this.CollisionManager = new CollisionManager(this, this.PlayerManager.Player, this.PlayerManager.SecondPlayer, this.PlayerManager.ActiveBullets);
            this.PlaceEnemies();
            this.SoundManager = new SoundManager();
        }

        /// <summary>
        /// Places the enemies.
        /// </summary>
        public void PlaceEnemies()
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

        #endregion

        #region Public Methods

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
            this.PlayerManager.PlayerCanShoot = false;
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
                this.PlayerManager.PlayerCanShoot = false;
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

        private void initializeNextLevel()
        {
            foreach(var bullet in this.PlayerManager.ActiveBullets)
            {
                this.canvas.Children.Remove(bullet.Sprite);
            }
            this.PlayerManager.ActiveBullets.Clear();
            this.LevelChanged?.Invoke(this, EventArgs.Empty);
            this.ManagerEnemy.BonusEnemyActive = false;
        }

        /// <summary>
        /// Players the power up. Let's player shoot all three bullets at the same time
        /// </summary>
        public async void PlayerPowerUp()
        {
            if (this.PlayerManager.PlayerPoweredUp)
            {
                return;
            }

            this.PlayerManager.PlayerPoweredUp = true;
            this.PlayerManager.MaxPlayerBullets *= 3;

            await Task.Delay(5000);

            this.PlayerManager.PlayerPoweredUp = false;
            this.PlayerManager.MaxPlayerBullets /= 3;
        }

        /// <summary>
        /// Activates the double player ship.
        /// </summary>
        public void ActivateDoublePlayerShip()
        {
            this.PlayerManager.IsDoubleShipActive = true;
            this.PlayerManager.MaxPlayerBullets *= 2;
            this.PlayerManager.CreateSecondShip();
        }
        #endregion

    }

       
}
        
        
    


   
