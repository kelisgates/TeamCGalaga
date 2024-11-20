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
                    X = X + Width /2,
                    Y = Y + Height
                };

                this.canvas.Children.Add(enemyBullet.Sprite);
                this.collisionManager.StartEnemyBulletMovement(enemyBullet, this.canvas);
                
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

