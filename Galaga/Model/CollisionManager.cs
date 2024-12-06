using System;
using System.Collections.Generic;
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
        /// <param name="secondPlayer"> The second player</param>
        /// <param name="activeBullets">The list of active bullets.</param>
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
            timer.Tick += (_, _) =>
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
            } else if (this.GameManager.Level == 1)
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

            var amount = enemy.ScoreValue;
            this.GameManager.Player.Score += amount;
            
            this.GameManager.OnEnemyKilled();

            if (this.GameManager.ManagerEnemy.Enemies.Count == 0)
            {
                this.GameManager.OnGameWon();
            }
        }

        private void updatePlayerBullet(Bullet bullet, DispatcherTimer dispatcherTimer, Canvas canvasParam)
        {
            if (this.GameManager.WasCollision)
            {
                this.GameManager.WasCollision = false;
                canvasParam.Children.Remove(bullet.Sprite);
                dispatcherTimer.Stop();
                this.GameManager.ActiveBullets.Remove(bullet);
                this.activeBullets.Remove(bullet);

            }
        }

        private void removePlayerBullet(Bullet bullet, DispatcherTimer dispatcherTimer, Canvas canvasParam)
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
        /// <param name="canvasParam">The canvasParam.</param>
        /// <param name="canTrackPlayer">tracks if enemy can track player or not</param>
        public void StartEnemyBulletMovement(Bullet bullet, Canvas canvasParam, bool canTrackPlayer)
        {
            this.GameManager.SoundManager.PlayEnemyFireSound();
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(5)
            };
            this.Timers.Add(timer);

            double velocityX = 0;
            double velocityY = 0;

            if (canTrackPlayer)
            {
                double deltaX = this.player.X - bullet.X;
                double deltaY = this.player.Y - bullet.Y;
                double angle = Math.Atan2(deltaY, deltaX);
                double speed = 5; 
                velocityX = Math.Cos(angle) * speed;
                velocityY = Math.Sin(angle) * speed;
            }

            timer.Tick += (_, _) =>
            {
                var position = bullet.Y;
                var canvasHeight = canvasParam.ActualHeight;

                if (position < canvasHeight)
                {
                    if (canTrackPlayer)
                    {
                        bullet.X += velocityX;
                        bullet.Y += velocityY;
                    }
                    else
                    {
                        var movementPerStep = 10;
                        bullet.Y += movementPerStep;
                        
                    }
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
            if (this.GameManager.PlayerManager.IsDoubleShipActive)
            {
                
                this.OneOfTwoPlayerDeath(enemyBullet, canvasParam);

            }
            
            if (enemyBullet.Intersects(this.player))
            {
                this.player.PlayExplosionAnimation(this.player.X, this.player.Y, canvasParam);
                this.updateGameState(enemyBullet, canvasParam, timer);
                this.checkPlayerStatus();
            }


        }
        private void OneOfTwoPlayerDeath(Bullet enemyBullet, Canvas canvasParam)
        {   
            if (enemyBullet.Intersects(this.player))
            {
                this.GameManager.SoundManager.PlayPlayerDeathSound();
                this.player.PlayExplosionAnimation(this.player.X, this.player.Y, canvasParam);
                this.GameManager.PlayerManager.removePlayer(this.player);
            }
            else if (enemyBullet.Intersects(this.secondPlayer))
            {
                this.GameManager.SoundManager.PlayPlayerDeathSound();
                this.player.PlayExplosionAnimation(this.secondPlayer.X, this.secondPlayer.Y, canvasParam);
                this.GameManager.PlayerManager.removePlayer(this.secondPlayer);
            }
            
        }

        private void updateGameState(Bullet enemyBullet, Canvas canvasParam, DispatcherTimer timer)
        {
            this.IsCollisionProcessed = true;
            this.GameManager.Player.Lives--;
            this.GameManager.OnPlayerHit();

            canvasParam.Children.Remove(enemyBullet.Sprite);
            timer.Stop();
            
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
            playerReturnTimer.Tick += (_, _) =>
            {
                if (this.player.Sprite.Parent != null)
                {
                    ((Panel)this.player.Sprite.Parent).Children.Remove(this.player.Sprite);
                }
                this.canvas.Children.Add(this.player.Sprite);
                this.GameManager.CanShoot = true;
                playerReturnTimer.Stop();
                this.IsCollisionProcessed = false;
            };
            playerReturnTimer.Start();
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

        #endregion
    }
}

