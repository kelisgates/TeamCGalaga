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
        private bool canTrackPlayer;

        /// <summary>
        /// The is bonus ship
        /// </summary>
        public bool IsBonusShip;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AttackEnemy"/> class.
        /// </summary>
        public AttackEnemy(ICollection<BaseSprite> sprites, int score, Canvas canvas, CollisionManager collisionManager, bool isBonusShip, bool canTrackPlayer) : base(sprites, score)
        {
            this.canvas = canvas;
            this.collisionManager = collisionManager;
            this.IsBonusShip = isBonusShip;

            if (canTrackPlayer)
            {
                this.canTrackPlayer = true;
            }

            if (this.IsBonusShip)
            {
                MoveEnemy(this.IsBonusShip);
            }
            else
            {
                this.moveEnemyBasedOnLevel();
            }
            this.shootAtPlayer();
        }

        #endregion

        #region private methods

        private void moveEnemyBasedOnLevel()
        {
            int level = this.collisionManager.GameManager.Level;
            switch (level)
            {
                case 1:
                    MoveEnemy(this.IsBonusShip);
                    break;
                case 2:
                    MoveEnemyPatternThree();

                    break;
                case 3:
                    var y = 100;
                    if (Y.Equals(y))
                    {
                        MoveEnemyPatternTwo();
                    }
                    else
                    {
                        MoveEnemyPatternOne();
                    }
                    break;
                case 4:
                    break;
                default:
                    throw new ArgumentException("Invalid level");
            }
        }

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
                Timer.Interval = this.getShootingTimerForLevel(this.collisionManager.GameManager.Level);
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
                    X = X + Width /2,
                    Y = Y + Height
                };

                

                this.canvas.Children.Add(enemyBullet.Sprite);
                this.collisionManager.StartEnemyBulletMovement(enemyBullet, this.canvas, this.canTrackPlayer);
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in shoot: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }
        }
        
        private void restartShootingTimer()
        {
            try
            {
                Timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(this.random.Next(1000, 10000))
                };
                this.collisionManager.Timers.Add(Timer);
                Timer.Tick += this.shootingTimer_Tick;
                Timer.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in restartShootingTimer: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private TimeSpan getShootingTimerForLevel(int level)
        {
            int minInterval, maxInterval;

            switch (level)
            {
                case 1:
                    minInterval = 5000;
                    maxInterval = 200000;
                    break;

                case 2:
                    minInterval = 4000;
                    maxInterval = 500000;
                    break;
                case 3:
                    minInterval = 3000;
                    maxInterval = 250000;
                    break;
                case 4:
                    minInterval = 1000;
                    maxInterval = 25000;
                    break;
                default:
                    throw new ArgumentException("Invalid Level");

            }
            return TimeSpan.FromMilliseconds(this.random.Next(minInterval, maxInterval));
        }

        #endregion
    }
    

}

