using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Galaga.View.Sprites;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;

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

        private Bullet bullet;
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
            this.random = new Random();
            this.restartShootingTimer();

            
        }

        private void shootingTimer_Tick(object sender, object e)
        {
            this.shoot();
            this.Timer.Interval = TimeSpan.FromMilliseconds(this.random.Next(1000, 10000));
        }

        private void shoot()
        {
            if (this.IsShooting)
            {
                return;
            }
            this.bullet = new Bullet
            {
                X = X ,
                Y = Y 
            };

            this.canvas.Children.Add(this.bullet.Sprite);
            this.IsShooting = true;
            this.startMovement(this.bullet);
        }

        private void startMovement(Bullet bulletParam)
        {
            var seconds = 50;

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(seconds)
            };

            timer.Tick += (s, e) =>
            {
                var position = bulletParam.Y;
                var canvasBarrier = 480;
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
                }
            };

            timer.Start();
        }

        private void checkCollision(Bullet enemyBullet)
        {
            this.bullet.UpdateBoundingBox();
            this.player.UpdateBoundingBox();

            if (this.isCollision(this.bullet.BoundingBox, this.player.BoundingBox))
            {
                this.canvas.Children.Remove(enemyBullet.Sprite);
                this.Timer.Stop();

                this.IsShooting = false;

                this.gameManager.OnPlayerHit();
                this.checkPlayerStatus();
            }
        }

        private void checkPlayerStatus()
        {
            if (this.player.Lives == 0)
            {
                this.canvas.Children.Clear();
                this.Timer.Stop();
                this.gameManager.OnGameOver();
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

        private void playerReturnTimer(DispatcherTimer playerReturnTimer)
        {
            playerReturnTimer.Tick += (s, e) =>
            {
                this.canvas.Children.Add(this.player.Sprite);
                playerReturnTimer.Stop();
                this.restartShootingTimer();
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
            this.Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(this.random.Next(1000, 10000))
            };
            this.Timer.Tick += this.shootingTimer_Tick;
            this.Timer.Start();
        }

        #endregion
    }
    

}

