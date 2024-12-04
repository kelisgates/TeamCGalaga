using Galaga.View.Sprites;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;

namespace Galaga.Model
{
    /// <summary>
    /// enemy type levels
    /// </summary>
    public enum EnemyType
    {
        /// <summary>
        /// the level1 enemy
        /// </summary>
        Level1,
        /// <summary>
        /// The level2 enemy
        /// </summary>
        Level2,
        /// <summary>
        /// The level3 enemy
        /// </summary>
        Level3,
        /// <summary>
        /// The level4 enemy
        /// </summary>
        Level4
    }

    /// <summary>
    /// manages enemies in the game
    /// </summary>
    public class EnemyManager
    {
        #region Data Members

        /// <summary>
        /// list of enemies in game
        /// </summary>
        public readonly IList<Enemy> Enemies;

        private readonly Canvas canvas;
        private DispatcherTimer animationTimer;
        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <value>
        /// The manager.
        /// </value>
        public GameManager Manager { get; }
        private readonly CollisionManager collisionManager;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyManager"/> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="manager">The game manager object</param>
        /// <param name="collisionManager"></param>
        public EnemyManager(Canvas canvas, GameManager manager, CollisionManager collisionManager)
        {
            this.Enemies = new List<Enemy>();
            this.Manager = manager;
            this.canvas = canvas;
            this.collisionManager = collisionManager;
            this.initializeAnimationTimer();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Places the enemies.
        /// </summary>
        public void PlaceEnemies()
        {
            var half = 2.0;
            var canvasWidth = this.canvas.Width;
            var canvasMiddle = canvasWidth / half;

            this.PlaceEnemy(EnemyType.Level1, 10, canvasMiddle, 250, 3, false);
            this.PlaceEnemy(EnemyType.Level2, 20, canvasMiddle, 170, 4, false);
            this.PlaceEnemy(EnemyType.Level3, 30, canvasMiddle, 100, 4, true);
            this.PlaceEnemy(EnemyType.Level4, 40, canvasMiddle, 20, 5, true);

            this.addEnemiesToCanvas();
        }

        private void addEnemiesToCanvas()
        {
            foreach (var enemy in this.Enemies)
            {
                if (enemy.Sprite.Parent != null)
                {
                    ((Panel)enemy.Sprite.Parent).Children.Remove(enemy.Sprite);
                }

                foreach (var currSprite in enemy.Sprites)
                {
                    Canvas.SetLeft(currSprite, enemy.X);
                    Canvas.SetTop(currSprite, enemy.Y);
                    this.canvas.Children.Add(currSprite);
                }

            }
        }

        /// <summary>
        /// Places the enemy.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="score">The score.</param>
        /// <param name="canvasMiddle">The canvas middle.</param>
        /// <param name="y">The y.</param>
        /// <param name="numOfEnemies">The number of enemies.</param>
        /// <param name="isAttackEnemy">if set to <c>true</c> [is attack enemy].</param>
        public void PlaceEnemy(EnemyType level, int score, double canvasMiddle, double y, int numOfEnemies,
            bool isAttackEnemy)
        {
            for (int i = 0; i < numOfEnemies; i++)
            {
                var widthDistance = 100;
                var sprites = ShipFactory.CreateEnemyShip(level);

                var xPosition = this.getStartPoint(numOfEnemies, canvasMiddle) + (i * widthDistance);

                this.checkIfAttackOrNonAttackEnemy(score, y, isAttackEnemy, sprites, xPosition, false);
            }
        }

        #endregion

        #region Private Methods

        private void checkIfAttackOrNonAttackEnemy(int score, double y, bool isAttackEnemy, ICollection<BaseSprite> sprites,
            double xPosition, bool isBonusShip)
        {
            if (isAttackEnemy)
            {
                var attackEnemy = new AttackEnemy(sprites, score, this.canvas, this.collisionManager, isBonusShip)
                {
                    X = xPosition,
                    Y = y
                };
                this.Enemies.Add(attackEnemy);
            }
            else
            {
                var nonAttackEnemy = new NonAttackEnemy(sprites, score)
                {
                    X = xPosition,
                    Y = y
                };
                this.Enemies.Add(nonAttackEnemy);
            }
        }

        private void initializeAnimationTimer()
        {
            var seconds = 20;
            this.animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(seconds)
            };
            this.animationTimer.Tick += this.OnAnimationTick;
            this.animationTimer.Start();
        }

        private void OnAnimationTick(object sender, object e)
        {
            foreach (var enemy in this.Enemies)
            {
                enemy.UpdateImage();
            }
        }

        private double getStartPoint(double numOfEnemies, double canvasMiddle)
        {
            var widthDistance = 100;
            var half = 2.0;
            return canvasMiddle - (numOfEnemies * widthDistance) / half;

        }
        /// <summary>
        /// Bonuses the enemy ship.
        /// </summary>
        private void bonusEnemyShip()
        {
            var bonusEnemyShip = ShipFactory.CreateEnemyShip(EnemyType.Level4);
            var random = new Random();
            var x = random.Next(50, 500);
            var y = random.Next(50, 200);
            this.checkIfAttackOrNonAttackEnemy(50, y, true, bonusEnemyShip, x, true);

            foreach (var currSprite in bonusEnemyShip)
            {
                Canvas.SetLeft(currSprite, x);
                Canvas.SetTop(currSprite, y);
                this.canvas.Children.Add(currSprite);
            }

        }
        
        /// <summary>
        /// Initializes bonushipTimer
        /// </summary>
        public void  initializeBonusShipTimer()
        {
            var bonusShipTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(15)
            };

            var random = new Random();

            bonusShipTimer.Tick += (_, _) =>
            {
                if (random.NextDouble() < 0.3)
                {
                    this.bonusEnemyShip();
                }
            };

            bonusShipTimer.Start();
        }
        #endregion
    }

}
