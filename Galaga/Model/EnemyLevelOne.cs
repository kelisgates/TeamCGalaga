using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    /// main class for level one enemy
    /// </summary>
    /// <seealso cref="Galaga.Model.Enemy" />
    public class EnemyLevelOne : Enemy
    
    {
        #region Data members



        private const int SpeedXDirection = 3;
        private const int SpeedYDirection = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyLevelTwo"/> class.
        /// </summary>
        public EnemyLevelOne()
        {
            Sprite = new EnemySprite();
            ScoreValue = 10;
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

        #endregion
    }
}
