using Galaga.View;
using System;
using System.Collections.Generic;
using System.Drawing;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Galaga.View.Sprites;

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

        /// <summary>
        /// Gets the score manager.
        /// </summary>
        /// <value>
        /// The score manager.
        /// </value>
        public ScoreManager ScoreManager { get; set; }


        private List<GameObject> enemies;


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

        #region Methods

        

        private void initializeGame()
        {
            this.ScoreManager = new ScoreManager();
            this.createAndPlacePlayer();
            this.enemies = new List<GameObject>();
            this.createAndPlaceEnemies();

        }

        

        private void createAndPlaceEnemies()
        {
            

            
            for (int i = 0; i < 4; i++)
            {
                
                var enemy = new Enemy
                {
                    X = 50 + i * 100,
                    Y = 5
                };
                this.canvas.Children.Add(enemy.Sprite);
                this.enemies.Add(enemy);
            }

            
            for (int i = 0; i < 3; i++)
            {
                var enemy = new EnemyLevelTwo
                {
                    X = 50 + i * 100,
                    Y = 100
                };
                this.canvas.Children.Add(enemy.Sprite);
                this.enemies.Add(enemy);
            }

            
            for (int i = 0; i < 2; i++)
            {
                var enemy = new EnemyLevelThree
                {
                    X = 50 + i * 100,
                    Y = 300
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
                    this.checkCollision(bullet);
                }
                else
                {
                    this.canvas.Children.Remove(bullet.Sprite);
                    timer.Stop();
                }
            };
            timer.Start();
        }

        private void checkCollision(BulletManager bullet)
        {
            var bulletRect = bullet.GetRectangle();

            foreach (var enemy in this.enemies)
            {
                if (enemy is GameObject enemySprite)
                {
                    var enemyRect = enemySprite.GetRectangle();
                    if (bulletRect.IntersectsWith(enemyRect))
                    {
                        this.canvas.Children.Remove(bullet.Sprite);
                        this.canvas.Children.Remove(enemySprite.Sprite);
                        this.enemies.Remove(enemy);

                        this.ScoreManager.Score += 10;

                        break;
                    }
                }
            }
        }

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
            this.manager = new BulletManager
            {
                IsShooting = true,
                X = this.player.X + 20,
                Y = this.player.Y,

            };

            this.canvas.Children.Add(this.manager.Sprite);
            this.startBulletMovement(this.manager);

            
        }
    }
        
        
    


    #endregion

}

