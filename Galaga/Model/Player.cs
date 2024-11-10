﻿using Galaga.View.Sprites;

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




        

    }
}
