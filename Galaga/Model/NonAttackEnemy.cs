using System;
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
        private const int MovementPerStep = 10;

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
            var resetSteps = 0;
            this.steps = resetSteps;
            this.movingRight = true;
            var seconds = 100;
            this.timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(seconds)
            };
            this.timer.Tick += (s, e) =>
            {
                if (this.movingRight)
                {
                    X += MovementPerStep;
                    this.steps++;
                    if (this.steps == MovementPerStep)
                    {
                        this.movingRight = false;
                        this.steps = resetSteps;
                    }
                }
                else
                {
                    X -= MovementPerStep;
                    this.steps++;
                    if (this.steps == MovementPerStep)
                    {
                        this.movingRight = true;
                        this.steps = resetSteps;
                    }
                }
            };
            this.timer.Start();
        }

        #endregion

        

        


    }
}
