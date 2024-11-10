﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Galaga.View.Sprites;
using Windows.UI.Xaml.Media;
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
        private const int TimerInterval = 1000;
        
        private Random random;
        private readonly Canvas canvas;

        private BulletManager bullet;
        private readonly Player player;



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
        public AttackEnemy(BaseSprite enemy, int score, Canvas canvas, Player player) : base(enemy, score)
        {
            Sprite = enemy;
            ScoreValue = score;
            this.canvas = canvas;
            this.player = player;
            SetSpeed(SpeedXDirection, SpeedYDirection);
            this.shootAtPlayer();
        }



        #endregion

        #region private methods


        /// <summary>
        /// Shoots at Player.
        /// </summary>
        private void shootAtPlayer()
        {
            this.random = new Random();

            this.Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(this.random.Next(TimerInterval, TimerInterval))
            };
            this.Timer.Tick += this.shootingTimer_Tick;
            this.Timer.Start();
        }

        
        

        private void shootingTimer_Tick(object sender, object e)
        {
            this.shoot();
            this.Timer.Interval = TimeSpan.FromMilliseconds(this.random.Next(TimerInterval, TimerInterval));
        }

        private void shoot()
        {
            if (this.IsShooting)
            {
                return;
            }
            this.bullet = new BulletManager
            {
                X = X ,
                Y = Y 
            };
            this.canvas.Children.Add(this.bullet.Sprite);
            this.IsShooting = true;
            this.startMovement(this.bullet);

        }

        private void startMovement(BulletManager bulletParam)
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

        private void checkCollision(BulletManager bulletManager)
        {
            this.bullet.UpdateBoundingBox();
            this.player.UpdateBoundingBox();

            if (this.isCollision(this.bullet.BoundingBox, this.player.BoundingBox))
            {
                this.canvas.Children.Remove(bulletManager.Sprite);
                this.Timer.Stop();
                this.canvas.Children.Clear();
                this.displayGameOver();
            }
        }

        private bool isCollision(BoundingBox boundingBox1, BoundingBox boundingBox2)
        {
            return !(boundingBox1.Left > boundingBox2.Left + boundingBox2.Width ||
                     boundingBox1.Left + boundingBox1.Width < boundingBox2.Left ||
                     boundingBox1.Top > boundingBox2.Top + boundingBox2.Height ||
                     boundingBox1.Top + boundingBox1.Height < boundingBox2.Top);
        }

        private void displayGameOver()
        {
            var gameWonTextBlock = new TextBlock
            {
                Text = "Game Over!",
                Foreground = new SolidColorBrush(Windows.UI.Colors.Red),
                FontSize = 48,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Canvas.SetLeft(gameWonTextBlock, (this.canvas.Width - gameWonTextBlock.ActualWidth) / 2);
            Canvas.SetTop(gameWonTextBlock, (this.canvas.Height - gameWonTextBlock.ActualHeight) / 2);

            this.canvas.Children.Add(gameWonTextBlock);
        }

        #endregion
    }
    

}

