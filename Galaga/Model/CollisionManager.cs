using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System.Diagnostics;

namespace Galaga.Model
{
    /// <summary>
    /// Manages collision detection and bullet movement.
    /// </summary>
    public class CollisionManager
    {
        #region Data Members

        private readonly GameManager gameManager;
        private readonly Player player;
        private readonly List<Bullet> activeBullets;
        private readonly Canvas canvas;
        private Random random;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is collision processed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is collision processed; otherwise, <c>false</c>.
        /// </value>
        public bool IsCollisionProcessed { get; set; }

        public DispatcherTimer EnemyTimer { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionManager"/> class.
        /// </summary>
        /// <param name="gameManager">The game manager.</param>
        /// <param name="player">The player.</param>
        /// <param name="activeBullets">The list of active bullets.</param>
        public CollisionManager(GameManager gameManager, Player player, List<Bullet> activeBullets)
        {
            this.gameManager = gameManager;
            this.player = player;
            this.activeBullets = activeBullets ?? new List<Bullet>();
            this.canvas = gameManager.Canvas;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the bullet movement.
        /// </summary>
        /// <param name="bullet">The bullet.</param>
        /// <param name="canvasParam">The canvasParam.</param>
        public void StartPlayerBulletMovement(Bullet bullet, Canvas canvasParam)
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(5)
            };

            timer.Tick += (s, e) =>
            {
                var position = bullet.Y;
                var canvasBarrier = 0;

                if (position > canvasBarrier)
                {
                    var movementPerStep = 10;
                    bullet.Y -= movementPerStep;
                    bullet.UpdateBoundingBox();
                    this.checkCollisionWithEnemy(bullet, canvasParam);
                    this.updatePlayerBullet(bullet, timer, canvasParam);
                }
                else
                {
                    this.removePlayerBullet(bullet, timer, canvasParam);
                }
            };

            timer.Start();
        }

        private void checkCollisionWithEnemy(Bullet bullet, Canvas canvasParam)
        {
            bullet.UpdateBoundingBox();

            foreach (var enemy in this.gameManager.enemyManager.Enemies)
            {
                if (enemy != null)
                {
                    enemy.UpdateBoundingBox();

                    if (this.isCollision(bullet.BoundingBox, enemy.BoundingBox))
                    {
                        this.checkIfEnemyIsAttackingEnemy(enemy);
                        canvasParam.Children.Remove(bullet.Sprite);
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

        private void checkIfEnemyIsAttackingEnemy(Enemy enemy)
        {
            if (enemy is AttackEnemy enemyLevelThree)
            {
                enemyLevelThree.Timer.Stop();
            }
        }

        private void removeEnemyAndUpdateScore(Enemy enemy)
        {
            this.gameManager.WasCollision = true;
            this.canvas.Children.Remove(enemy.Sprite);
            this.gameManager.enemyManager.Enemies.Remove(enemy);

            var amount = enemy.ScoreValue;
            this.gameManager.Player.Score += amount;

            this.gameManager.OnEnemyKilled();

            if (this.gameManager.enemyManager.Enemies.Count == 0)
            {
                this.gameManager.OnGameWon();
            }
        }

        private void updatePlayerBullet(Bullet bullet, DispatcherTimer dispatcherTimer, Canvas canvasParam)
        {
            if (this.gameManager.WasCollision)
            {
                this.gameManager.WasCollision = false;
                canvasParam.Children.Remove(bullet.Sprite);
                dispatcherTimer.Stop();
                this.gameManager.activeBullets.Remove(bullet);
                this.activeBullets.Remove(bullet);
            }
        }

        private void removePlayerBullet(Bullet bullet, DispatcherTimer dispatcherTimer, Canvas canvasParam)
        {
            canvasParam.Children.Remove(bullet.Sprite);
            this.gameManager.Player.BulletsShot--;
            dispatcherTimer.Stop();
            this.gameManager.activeBullets.Remove(bullet);
            this.activeBullets.Remove(bullet);
        }





        

        /// <summary>
        /// Starts the enemy bullet movement.
        /// </summary>
        /// <param name="bullet">The bullet.</param>
        /// <param name="canvasParam">The canvasParam.</param>
        public void StartEnemyBulletMovement(Bullet bullet, Canvas canvasParam)
        {
            
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(5)
            };

            timer.Tick += (s, e) =>
            {
                var position = bullet.Y;
                var canvasHeight = canvasParam.ActualHeight;

                if (position < canvasHeight)
                {
                    var movementPerStep = 10;
                    bullet.Y += movementPerStep;
                    bullet.UpdateBoundingBox();
                    this.checkCollisionWithPlayer(bullet, canvasParam, timer);
                }
                else
                {
                    canvasParam.Children.Remove(bullet.Sprite);
                    timer.Stop();
                }
            };

            timer.Start();
        }

        private void checkCollisionWithPlayer(Bullet enemyBullet, Canvas canvasParam, DispatcherTimer timer)
        {
            enemyBullet.UpdateBoundingBox();
            this.player.UpdateBoundingBox();

            if (this.isCollision(enemyBullet.BoundingBox, this.player.BoundingBox))
            {
                this.updateGameState(enemyBullet, canvasParam, timer);
                this.checkPlayerStatus();
            }
        }

        private void updateGameState(Bullet enemyBullet, Canvas canvasParam, DispatcherTimer timer)
        {
            this.IsCollisionProcessed = true;
            this.gameManager.Player.Lives--;
            this.gameManager.OnPlayerHit();

            canvasParam.Children.Remove(enemyBullet.Sprite);
            timer.Stop();
            
        }


        private void checkPlayerStatus()
        {
            if (this.gameManager.Player.Lives == 0)
            {
                this.gameManager.OnGameOver();
            }
            else
            {
                this.canvas.Children.Remove(this.player.Sprite);
                this.player.StartInvincibility(3);

                var playerReturnTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                this.playerReturnTimer(playerReturnTimer);
            }
        }

        private void playerReturnTimer(DispatcherTimer playerReturnTimer)
        {
            playerReturnTimer.Tick += (s, e) =>
            {
                this.canvas.Children.Add(this.player.Sprite);
                playerReturnTimer.Stop();
            };
            playerReturnTimer.Start();
        }

        #endregion
    }
}

