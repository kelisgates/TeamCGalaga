using Galaga.View.Sprites;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;

namespace Galaga.Model
{
    /// <summary>
    /// manages enemies in the game
    /// </summary>
    public class EnemyManager
    {
        #region Data Members

        private readonly Canvas canvas;
        private DispatcherTimer animationTimer;
        private readonly CollisionManager collisionManager;

        /// <summary>
        /// list of enemies in game
        /// </summary>
        public readonly IList<Enemy> Enemies;

        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <value>
        /// The manager.
        /// </value>
        public GameManager Manager { get; }

        /// <summary>
        /// The bonus enemy active
        /// </summary>
        public bool BonusEnemyActive;

        /// <summary>
        /// The bonus ship timer
        /// </summary>
        public DispatcherTimer BonusShipTimer;

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
            this.BonusEnemyActive = false;
            this.initializeBonusShipTimer();

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

            this.placeEnemy(EnemyType.Level1, 10, canvasMiddle, 300, 3, false, false);
            this.placeEnemy(EnemyType.Level2, 20, canvasMiddle, 200, 4, false, false);
            this.placeEnemy(EnemyType.Level3, 30, canvasMiddle, 100, 4, true, false);
            this.placeEnemy(EnemyType.Level4, 40, canvasMiddle, 10, 5, true, true);

            this.addEnemiesToCanvas();
        }

        /// <summary>
        /// Places the enemies for boss round.
        /// </summary>
        public void PlaceEnemiesForBossRound()
        {
            var canvasMiddle = this.canvas.Width / 2.0;
            var bossSprites = ShipFactory.CreateEnemyShip(EnemyType.Boss);

            this.checkIfAttackOrNonAttackEnemy(50, 150, true, bossSprites, canvasMiddle, false, true);

            var numOfEnemies = 7;
            var distanceFromBoss = 100;

            for (int i = 0; i < numOfEnemies; i++)
            {
                var xPosition = canvasMiddle + (i * distanceFromBoss) - (numOfEnemies - 1) * distanceFromBoss / 2.0;
                this.checkIfAttackOrNonAttackEnemy(40, 150 + distanceFromBoss, true, ShipFactory.CreateEnemyShip(EnemyType.Level4), xPosition, false, true);
            }

            this.addEnemiesToCanvas();
            this.startSquareMovement(this.Enemies[0].Sprite, 100);
        }

        #endregion

        #region Private Methods

        private void placeEnemy(EnemyType level, int score, double canvasMiddle, double y, int numOfEnemies,
            bool isAttackEnemy, bool canTrackPlayer)
        {
            for (int i = 0; i < numOfEnemies; i++)
            {
                var widthDistance = 100;
                var sprites = ShipFactory.CreateEnemyShip(level);

                var xPosition = this.getStartPoint(numOfEnemies, canvasMiddle) + (i * widthDistance);

                this.checkIfAttackOrNonAttackEnemy(score, y, isAttackEnemy, sprites, xPosition, false, canTrackPlayer);
            }
        }

        private void checkIfAttackOrNonAttackEnemy(int score, double y, bool isAttackEnemy, ICollection<BaseSprite> sprites,
            double xPosition, bool isBonusShip, bool canTrackPlayer)
        {
            if (isAttackEnemy)
            {
                AttackEnemy attackEnemy = new AttackEnemy(sprites, score, this.canvas, this.collisionManager, isBonusShip, canTrackPlayer)
                {
                    X = xPosition,
                    Y = y
                };
                this.Enemies.Add(attackEnemy);
            }
            else
            {
                var nonAttackEnemy = new NonAttackEnemy(sprites, score, this)
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
            this.checkIfAttackOrNonAttackEnemy(50, y, true, bonusEnemyShip, x, true, true);

            foreach (var currSprite in bonusEnemyShip)
            {
                Canvas.SetLeft(currSprite, x);
                Canvas.SetTop(currSprite, y);
                this.canvas.Children.Add(currSprite);
            }

            this.Manager.SoundManager.PlayBonusEnenySound();
            
        }

        private void startSquareMovement(BaseSprite boss, double speed)
        {
            var squareWidth = 800;
            var squareHeight = 400;
            var stepSize = 5;

            var movementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000 / speed)
            };

            movementTimer.Tick += (_, _) =>
            {
                for (int i = 1; i < this.Enemies.Count; i++)
                {
                    var enemy = this.Enemies[i];
                    if (enemy.Sprite != boss)
                    {
                        this.moveEnemy(enemy, stepSize, squareWidth, squareHeight);
                    }
                }
            };

            movementTimer.Start();
        }

        private void moveEnemy(Enemy enemy, int stepSize, int squareWidth, int squareHeight)
        {
            switch (enemy.CurrDirection)
            {
                case MovementDirection.Right:
                    enemy.X += stepSize;
                    if (enemy.X >= squareWidth)
                    {
                        enemy.CurrDirection = MovementDirection.Up;
                    }
                    break;
                case MovementDirection.Up:
                    enemy.Y -= stepSize;
                    if (enemy.Y <= 0)
                    {
                        enemy.CurrDirection = MovementDirection.Left;
                    }
                    break;
                case MovementDirection.Left:
                    enemy.X -= stepSize;
                    if (enemy.X <= 0)
                    {
                        enemy.CurrDirection = MovementDirection.Down;
                    }
                    break;
                case MovementDirection.Down:
                    enemy.Y += stepSize;
                    if (enemy.Y >= squareHeight)
                    {
                        enemy.CurrDirection = MovementDirection.Right;
                    }
                    break;
            }

            enemy.checkWhichSpriteIsVisible();
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
                    if (this.canvas.Children.Contains(currSprite))
                    {
                        this.canvas.Children.Remove(currSprite);
                    }
                    this.canvas.Children.Add(currSprite);
                }

            }
        }

        /// <summary>
        /// Initializes bonushipTimer
        /// </summary>
        private void  initializeBonusShipTimer()
        {
            this.BonusShipTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(4)
            };
            var random = new Random();

            this.BonusShipTimer.Tick += (_, _) =>
            {
                if (!this.BonusEnemyActive && random.NextDouble() < 0.3)
                {
                    this.bonusEnemyShip();
                    this.BonusEnemyActive = true;
                }
            };

            this.BonusShipTimer.Start();
        }

        #endregion
    }

}
