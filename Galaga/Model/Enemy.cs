using Galaga.View.Sprites;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    /// movement direction of enemy
    /// </summary>
    public enum MovementDirection
    {
        Right,
        Left,
        Down,
        Up
    }

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
        protected const int SpeedYDirection = 10;

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
        protected int SidewaysSteps;
        /// <summary>
        /// The horizontal steps
        /// </summary>
        protected int HorizontalSteps;
       
        /// <summary>
        /// The moving right
        /// </summary>
        protected bool MovingRight;
        /// <summary>
        /// The moving down
        /// </summary>
        protected bool MovingDown;

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
            this.SidewaysSteps = resetSteps;
            this.MovingRight = true;
            this.movementTimer(resetSteps, MovementDirection.Right);
        }

        /// <summary>
        /// Moves the enemy pattern one.
        /// </summary>
        protected void MoveEnemyPatternOne()
        {
            var resetSteps = 3;
            this.SidewaysSteps = resetSteps;
            this.MovingRight = true;
            this.movementTimer(resetSteps, MovementDirection.Left);
        }

        /// <summary>
        /// Moves the enemy pattern two.
        /// </summary>
        protected void MoveEnemyPatternTwo()
        {
            var resetSteps = 0;
            this.SidewaysSteps = resetSteps;
            this.MovingRight = true;
            this.movementTimer(resetSteps, MovementDirection.Right);
        }

        /// <summary>
        /// Moves the enemy pattern three.
        /// </summary>
        protected void MoveEnemyPatternThree()
        {
            var resetSteps = 2;
            this.SidewaysSteps = resetSteps;
            this.MovingRight = false;
            this.movementTimer(resetSteps, MovementDirection.Left);
        }

        /// <summary>
        /// Moves the enemy pattern four.
        /// </summary>
        protected void MoveEnemyPatternFour()
        {
            var resetSteps = 1;
            this.SidewaysSteps = resetSteps;
            this.MovingRight = true;
            this.movementTimer(resetSteps, MovementDirection.Right);
        }


        private void movementTimer(int resetSteps, MovementDirection direction)
        {
            var seconds = 1000;

            this.Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(seconds)
            };
            if (direction == MovementDirection.Left)
            {
                this.MovingRight = false;
                this.Timer.Tick += (_, _) => { this.checkMovingRightOrLeft(resetSteps); };
            }
            else if (direction == MovementDirection.Right)
            {
                this.MovingRight = true;
                this.Timer.Tick += (_, _) => { this.checkMovingRightOrLeft(resetSteps); };
            }
            else if (direction == MovementDirection.Down)
            {

                this.Timer.Tick += (_, _) => { this.checkMovingUpOrDown(resetSteps); };
            }
            else if (direction == MovementDirection.Up)
            {

                this.Timer.Tick += (_, _) => { this.checkMovingUpOrDown(resetSteps); };
            }

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

            this.SidewaysSteps++;
            if (this.SidewaysSteps == MovementPerStep)
            {
                this.MovingRight = !this.MovingRight;
                this.SidewaysSteps = resetSteps;
            }
        }

        private void checkMovingUpOrDown(int resetSteps)
        {
            if (this.MovingDown && Y <= 600)
            {
                MoveDown();
            }
            else
            {
                Y = 0;
            }
            this.checkWhichSpriteIsVisible();
            this.HorizontalSteps--;
            if (this.HorizontalSteps == MovementPerStep)
            {
                this.MovingDown = !this.MovingDown;
                this.HorizontalSteps = resetSteps;
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

        /// <summary>
        /// Moves bonus enemy ships
        /// </summary>
        protected void MoveBonusEnemyShip()
        {
            var resetSteps = 0;
            this.HorizontalSteps = resetSteps;
            this.MovingDown = true;
            this.movementTimer(resetSteps, MovementDirection.Down);
        }

        

        #endregion
    }
}
