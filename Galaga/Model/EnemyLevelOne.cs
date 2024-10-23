using Galaga.View.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

        #endregion
    }
}
