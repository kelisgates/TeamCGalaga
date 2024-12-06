using Galaga.View.Sprites;
using Windows.UI.Xaml;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Galaga.Model
{
    /// <summary>
    /// Represents a Player in the game.
    /// </summary>
    public class Player : GameObject
    {
        #region Data members

        private const int SpeedXDirection = 3;
        private const int SpeedYDirection = 0;

        private DispatcherTimer invincibilityTimer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the lives.
        /// </summary>
        /// <value>
        /// The lives.
        /// </value>
        public int Lives { get; set; }

        /// <summary>
        /// Gets or sets the bullets shot.
        /// </summary>
        /// <value>
        /// The bullets shot.
        /// </value>
        public int BulletsShot { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is shooting.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is shooting; otherwise, <c>false</c>.
        /// </value>
        public bool IsShooting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is moving left.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is moving left; otherwise, <c>false</c>.
        /// </value>
        public bool IsMovingLeft { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is moving right.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is moving right; otherwise, <c>false</c>.
        /// </value>
        public bool IsMovingRight { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is invincible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is invincible; otherwise, <c>false</c>.
        /// </value>
        public bool IsInvincible { get; private set; }


        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        public Player()
        {
            Sprite = new PlayerSprite();
            SetSpeed(SpeedXDirection, SpeedYDirection);
            
            this.Score = 0;
            this.Lives = 3;
            
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the invincibility.
        /// </summary>
        /// <param name="durationInSeconds">The duration in seconds.</param>
        public void StartInvincibility(int durationInSeconds)
        {
            this.IsInvincible = true;
            this.invincibilityTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(durationInSeconds)
            };
            this.invincibilityTimer.Tick += (_, _) =>
            {
                this.IsInvincible = false;
                this.invincibilityTimer.Stop();
            };
            this.invincibilityTimer.Start();
        }

        /// <summary>
        /// Plays the explosion animation.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="canvas">The canvas.</param>
        public void PlayExplosionAnimation(double x, double y, Canvas canvas)
        {
            var explosionFrames = new List<Image>
            {
                new Image {Source = new BitmapImage(new Uri("ms-appx:///Assets/ExplosionImages/ExplosionImage3.PNG"))},
                new Image {Source = new BitmapImage(new Uri("ms-appx:///Assets/ExplosionImages/ExplosionImage2.PNG"))},
                new Image {Source = new BitmapImage(new Uri("ms-appx:///Assets/ExplosionImages/ExplosionImage1.PNG"))}
            };

            var explosion = new Image();
            canvas.Children.Add(explosion);
            Canvas.SetLeft(explosion, x);
            Canvas.SetTop(explosion, y);

            var frameIndex = 0;
            var explosionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };

            explosionTimer.Tick += (_, _) =>
            {
                if (frameIndex < explosionFrames.Count)
                {
                    explosion.Source = explosionFrames[frameIndex].Source;
                    frameIndex++;
                }
                else
                {
                    explosionTimer.Stop();
                    canvas.Children.Remove(explosion);
                }

            };
            explosionTimer.Start();
        }

        #endregion

    }
}
