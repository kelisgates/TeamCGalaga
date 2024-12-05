using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Galaga.Model
{
    /// <summary>
    /// Manages collision detection and bullet movement.
    /// </summary>
    public class CollisionManager
    {
        #region Data Members        
        /// <summary>
        /// The game manager
        /// </summary>
        public GameManager gameManager;
        private readonly Player player;
        private readonly List<Bullet> activeBullets;
        private readonly Canvas canvas;


        /// <summary>
        /// Gets or sets a value indicating whether this instance is collision processed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is collision processed; otherwise, <c>false</c>.
        /// </value>
        public bool IsCollisionProcessed { get; set; }
        /// <summary>
        /// Gets or sets the timers.
        /// </summary>
        /// <value>
        /// The timers.
        /// </value>
        public IList<DispatcherTimer> Timers { get; set; }

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
            this.Timers = new List<DispatcherTimer>();
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
            this.Timers.Add(timer);
            timer.Tick += (s, e) =>
            {
                var position = bullet.Y;
                var canvasBarrier = 0;

                if (position > canvasBarrier)
                {
                    var movementPerStep = 10;
                    bullet.Y -= movementPerStep;
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
            foreach (var enemy in this.gameManager.enemyManager.Enemies)
            {
                if (enemy == null) continue;
                if (bullet.Intersects(enemy))
                {
                    this.gameManager.soundManager.PlayEnemyHitSound();
                    canvasParam.Children.Remove(bullet.Sprite);
                    this.checkIfEnemyIsAttackingEnemy(enemy);
                    this.removeEnemyAndUpdateScore(enemy);
                    break;
                }
                
            }
        }

        private void checkIfEnemyIsAttackingEnemy(Enemy enemy)
        {
            if (enemy is AttackEnemy enemyLevelThree)
            {
                if (enemyLevelThree.isBonusShip)
                {
                    this.handleBonusEnemyException(enemyLevelThree);
                }
                enemyLevelThree.Timer.Stop();
            }
        }

        private void handleBonusEnemyException(AttackEnemy enemyLevelThree)
        {
            this.gameManager.enemyManager.bonusShipTimer.Stop();
            this.gameManager.Player.Lives++;
            this.gameManager.soundManager.StopBonusEnemySound();
            this.gameManager.playerPowerUp();
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
            this.gameManager.soundManager.PlayEnemyFireSound();
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(5)
            };
            this.Timers.Add(timer);
            timer.Tick += (_, _) =>
            {
                var position = bullet.Y;
                var canvasHeight = canvasParam.ActualHeight;

                if (position < canvasHeight)
                {
                    var movementPerStep = 10;
                    bullet.Y += movementPerStep;
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
            if (this.IsCollisionProcessed)
            {
                return;
            }

            if (enemyBullet.Intersects(this.player))
            {
                this.player.PlayExplosionAnimation(this.player.X, this.player.Y, canvasParam);
                this.updateGameState(enemyBullet, canvasParam, timer);
                this.checkPlayerStatus(enemyBullet);
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

        private void checkPlayerStatus(Bullet enemyBullet)
        {
            if (this.gameManager.Player.Lives == 0)
            {
                this.gameManager.OnGameOver();
            }
            else
            {
                this.gameManager.canShoot = false;
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
                if (this.player.Sprite.Parent != null)
                {
                    ((Panel)this.player.Sprite.Parent).Children.Remove(this.player.Sprite);
                }
                this.canvas.Children.Add(this.player.Sprite);
                this.gameManager.canShoot = true;
                playerReturnTimer.Stop();
                this.IsCollisionProcessed = false;
            };
            playerReturnTimer.Start();
        }

        /// <summary>
        /// Stops all timers.
        /// </summary>
        public void StopAllTimers()
        {
            foreach (var timer in this.Timers)
            {
                timer.Stop();
            }
        }

        #endregion
    }
}

