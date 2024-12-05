using System;
using System.Collections.Generic;
using Galaga.View.Sprites;

namespace Galaga.Model
{
    /// <summary>
    /// Represents an enemy in the game.
    /// </summary>
    public class NonAttackEnemy: Enemy
    {
        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <value>
        /// The manager.
        /// </value>
        private EnemyManager manager { get; }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NonAttackEnemy"/> class.
        /// </summary>
        public NonAttackEnemy(ICollection<BaseSprite> sprites, int score, EnemyManager manager) : base(sprites, score)
        {
            MoveEnemy(false);
            this.manager = manager;
            this.moveEnemyBasedOnLevel();
        }

        private void moveEnemyBasedOnLevel()
        {
            int level = this.manager.Manager.Level;
            switch (level)
            {
                case 1:
                    MoveEnemy(false);
                    break;
                case 2:
                    MoveEnemyPatternFour();

                    break;
                case 3:
                    if (this.Y == 300)
                    {
                        MoveEnemyPatternTwo();
                    }
                    else
                    {
                        MoveEnemyPatternOne();
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid level");
            }
        }

        #endregion

    }
}
