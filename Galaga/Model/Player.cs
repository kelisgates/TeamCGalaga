using Galaga.View.Sprites;
using Windows.UI.Xaml;
using System;

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
            this.invincibilityTimer.Tick += (s, e) =>
            {
                this.IsInvincible = false;
                this.invincibilityTimer.Stop();
            };
            this.invincibilityTimer.Start();
        }

        #endregion

    }
}
