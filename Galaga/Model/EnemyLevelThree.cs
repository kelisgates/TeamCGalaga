using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    ///  class for enemy level three
    /// </summary>
    public class EnemyLevelThree : Enemy
    {
        #region Data members

        private const int SpeedXDirection = 3;
        private const int SpeedYDirection = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyLevelThree"/> class.
        /// </summary>
        public EnemyLevelThree()
        {
            Sprite = new EnemyL3();
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

        #endregion
    }
}
