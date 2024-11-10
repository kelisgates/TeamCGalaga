﻿using System;
using Windows.UI.Xaml;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    /// Represents an enemy in the game.
    /// </summary>
    public class NonAttackEnemy: GameObject
    {
        #region Data members

        private const int SpeedXDirection = 3;
        private const int SpeedYDirection = 0;

        private DispatcherTimer timer;
        private int steps;
        private bool movingRight;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the score value.
        /// </summary>
        /// <value>
        /// The score value.
        /// </value>
        public int ScoreValue { get; set; }

        #endregion
        

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NonAttackEnemy"/> class.
        /// </summary>
        public NonAttackEnemy(BaseSprite enemy, int score)
        {
            Sprite = enemy;
            this.moveEnemy();
            this.ScoreValue = score;
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

       

        #endregion

        #region Methods
        private void moveEnemy()
        {
            this.steps = 0;
            this.movingRight = true;
            this.timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            this.timer.Tick += (s, e) =>
            {
                if (this.movingRight)
                {
                    X += 10;
                    this.steps++;
                    if (this.steps == 10)
                    {
                        this.movingRight = false;
                        this.steps = 0;
                    }
                }
                else
                {
                    X -= 10;
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
