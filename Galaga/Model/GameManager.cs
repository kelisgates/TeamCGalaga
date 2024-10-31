﻿using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Galaga.View.Sprites;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;

namespace Galaga.Model
{
    /// <summary>
    /// Manages the Galaga game play.
    /// </summary>
    public class GameManager
    {
        #region Data members

        private const double PlayerOffsetFromBottom = 30;
        private readonly Canvas canvas;
        private readonly double canvasHeight;
        private readonly double canvasWidth;

        
        private BulletManager manager;
        private Player player;
        private EnemyManager enemyManager;
        private DispatcherTimer timer;

        #endregion

        #region Properties
        /// <summary>
        /// Gets the score manager.
        /// </summary>
        /// <value>
        /// The score manager.
        /// </value>
        public ScoreManager ScoreManager { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is player bullet active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is player bullet active; otherwise, <c>false</c>.
        /// </value>
        public bool IsPlayerBulletActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [was collision].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [was collision]; otherwise, <c>false</c>.
        /// </value>
        public bool WasCollision { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameManager"/> class.
        /// </summary>
        public GameManager(Canvas canvas)
        {
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));

            this.canvas = canvas;
            this.canvasHeight = canvas.Height;
            this.canvasWidth = canvas.Width;

            this.initializeGame();
        }

        #endregion

        #region Private Methods

        

        private void initializeGame()
        {
            this.ScoreManager = new ScoreManager();
            this.createAndPlacePlayer();
            this.placeEnemies();

            

        }

        private void placeEnemies()
        {
            this.enemyManager = new EnemyManager(this.canvas);

            var canvasMiddle = this.canvasWidth / 2.0;

            this.enemyManager.placeNonAttackEnemy(EnemyType.Level1, 10, canvasMiddle, 150, 2);
            this.enemyManager.placeNonAttackEnemy(EnemyType.Level2, 20, canvasMiddle, 70, 3);
            this.enemyManager.placeAttackEnemy(EnemyType.Level3, 30, canvasMiddle, 5, 4, this.player);

            foreach (var enemy in this.enemyManager.enemies)
            {
                this.canvas.Children.Add(enemy.Sprite);
            }
        }


        private void createAndPlacePlayer()
        {
            this.player = new Player();
            this.canvas.Children.Add(this.player.Sprite);

            this.placePlayerNearBottomOfBackgroundCentered();
        }

        private void placePlayerNearBottomOfBackgroundCentered()
        {
            this.player.X = this.canvasWidth / 2 - this.player.Width / 2.0;
            this.player.Y = this.canvasHeight - this.player.Height - PlayerOffsetFromBottom;
        }


        private void startBulletMovement(BulletManager bullet)
        {
            this.timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            this.timer.Tick += (s, e) =>
            {
                var position = this.manager.Y;
                if (position > 0)
                {
                    bullet.Y -= 10;
                    bullet.UpdateBoundingBox();
                    this.checkCollision(bullet);
                    if (this.WasCollision)
                    {
                        this.WasCollision = false;
                        this.canvas.Children.Remove(bullet.Sprite);
                        this.timer.Stop();
                        this.IsPlayerBulletActive = false;
                    }
                }
                else
                {
                    this.canvas.Children.Remove(bullet.Sprite);
                    this.timer.Stop();
                    this.IsPlayerBulletActive = false;
                }
            };
            this.timer.Start();
        }

        

        private void checkCollision(BulletManager bullet)
        {
            bullet.UpdateBoundingBox();
            
            foreach (var enemy in this.enemyManager.enemies)
            {
                if (enemy is NonAttackEnemy enemySprite)
                {
                    enemy.UpdateBoundingBox();
                    if (this.isCollision(bullet.BoundingBox, enemy.BoundingBox))
                    {
                        if (enemy is AttackEnemy enemyLevelThree)
                        {
                            enemyLevelThree.IsShooting = false;
                            enemyLevelThree.Timer.Stop();
                        }


                        this.WasCollision = true;
                        this.canvas.Children.Remove(bullet.Sprite);
                        this.canvas.Children.Remove(enemySprite.Sprite);
                        this.enemyManager.enemies.Remove(enemySprite);
                        var amount = enemy.ScoreValue;
                        this.ScoreManager.Score += amount;

                        if (this.enemyManager.enemies.Count == 0)
                        {
                            this.displayGameWon();
                        }

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

        private void displayGameWon()
        {
            var gameWonTextBlock = new TextBlock
            {
                Text = "Game Won!",
                Foreground = new SolidColorBrush(Windows.UI.Colors.Green),
                FontSize = 48,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Canvas.SetLeft(gameWonTextBlock, (this.canvasWidth - gameWonTextBlock.ActualWidth) / 2);
            Canvas.SetTop(gameWonTextBlock, (this.canvasHeight - gameWonTextBlock.ActualHeight) / 2);

            this.canvas.Children.Add(gameWonTextBlock);
        }


        #endregion

        #region Public Methods
        /// <summary>
        /// Moves the player left.
        /// </summary>
        public void MovePlayerLeft()
        {
            if (this.player.X - this.player.SpeedX >= 0)
            {
                this.player.MoveLeft();
            }
            
        }

        /// <summary>
        /// Moves the player right.
        /// </summary>
        public void MovePlayerRight()
        {
            if (this.player.X + this.player.SpeedX + this.player.Width <= this.canvasWidth)
            {
                this.player.MoveRight();
            }
            
        }

        /// <summary>
        /// Player shoot a bullet.
        /// </summary>
        public void PlayerShoot()
        {
            if (this.IsPlayerBulletActive)
            {
                return;
            }
            this.manager = new BulletManager
            {
                IsShooting = true,
                X = this.player.X + 20,
                Y = this.player.Y,

            };

            this.canvas.Children.Add(this.manager.Sprite);
            this.IsPlayerBulletActive = true;
            this.startBulletMovement(this.manager);

            
        }
    }
        
        
    


    #endregion

}

