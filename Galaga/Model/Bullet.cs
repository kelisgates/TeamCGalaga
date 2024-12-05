namespace Galaga.Model
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
        public double Dx { get; set; }
        public double Dy { get; set; }

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
