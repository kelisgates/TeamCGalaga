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
        #region Data Members

        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <value>
        /// The manager.
        /// </value>
        private EnemyManager Manager { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NonAttackEnemy"/> class.
        /// </summary>
        public NonAttackEnemy(ICollection<BaseSprite> sprites, int score, EnemyManager manager) : base(sprites, score)
        {
            
            this.Manager = manager;
            this.moveEnemyBasedOnLevel();
        }

        #endregion

        #region Methods

        private void moveEnemyBasedOnLevel()
        {
            int level = this.Manager.Manager.Level;
            switch (level)
            {
                case 1:
                    MoveEnemy(false);
                    break;
                case 2:
                    MoveEnemyPatternFour();

                    break;
                case 3:
                    if (ScoreValue == 10)
                    {
                        MoveEnemyPatternTwo();
                    }
                    else
                    {
                        MoveEnemyPatternFour();
                    }
                    break;
                case 4:
                    break;
                default:
                    throw new ArgumentException("Invalid level");
            }
        }

        #endregion

    }
}
