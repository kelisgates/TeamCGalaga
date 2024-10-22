using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Galaga.View.Sprites;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    /// Represents an enemy in the game.
    /// </summary>
    public class Enemy: GameObject
    {
        #region Data members

        private const int SpeedXDirection = 3;
        private const int SpeedYDirection = 0;

        private DispatcherTimer timer;
        private int steps;
        private bool movingRight;

        #endregion

        

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Enemy"/> class.
        /// </summary>
        public Enemy()
        { 
            

            Sprite = new EnemySprite();
            this.moveEnemy();
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

        private void moveEnemy()
        {
            this.steps = 0;
            this.movingRight = true;
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(100);
            this.timer.Tick += (s, e) =>
            {
                if (this.movingRight)
                {
                    this.X += 10;
                    this.steps++;
                    if (this.steps == 10)
                    {
                        this.movingRight = false;
                        this.steps = 0;
                    }
                }
                else
                {
                    this.X -= 10;
                    this.steps++;
                    if (this.steps == 10)
                    {
                        this.movingRight = true;
                        this.steps = 0;
                    }
                }
            };
            this.timer.Start();
        }

        #endregion

        

        


    }
}
