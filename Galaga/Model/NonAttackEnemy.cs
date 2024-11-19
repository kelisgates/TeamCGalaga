using System.Collections.Generic;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    /// Represents an enemy in the game.
    /// </summary>
    public class NonAttackEnemy: Enemy
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NonAttackEnemy"/> class.
        /// </summary>
        public NonAttackEnemy(ICollection<BaseSprite> sprites, int score) : base(sprites, score)
        {
            MoveEnemy();
        }

        #endregion

    }
}
