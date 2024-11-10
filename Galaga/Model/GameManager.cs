using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;

namespace Galaga.Model
{
    /// <summary>
    /// Manages the Galaga game play.
    /// </summary>
    public class GameManager
    {
        #region Data members

        private const double PlayerOffsetFromBottom = 30;
        private readonly Canvas canvas;
        private readonly double canvasHeight;
        private readonly double canvasWidth;

        
        private BulletManager manager;
        private EnemyManager enemyManager;
        private DispatcherTimer timer;

        #endregion

        #region Properties
        /// <summary>
        /// player object
        /// </summary>
        public Player Player { get; private set; }
        /// <summary>
        /// Gets the score manager.
        /// </summary>
        /// <value>
        /// The score manager.
        /// </value>
        public ScoreManager ScoreManager { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is Player bullet active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is Player bullet active; otherwise, <c>false</c>.
        /// </value>
        public bool IsPlayerBulletActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [was collision].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [was collision]; otherwise, <c>false</c>.
        /// </value>
        public bool WasCollision { get; set; }

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
            this.ScoreManager = new ScoreManager();
            this.createAndPlacePlayer();
            this.placeEnemies();

            

        }

        private void placeEnemies()
        {
            this.enemyManager = new EnemyManager(this.canvas);

            var half = 2.0;
            var canvasMiddle = this.canvasWidth / half;

            this.enemyManager.PlaceNonAttackEnemy(EnemyType.Level1, 10, canvasMiddle, 250, 3);
            this.enemyManager.PlaceNonAttackEnemy(EnemyType.Level2, 20, canvasMiddle, 170, 4);
            this.enemyManager.PlaceAttackEnemy(EnemyType.Level3, 30, canvasMiddle, 100, 4, this.Player);
            this.enemyManager.PlaceAttackEnemy(EnemyType.Level4, 40, canvasMiddle, 20, 5, this.Player);

            foreach (var enemy in this.enemyManager.Enemies)
            {
                this.canvas.Children.Add(enemy.Sprite);
            }
        }


        private void createAndPlacePlayer()
        {
            this.Player = new Player();
            this.canvas.Children.Add(this.Player.Sprite);

            this.placePlayerNearBottomOfBackgroundCentered();
        }

        private void placePlayerNearBottomOfBackgroundCentered()
        {
            var half = 2;
            this.Player.X = this.canvasWidth / half - this.Player.Width / half;
            this.Player.Y = this.canvasHeight - this.Player.Height - PlayerOffsetFromBottom;
        }


        private void startBulletMovement(BulletManager bullet)
        {
            this.timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            this.timer.Tick += (s, e) =>
            {
                var position = this.manager.Y;
                var canvasBarrier = 0;
                if (position > canvasBarrier)
                {
                    var movementPerStep = 10;
                    bullet.Y -= movementPerStep;
                    bullet.UpdateBoundingBox();
                    this.checkCollision(bullet);
                    if (this.WasCollision)
                    {
                        this.WasCollision = false;
                        this.canvas.Children.Remove(bullet.Sprite);
                        this.timer.Stop();
                        this.IsPlayerBulletActive = false;
                    }
                }
                else
                {
                    this.canvas.Children.Remove(bullet.Sprite);
                    this.timer.Stop();
                    this.IsPlayerBulletActive = false;
                }
            };
            this.timer.Start();
        }

        

        private void checkCollision(BulletManager bullet)
        {
            bullet.UpdateBoundingBox();
            
            foreach (var enemy in this.enemyManager.Enemies)
            {
                if (enemy != null)
                {
                    enemy.UpdateBoundingBox();
                    
                    if (this.isCollision(bullet.BoundingBox, enemy.BoundingBox))
                    {
                        this.checkIfEnemyIsAttackingEnemy(enemy);
                        this.removeEnemyAndUpdateScore(enemy);

                        break;
                    }
                }
            }
        }

        private bool isCollision(BoundingBox boundingBox1, BoundingBox boundingBox2)
        {
            return !(boundingBox1.Left > boundingBox2.Left + boundingBox2.Width ||
                     boundingBox1.Left + boundingBox1.Width < boundingBox2.Left ||
                     boundingBox1.Top > boundingBox2.Top + boundingBox2.Height ||
                     boundingBox1.Top + boundingBox1.Height < boundingBox2.Top);
        }

        private void displayGameWon()
        {
            var gameWonTextBlock = new TextBlock
            {
                Text = "Game Won!",
                Foreground = new SolidColorBrush(Windows.UI.Colors.Green),
                FontSize = 48,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Canvas.SetLeft(gameWonTextBlock, (this.canvasWidth - gameWonTextBlock.ActualWidth) / 2);
            Canvas.SetTop(gameWonTextBlock, (this.canvasHeight - gameWonTextBlock.ActualHeight) / 2);

            this.canvas.Children.Add(gameWonTextBlock);
        }

        private void removeEnemyAndUpdateScore(NonAttackEnemy enemy)
        {
            
            this.WasCollision = true;
            this.canvas.Children.Remove(this.manager.Sprite);
            this.canvas.Children.Remove(enemy.Sprite);
            this.enemyManager.Enemies.Remove(enemy);
            var amount = enemy.ScoreValue;
            this.ScoreManager.Score += amount;

            if (this.enemyManager.Enemies.Count == 0)
            {
                this.displayGameWon();
            }


            
        }

        private void checkIfEnemyIsAttackingEnemy(NonAttackEnemy enemy)
        {
            if (enemy is AttackEnemy enemyLevelThree)
            {
                enemyLevelThree.IsShooting = false;
                enemyLevelThree.Timer.Stop();
            }
        }
        #endregion

        #region Public Methods
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
            if (this.Player.X + this.Player.SpeedX + this.Player.Width <= this.canvasWidth)
            {
                this.Player.MoveRight();
            }
            
        }

        /// <summary>
        /// Player shoot a bullet.
        /// </summary>
        public void PlayerShoot()
        {
            if (this.IsPlayerBulletActive)
            {
                return;
            }

            var movementPerStep = 20;
            this.manager = new BulletManager
            {
                IsShooting = true,
                X = this.Player.X + movementPerStep,
                Y = this.Player.Y,

            };

            this.canvas.Children.Add(this.manager.Sprite);
            this.IsPlayerBulletActive = true;
            this.startBulletMovement(this.manager);

            
        }
    }
        
        
    


    #endregion

}

