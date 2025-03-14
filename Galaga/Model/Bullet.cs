﻿namespace Galaga.Model
{
    /// <summary>
    /// manages the bullets in the game
    /// </summary>
    /// <seealso cref="Galaga.Model.GameObject" />
    public class Bullet : GameObject
    {
        #region Data members

        private const int SpeedXDirection = 0;
        private const int SpeedYDirection = 5;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is shooting.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is shooting; otherwise, <c>false</c>.
        /// </value>
        /// <returns>bool value is bullet is being shot</returns>
        public bool IsShooting { get;  set; }

        /// <summary>
        /// Gets or sets the velocity x.
        /// </summary>
        /// <value>
        /// The velocity x.
        /// </value>
        public double VelocityX { get; set; }

        /// <summary>
        /// Gets or sets the velocity y.
        /// </summary>
        /// <value>
        /// The velocity y.
        /// </value>
        public double VelocityY { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Bullet"/> class.
        /// </summary>
        public Bullet()
        {
            Sprite = new View.Sprites.Bullet();
            SetSpeed(SpeedXDirection, SpeedYDirection);
            
        }

        #endregion
       
    }
}
