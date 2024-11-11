using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System.Diagnostics;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///  class for enemy level three
    /// </summary>
    public class AttackEnemy : NonAttackEnemy
    {
        #region Data members

        private const int SpeedXDirection = 3;
        private const int SpeedYDirection = 0;
        
        private Random random;
        private readonly Canvas canvas;

        
        private readonly Player player;
        private readonly GameManager gameManager;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is shooting.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is shooting; otherwise, <c>false</c>.
        /// </value>
        public bool IsShooting { get; set; }

        /// <summary>
        /// timer for shooting at Player
        /// </summary>
        public DispatcherTimer Timer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AttackEnemy"/> class.
        /// </summary>
        public AttackEnemy(GameManager gameManager,List<BaseSprite> sprites, int score, Canvas canvas, Player player) : base(sprites, score)
        {
            
            ScoreValue = score;
            this.canvas = canvas;
            this.player = player;
            SetSpeed(SpeedXDirection, SpeedYDirection);
            this.shootAtPlayer();
            this.gameManager = gameManager;
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
                this.Timer.Interval = TimeSpan.FromMilliseconds(this.random.Next(1000, 10000));
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
                if (this.IsShooting)
                {
                    return;
                }
                var enemyBullet = new Bullet
                {
                    X = X,
                    Y = Y
                };

                this.canvas.Children.Add(enemyBullet.Sprite);
                this.IsShooting = true;
                this.startMovement(enemyBullet);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in shoot: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void startMovement(Bullet bulletParam)
        {
            try
            {
                var seconds = 50;

                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(seconds)
                };

                timer.Tick += (s, e) =>
                {
                    try
                    {
                        var position = bulletParam.Y;
                        var canvasHeight = this.canvas.ActualHeight;
                        var canvasBarrier = Math.Min(600, canvasHeight);

                        Debug.WriteLine($"Bullet Position: {position}, Canvas Barrier: {canvasBarrier}");

                        if (position < canvasBarrier)
                        {
                            var movementPerStep = 10;
                            bulletParam.Y += movementPerStep;
                            bulletParam.UpdateBoundingBox();
                            this.checkCollision(bulletParam);
                        }
                        else
                        {
                            this.canvas.Children.Remove(bulletParam.Sprite);
                            timer.Stop();
                            this.IsShooting = false;
                            Debug.WriteLine("Bullet removed from canvas.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Exception in timer.Tick: {ex.Message}");
                        Debug.WriteLine(ex.StackTrace);
                    }
                };

                timer.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in startMovement: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void checkCollision(Bullet enemyBullet)
        {
            
            try
            {
                enemyBullet.UpdateBoundingBox();
                this.player.UpdateBoundingBox();

                Debug.WriteLine($"Bullet BoundingBox: {enemyBullet.BoundingBox}");
                Debug.WriteLine($"Player BoundingBox: {this.player.BoundingBox}");

                if (this.isCollision(enemyBullet.BoundingBox, this.player.BoundingBox))
                {
                    this.gameManager.OnPlayerHit(); //TODO: fix how many lives player is losing
                    this.canvas.Children.Remove(enemyBullet.Sprite);
                    this.Timer.Stop();

                    this.IsShooting = false;
                    this.checkPlayerStatus();
                    
                    
                    

                    Debug.WriteLine("Collision detected between bullet and player.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in checkCollision: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void checkPlayerStatus()
        {
            try
            {
                if (this.gameManager.Player.Lives == 0)
                {
                    this.Timer.Stop();
                    this.gameManager.OnGameOver();
                    Debug.WriteLine("Game Over: Player has no lives left.");
                }
                else
                {
                    this.canvas.Children.Remove(this.player.Sprite);
                    this.player.StartInvincibility(2);

                    var playerReturnTimer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(1)
                    };
                    this.playerReturnTimer(playerReturnTimer);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in checkPlayerStatus: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void playerReturnTimer(DispatcherTimer playerReturnTimer)
        {

            playerReturnTimer.Tick += (s, e) =>
            {
                try
                {
                    this.canvas.Children.Add(this.player.Sprite);
                    playerReturnTimer.Stop();
                    this.restartShootingTimer();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception in playerReturnTimer: {ex.Message}");
                    Debug.WriteLine(ex.StackTrace);
                }
            };
            playerReturnTimer.Start();
        }

        private bool isCollision(BoundingBox boundingBox1, BoundingBox boundingBox2)
        {
            return !(boundingBox1.Left > boundingBox2.Left + boundingBox2.Width ||
                     boundingBox1.Left + boundingBox1.Width < boundingBox2.Left ||
                     boundingBox1.Top > boundingBox2.Top + boundingBox2.Height ||
                     boundingBox1.Top + boundingBox1.Height < boundingBox2.Top);
        }

        private void restartShootingTimer()
        {
            try
            {
                this.Timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(this.random.Next(1000, 10000))
                };
                this.Timer.Tick += this.shootingTimer_Tick;
                this.Timer.Start();
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

