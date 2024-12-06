using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Galaga.Model
{
    /// <summary>
    /// class that manages the collisions in the game
    /// </summary>
    public class CollisionManager
    {
        #region Data Members

        /// <summary>
        /// The game manager
        /// </summary>
        public GameManager GameManager;

        private readonly Player player;
        private readonly Player secondPlayer;
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
        /// <param name="secondPlayer">The second player.</param>
        /// <param name="activeBullets">The active bullets.</param>
        public CollisionManager(GameManager gameManager, Player player, Player secondPlayer, List<Bullet> activeBullets)
        {
            this.GameManager = gameManager;
            this.player = player;
            this.secondPlayer = secondPlayer;
            this.activeBullets = activeBullets ?? new List<Bullet>();
            this.canvas = gameManager.Canvas;
            this.Timers = new List<DispatcherTimer>();
        }

        #endregion

        #region Player Methods

        /// <summary>
        /// Starts the player bullet movement.
        /// </summary>
        /// <param name="bullet">The bullet.</param>
        /// <param name="canvasParam">The canvas parameter.</param>
        /// <param name="direction">The direction.</param>
        public void StartPlayerBulletMovement(Bullet bullet, Canvas canvasParam, int direction = 0)
        {
            var timer = this.createTimer(timer =>
            {
                this.movePlayerBullet(bullet, canvasParam, direction, timer);
            });
            this.Timers.Add(timer);
            timer.Start();
        }

        private void movePlayerBullet(Bullet bullet, Canvas canvasParam, int direction, DispatcherTimer timer)
        {
            var canvasBarrier = 0;
            var canvasLeftBarrier = 0;
            var canvasRightBarrier = 900;
            if (bullet.Y > canvasBarrier && bullet.X > canvasLeftBarrier && bullet.X < canvasRightBarrier)
            {
                var movementPerStep = 10;
                bullet.Y -= movementPerStep;
                var angleStep = 2;
                bullet.X += direction < 0 ? -angleStep : direction > 0 ? angleStep : 0;
                this.checkCollisionWithEnemy(bullet, canvasParam);
                this.updateBullet(bullet, timer, canvasParam);
            }
            else
            {
                this.removeBullet(bullet, timer, canvasParam);
            }
        }

        private void checkCollisionWithEnemy(Bullet bullet, Canvas canvasParam)
        {
            foreach (var enemy in this.GameManager.ManagerEnemy.Enemies)
            {
                if (enemy == null)
                {
                    continue;
                }

                if (bullet.Intersects(enemy))
                {
                    this.GameManager.SoundManager.PlayEnemyHitSound();
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
                if (enemyLevelThree.IsBonusShip)
                {
                    this.handleBonusEnemyException();
                }

                enemyLevelThree.Timer.Stop();
            }
        }

        private void handleBonusEnemyException()
        {
            this.GameManager.ManagerEnemy.BonusShipTimer.Stop();
            if (this.GameManager.Level != 1 && !this.GameManager.PlayerManager.IsDoubleShipActive)
            {
                this.GameManager.ActivateDoublePlayerShip();
            }
            else if (this.GameManager.Level == 1)
            {
                this.GameManager.Player.Lives++;
            }

            this.GameManager.SoundManager.StopBonusEnemySound();
            this.GameManager.playerPowerUp();
        }

        private void removeEnemyAndUpdateScore(Enemy enemy)
        {
            this.GameManager.WasCollision = true;
            this.canvas.Children.Remove(enemy.Sprite);
            this.GameManager.ManagerEnemy.Enemies.Remove(enemy);
            this.GameManager.Player.Score += enemy.ScoreValue;
            this.GameManager.OnEnemyKilled();
            if (this.GameManager.ManagerEnemy.Enemies.Count == 0)
            {
                this.GameManager.OnGameWon();
            }
        }

        private void updateBullet(Bullet bullet, DispatcherTimer dispatcherTimer, Canvas canvasParam)
        {
            if (this.GameManager.WasCollision)
            {
                this.GameManager.WasCollision = false;
                this.removeBullet(bullet, dispatcherTimer, canvasParam);
            }
        }

        private void removeBullet(Bullet bullet, DispatcherTimer dispatcherTimer, Canvas canvasParam)
        {
            canvasParam.Children.Remove(bullet.Sprite);
            this.GameManager.Player.BulletsShot--;
            dispatcherTimer.Stop();
            this.GameManager.ActiveBullets.Remove(bullet);
            this.activeBullets.Remove(bullet);
        }

        #endregion

        #region Enemy Methods

        /// <summary>
        /// Starts the enemy bullet movement.
        /// </summary>
        /// <param name="bullet">The bullet.</param>
        /// <param name="canvasParam">The canvas parameter.</param>
        /// <param name="canTrackPlayer">if set to <c>true</c> [can track player].</param>
        public void StartEnemyBulletMovement(Bullet bullet, Canvas canvasParam, bool canTrackPlayer)
        {
            this.GameManager.SoundManager.PlayEnemyFireSound();
            var timer = this.createTimer(timer => this.moveEnemyBullet(bullet, canvasParam, timer));
            this.Timers.Add(timer);

            if (canTrackPlayer)
            {
                double deltaX = this.player.X - bullet.X;
                double deltaY = this.player.Y - bullet.Y;
                double angle = Math.Atan2(deltaY, deltaX);
                double speed = 5;
                bullet.VelocityX = Math.Cos(angle) * speed;
                bullet.VelocityY = Math.Sin(angle) * speed;
            }
            else
            {
                bullet.VelocityY = 10; // Move straight down if not tracking
            }

            timer.Start();
        }

        private void moveEnemyBullet(Bullet bullet, Canvas canvasParam, DispatcherTimer timer)
        {
            var position = bullet.Y;
            var canvasHeight = canvasParam.ActualHeight;
            if (position < canvasHeight)
            {
                bullet.X += bullet.VelocityX;
                bullet.Y += bullet.VelocityY;
                this.checkCollisionWithPlayer(bullet, canvasParam, timer);
            }
            else
            {
                this.removeBullet(bullet, timer, canvasParam);
            }
        }

        private void trackPlayer(Bullet bullet)
        {
            double deltaX = this.player.X - bullet.X;
            double deltaY = this.player.Y - bullet.Y;
            double angle = Math.Atan2(deltaY, deltaX);
            double speed = 5;
            bullet.X += Math.Cos(angle) * speed;
            bullet.Y += Math.Sin(angle) * speed;
        }

        private void checkCollisionWithPlayer(Bullet enemyBullet, Canvas canvasParam, DispatcherTimer timer)
        {
            if (this.IsCollisionProcessed)
            {
                return;
            }

            if (this.GameManager.PlayerManager.IsDoubleShipActive && enemyBullet.Intersects(this.secondPlayer))
            {
                this.OneOfTwoPlayerDeath(enemyBullet, canvasParam);
            }

            if (enemyBullet.Intersects(this.player))
            {
                this.player.PlayExplosionAnimation(this.player.X, this.player.Y, canvasParam);
                this.updateGameState(enemyBullet, canvasParam, timer);
                this.checkPlayerStatus();
                if (this.GameManager.PlayerManager.IsDoubleShipActive)
                {
                    this.GameManager.PlayerManager.removePlayer(this.secondPlayer);
                    this.GameManager.MaxPlayerBullets /= 2;
                }
            }
        }

        private void OneOfTwoPlayerDeath(Bullet enemyBullet, Canvas canvasParam)
        {
            if (enemyBullet.Intersects(this.secondPlayer))
            {
                this.GameManager.SoundManager.PlayPlayerDeathSound();
                this.player.PlayExplosionAnimation(this.secondPlayer.X, this.secondPlayer.Y, canvasParam);
                this.GameManager.PlayerManager.removePlayer(this.secondPlayer);
                this.GameManager.MaxPlayerBullets /= 2;
            }
        }

        private void updateGameState(Bullet enemyBullet, Canvas canvasParam, DispatcherTimer timer)
        {
            this.IsCollisionProcessed = true;
            if (!this.GameManager.PlayerManager.IsDoubleShipActive)
            {
                this.GameManager.Player.Lives--;
            }

            this.GameManager.OnPlayerHit();
            this.removeBullet(enemyBullet, timer, canvasParam);
        }

        private void checkPlayerStatus()
        {
            if (this.GameManager.Player.Lives == 0)
            {
                this.GameManager.OnGameOver();
            }
            else
            {
                this.GameManager.CanShoot = false;
                this.canvas.Children.Remove(this.player.Sprite);
                this.player.StartInvincibility(3); var playerReturnTimer = this.createTimer(timer => this.returnPlayer(timer));
                playerReturnTimer.Interval = TimeSpan.FromSeconds(1);
                playerReturnTimer.Start();
            }
        }

        private void returnPlayer(DispatcherTimer playerReturnTimer)
        {
            if (this.player.Sprite.Parent != null)
            {
                ((Panel)this.player.Sprite.Parent).Children.Remove(this.player.Sprite);
            }

            this.canvas.Children.Add(this.player.Sprite);
            this.GameManager.CanShoot = true;
            playerReturnTimer.Stop();
            this.IsCollisionProcessed = false;
        }

        #endregion

        #region Methods

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

        private DispatcherTimer createTimer(Action<DispatcherTimer> tickAction)
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(5)
            };
            timer.Tick += (_, _) => tickAction(timer);
            return timer;
        }

        #endregion
    }
}

