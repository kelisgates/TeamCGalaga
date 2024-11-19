using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///  class for attack enemy 
    /// </summary>
    public class AttackEnemy : Enemy
    {
        #region Data members

        private Random random;
        private readonly Canvas canvas;
        private readonly CollisionManager collisionManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AttackEnemy"/> class.
        /// </summary>
        public AttackEnemy(ICollection<BaseSprite> sprites, int score, Canvas canvas, CollisionManager collisionManager) : base(sprites, score)
        {
            this.canvas = canvas;
            this.collisionManager = collisionManager;
            MoveEnemy();
            this.shootAtPlayer();
        }

        #endregion

        #region private methods

        private void shootAtPlayer()
        {
            try
            {
                this.random = new Random();
                this.restartShootingTimer();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in shootAtPlayer: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void shootingTimer_Tick(object sender, object e)
        {
            try
            {
                this.shoot();
                Timer.Interval = TimeSpan.FromMilliseconds(this.random.Next(1000, 10000));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in shootingTimer_Tick: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void shoot()
        {
            try
            {
                var enemyBullet = new Bullet
                {
                    X = X,
                    Y = Y
                };

                this.canvas.Children.Add(enemyBullet.Sprite);
                this.collisionManager.StartEnemyBulletMovement(enemyBullet, this.canvas);
                //this.startMovement(enemyBullet);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in shoot: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }
        }

        //private void startMovement(Bullet bulletParam)
        //{
        //    try
        //    {
        //        var seconds = 50;

        //        var timer = new DispatcherTimer
        //        {
        //            Interval = TimeSpan.FromMilliseconds(seconds)
        //        };

        //        timer.Tick += (s, e) =>
        //        {
        //            try
        //            {
        //                var position = bulletParam.Y;
        //                var canvasHeight = this.canvas.ActualHeight;
        //                var canvasBarrier = Math.Min(600, canvasHeight);

        //                Debug.WriteLine($"Bullet Position: {position}, Canvas Barrier: {canvasBarrier}");

        //                if (position < canvasBarrier)
        //                {
        //                    var movementPerStep = 10;
        //                    bulletParam.Y += movementPerStep;
        //                    this.checkCollision(bulletParam);
        //                }
        //                else
        //                {
        //                    this.canvas.Children.Remove(bulletParam.Sprite);
        //                    timer.Stop();
        //                    this.IsShooting = false;
        //                    Debug.WriteLine("Bullet removed from canvas.");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Debug.WriteLine($"Exception in timer.Tick: {ex.Message}");
        //                Debug.WriteLine(ex.StackTrace);
        //            }
        //        };

        //        timer.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Exception in startMovement: {ex.Message}");
        //        Debug.WriteLine(ex.StackTrace);
        //    }
        //}

        //private void checkCollision(Bullet enemyBullet)
        //{
        //    try
        //    {
        //        if (this.isCollisionProcessed)
        //        {
        //            return;
        //        }

        //        if (enemyBullet.IntersectsWith(this.player))
        //        {
        //            this.updateGameState(enemyBullet);
        //            this.checkPlayerStatus();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Exception in checkCollision: {ex.Message}");
        //        Debug.WriteLine(ex.StackTrace);
        //    }
        //}

        //private void updateGameState(Bullet enemyBullet)
        //{
        //    this.isCollisionProcessed = true;

        //    this.gameManager.Player.Lives--;
        //    this.gameManager.OnPlayerHit(); 

        //    this.canvas.Children.Remove(enemyBullet.Sprite);

        //    this.Timer.Stop();
        //    this.IsShooting = false;
        //}
        ///// <summary>
        ///// move to player
        ///// </summary>
        //private void checkPlayerStatus()
        //{
        //    try
        //    {
        //        if (this.gameManager.Player.Lives == 0)
        //        {
        //            this.Timer.Stop();
        //            this.gameManager.OnGameOver();
        //        }
        //        else
        //        {
        //            this.canvas.Children.Remove(this.player.Sprite);
        //            this.player.StartInvincibility(2);

        //            var playerReturnTimer = new DispatcherTimer
        //            {
        //                Interval = TimeSpan.FromSeconds(1)
        //            };
        //            this.playerReturnTimer(playerReturnTimer);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Exception in checkPlayerStatus: {ex.Message}");
        //        Debug.WriteLine(ex.StackTrace);
        //    }
        //}
        ///// <summary>
        ///// move to player
        ///// </summary>
        ///// <param name="playerReturnTimer"></param>
        //private void playerReturnTimer(DispatcherTimer playerReturnTimer)
        //{

        //    playerReturnTimer.Tick += (s, e) =>
        //    {
        //        try
        //        {
        //            this.canvas.Children.Add(this.player.Sprite);
        //            playerReturnTimer.Stop();
        //            this.isCollisionProcessed = false;
        //            this.restartShootingTimer();
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Exception in playerReturnTimer: {ex.Message}");
        //            Debug.WriteLine(ex.StackTrace);
        //        }
        //    };
        //    playerReturnTimer.Start();
        //}

        private void restartShootingTimer()
        {
            try
            {
                Timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(this.random.Next(1000, 10000))
                };
                Timer.Tick += this.shootingTimer_Tick;
                Timer.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in restartShootingTimer: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }
        }

        #endregion
    }
    

}

