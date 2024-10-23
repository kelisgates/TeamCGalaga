using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    /// class for level two enemy
    /// </summary>
    public class EnemyLevelTwo : Enemy
    {
        #region Data members

        private const int SpeedXDirection = 3;
        private const int SpeedYDirection = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyLevelTwo"/> class.
        /// </summary>
        public EnemyLevelTwo()
        {
            Sprite = new EnemyL2();
            this.ScoreValue = 20;
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

        #endregion


    }
}
