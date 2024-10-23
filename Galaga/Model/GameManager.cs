using System;
using System.Collections.Generic;
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
        private Player player;
        private List<GameObject> enemies;


        #endregion

        #region Properties
        /// <summary>
        /// Gets the score manager.
        /// </summary>
        /// <value>
        /// The score manager.
        /// </value>
        public ScoreManager ScoreManager { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is player bullet active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is player bullet active; otherwise, <c>false</c>.
        /// </value>
        public bool IsPlayerBulletActive { get; set; }


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
            this.enemies = new List<GameObject>();
            this.createAndPlaceEnemies();

        }

        

        private void createAndPlaceEnemies()
        {
            var numOfLevelOne = 4;
            var numOfLevelTwo = 3;
            var numOfLevelThree = 2;
            var canvasMiddle = this.canvasWidth / 2.0;
            var startX = canvasMiddle - (numOfLevelOne * 100) / 2.0;
            var startX2 = canvasMiddle - (numOfLevelTwo * 100) / 2.0;
            var startX3 = canvasMiddle - (numOfLevelThree * 80) / 2.0;

            for (int i = 0; i < 4; i++)
            {

                var enemy = new EnemyLevelOne
                {
                    X = startX + (i * 100),
                    Y = 5
                };
                this.canvas.Children.Add(enemy.Sprite);
                this.enemies.Add(enemy);
            }


            for (int i = 0; i < 3; i++)
            {
                var enemy = new EnemyLevelTwo
                {
                    X = startX2 + (i * 100),
                    Y = 70
                };
                this.canvas.Children.Add(enemy.Sprite);
                this.enemies.Add(enemy);
            }


            for (int i = 0; i < 2; i++)
            {
                var enemy = new EnemyLevelThree(this.canvas, this.player)
                {
                    X = startX3 + (i * 100),
                    Y = 150
                };
                this.canvas.Children.Add(enemy.Sprite);
                this.enemies.Add(enemy);
            }

        }



        private void createAndPlacePlayer()
        {
            this.player = new Player();
            this.canvas.Children.Add(this.player.Sprite);

            this.placePlayerNearBottomOfBackgroundCentered();
        }

        private void placePlayerNearBottomOfBackgroundCentered()
        {
            this.player.X = this.canvasWidth / 2 - this.player.Width / 2.0;
            this.player.Y = this.canvasHeight - this.player.Height - PlayerOffsetFromBottom;
        }


        private void startBulletMovement(BulletManager bullet)
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            timer.Tick += (s, e) =>
            {
                var position = this.manager.Y;
                if (position > 0)
                {
                    bullet.Y -= 10;
                    bullet.UpdateBoundingBox();
                    this.checkCollision(bullet);
                }
                else
                {
                    this.canvas.Children.Remove(bullet.Sprite);
                    timer.Stop();
                    this.IsPlayerBulletActive = false;
                }
            };
            timer.Start();
        }

        

        private void checkCollision(BulletManager bullet)
        {
            bullet.UpdateBoundingBox();
            
            foreach (var enemy in this.enemies)
            {
                if (enemy is GameObject enemySprite)
                {
                    enemy.UpdateBoundingBox();
                    if (this.isCollision(bullet.BoundingBox, enemy.BoundingBox))
                    {
                        var enemyLevelThree = enemy as EnemyLevelThree;
                        if (enemyLevelThree != null)
                        {
                            enemyLevelThree.IsShooting = false;
                            enemyLevelThree.Timer.Stop();
                        }



                        this.canvas.Children.Remove(bullet.Sprite);
                        this.canvas.Children.Remove(enemySprite.Sprite);
                        this.enemies.Remove(enemy);
                        var amount = enemy is EnemyLevelOne ? 10 : enemy is EnemyLevelTwo ? 20 : 30;
                        this.ScoreManager.Score += amount;

                        if (this.enemies.Count == 0)
                        {
                            this.displayGameWon();
                        }

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


        #endregion

        #region Public Methods
        /// <summary>
        /// Moves the player left.
        /// </summary>
        public void MovePlayerLeft()
        {
            if (this.player.X - this.player.SpeedX >= 0)
            {
                this.player.MoveLeft();
            }
            
        }

        /// <summary>
        /// Moves the player right.
        /// </summary>
        public void MovePlayerRight()
        {
            if (this.player.X + this.player.SpeedX + this.player.Width <= this.canvasWidth)
            {
                this.player.MoveRight();
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
            this.manager = new BulletManager
            {
                IsShooting = true,
                X = this.player.X + 20,
                Y = this.player.Y,

            };

            this.canvas.Children.Add(this.manager.Sprite);
            this.IsPlayerBulletActive = true;
            this.startBulletMovement(this.manager);

            
        }
    }
        
        
    


    #endregion

}

