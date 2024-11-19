using Galaga.View.Sprites;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    /// abstract class for enemy
    /// </summary>
    /// <seealso cref="Galaga.Model.GameObject" />
    public abstract class Enemy : GameObject
    {
        #region Data members

        /// <summary>
        /// The speed x direction
        /// </summary>
        protected const int SpeedXDirection = 3;

        /// <summary>
        /// The speed y direction
        /// </summary>
        protected const int SpeedYDirection = 0;

        /// <summary>
        /// The movement per step
        /// </summary>
        protected const int MovementPerStep = 10;

        /// <summary>
        /// The timer
        /// </summary>
        public DispatcherTimer Timer;

        /// <summary>
        /// The steps
        /// </summary>
        protected int Steps;
       
        /// <summary>
        /// The moving right
        /// </summary>
        protected bool MovingRight;

        /// <summary>
        /// The sprites used for animation.
        /// </summary>
        public readonly ICollection<BaseSprite> Sprites;
       
        /// <summary>
        /// The is first sprite visible
        /// </summary>
        protected bool IsFirstSpriteVisible = true;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the score value.
        /// </summary>
        /// <value>
        /// The score value.
        /// </value>
        /// <returns>int the score value</returns>
        public int ScoreValue { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Enemy"/> class.
        /// </summary>
        protected Enemy(ICollection<BaseSprite> sprites, int score)
        {
            this.Sprites = sprites;
            Sprite = this.getFirstSprite();
            this.ScoreValue = score;
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

        #endregion

        #region Methods

        private BaseSprite getFirstSprite()
        {
            foreach (var sprite in this.Sprites)
            {
                return sprite;
            }
            throw new InvalidOperationException("Sprites collection is empty.");
        }

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

            foreach (var sprite in this.Sprites)
            {
                sprite.Visibility = Visibility.Collapsed;
            }

            this.chooseWhichSpriteToDisplay();
        }

        private void chooseWhichSpriteToDisplay()
        {
            var spriteArray = new List<BaseSprite>(this.Sprites);
            if (this.IsFirstSpriteVisible)
            {
                spriteArray[0].Visibility = Visibility.Visible;
                Sprite = spriteArray[0];
                this.IsFirstSpriteVisible = false;
            }
            else
            {
                spriteArray[1].Visibility = Visibility.Visible;
                Sprite = spriteArray[1];
                this.IsFirstSpriteVisible = true;
            }
        }

        /// <summary>
        /// Moves the enemy.
        /// </summary>
        protected void MoveEnemy()
        {
            var resetSteps = 0;
            this.Steps = resetSteps;
            this.MovingRight = true;
            var seconds = 1000;

            this.Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(seconds)
            };

            this.Timer.Tick += (s, e) => { this.checkMovingRightOrLeft(resetSteps); };

            this.Timer.Start();
        }

        private void checkMovingRightOrLeft(int resetSteps)
        {
            if (this.MovingRight)
            {
                X += MovementPerStep;
            }
            else
            {
                X -= MovementPerStep;
            }

            this.checkWhichSpriteIsVisible();

            this.Steps++;
            if (this.Steps == MovementPerStep)
            {
                this.MovingRight = !this.MovingRight;
                this.Steps = resetSteps;
            }
        }

        private void checkWhichSpriteIsVisible()
        {
            var spriteArray = new List<BaseSprite>(this.Sprites);
            BaseSprite visibleSprite = this.IsFirstSpriteVisible ? spriteArray[0] : spriteArray[1];
            BaseSprite hiddenSprite = this.IsFirstSpriteVisible ? spriteArray[1] : spriteArray[0];

            Canvas.SetLeft(visibleSprite, X);
            Canvas.SetTop(visibleSprite, Y);

            Canvas.SetLeft(hiddenSprite, X);
            Canvas.SetTop(hiddenSprite, Y);
        }

        #endregion
    }
}
