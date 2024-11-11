using System;
using Windows.UI.Xaml;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
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

        /// <summary>
        /// The sprites used for animation.
        /// </summary>
        public readonly List<BaseSprite> Sprites;
        private bool isFirstSpriteVisible = true;

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
        public NonAttackEnemy(List<BaseSprite> sprites, int score)
        {
            
            this.Sprites = sprites;
            Sprite = sprites[0];
            
            this.moveEnemy();
            this.ScoreValue = score;
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the image of the sprites to simulate animation.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">There must be at least two sprites to toggle between.</exception>
        public void UpdateImage()
        {

            if (this.Sprites.Count < 2)
            {
                throw new InvalidOperationException("There must be at least two sprites to toggle between.");
            }

            this.Sprites[0].Visibility = Visibility.Collapsed;
            this.Sprites[1].Visibility = Visibility.Collapsed;

            this.chooseWhichSpriteToDisplay();

        }

        private void chooseWhichSpriteToDisplay()
        {
            if (this.isFirstSpriteVisible)
            {
                this.Sprites[0].Visibility = Visibility.Visible;
                Sprite = this.Sprites[0];
                this.isFirstSpriteVisible = false;

            }
            else
            {
                this.Sprites[1].Visibility = Visibility.Visible;
                Sprite = this.Sprites[1];
                this.isFirstSpriteVisible = true;
            }

        }

        private void moveEnemy()
        {
            var resetSteps = 0;
            this.steps = resetSteps;
            this.movingRight = true;
            var seconds = 1000;

            this.timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(seconds)
            };

            this.timer.Tick += (s, e) => { this.checkMovingRightOrLeft(resetSteps); };

            this.timer.Start();
        }

        private void checkMovingRightOrLeft(int resetSteps)
        {

            if (this.movingRight)
            {
                X += MovementPerStep;
            }
            else
            {
                X -= MovementPerStep;
            }

            this.checkWhichSpriteIsVisible();

            this.steps++;
            if (this.steps == MovementPerStep)
            {
                this.movingRight = !this.movingRight;
                this.steps = resetSteps;
            }

        }

        private void checkWhichSpriteIsVisible()
        {
            BaseSprite visibleSprite = this.isFirstSpriteVisible ? this.Sprites[0] : this.Sprites[1];
            BaseSprite hiddenSprite = this.isFirstSpriteVisible ? this.Sprites[1] : this.Sprites[0];

            Canvas.SetLeft(visibleSprite, X);
            Canvas.SetTop(visibleSprite, Y);

            Canvas.SetLeft(hiddenSprite, X);
            Canvas.SetTop(hiddenSprite, Y);
        }

        #endregion

    }
}
