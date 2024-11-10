using Galaga.View.Sprites;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    /// enemy type levels
    /// </summary>
    public enum EnemyType
    {
        /// <summary>
        /// the level1 enemy
        /// </summary>
        Level1,
        /// <summary>
        /// The level2 enemy
        /// </summary>
        Level2,
        /// <summary>
        /// The level3 enemy
        /// </summary>
        Level3,
        /// <summary>
        /// The level4 enemy
        /// </summary>
        Level4
    }

    /// <summary>
    /// manages enemies in the game
    /// </summary>
    public class EnemyManager
    {
        /// <summary>
        /// list of enemies in game
        /// </summary>
        public readonly List<NonAttackEnemy> Enemies;
        private Canvas canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyManager"/> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public EnemyManager(Canvas canvas)
        {
            this.Enemies = new List<NonAttackEnemy>();
            this.canvas = canvas;
        }

        /// <summary>
        /// Places the non attack enemy.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="score">The score.</param>
        /// <param name="canvasMiddle">The canvas middle.</param>
        /// <param name="y">The y.</param>
        /// <param name="numOfEnemies">The number of enemies.</param>
        public void PlaceNonAttackEnemy(EnemyType level,int score, double canvasMiddle, double y, int numOfEnemies)
        {
            
            for (int i = 0; i < numOfEnemies; i++)
            {
                var widthDistance = 100;
                if (level == EnemyType.Level1)
                {
                    var enemySprite = new NonAttackEnemy(new EnemyL1Sprite(), score)
                    {
                        X = this.getStartPoint(numOfEnemies, canvasMiddle) + (i * widthDistance),
                        Y = y
                    };

                    this.Enemies.Add(enemySprite);
                }
                else if (level == EnemyType.Level2)
                {
                    var enemySprite = new NonAttackEnemy(new EnemyL2Sprite(), score)
                    {
                        X = this.getStartPoint(numOfEnemies, canvasMiddle) + (i * widthDistance),
                        Y = y
                    };

                    this.Enemies.Add(enemySprite);
                }
               
            }
        }

        /// <summary>
        /// Places the attack enemy.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="score">The score.</param>
        /// <param name="canvasMiddle">The canvas middle.</param>
        /// <param name="y">The y.</param>
        /// <param name="numOfEnemies">The number of enemies.</param>
        /// <param name="player">The player.</param>
        public void PlaceAttackEnemy(EnemyType level, int score, double canvasMiddle, double y, int numOfEnemies, Player player)
        {
            for (int i = 0; i < numOfEnemies; i++)
            {
                var widthDistance = 100;

                if (level == EnemyType.Level3)
                {
                    var enemySprite = new AttackEnemy(new EnemyL3Sprite(), score, this.canvas, player)
                    {
                        X = this.getStartPoint(numOfEnemies, canvasMiddle) + (i * widthDistance),
                        Y = y
                    };

                    this.Enemies.Add(enemySprite);
                } else if (level == EnemyType.Level4)
                {
                    var enemySprite = new AttackEnemy(new EnemyL4Sprite(), score, this.canvas, player)
                    {
                        X = this.getStartPoint(numOfEnemies, canvasMiddle) + (i * widthDistance),
                        Y = y
                    };

                    this.Enemies.Add(enemySprite);
                }

            }
        }


        private double getStartPoint(double numOfEnemies, double canvasMiddle)
        {
            var widthDistance = 100;
            var half = 2.0;
            return canvasMiddle - (numOfEnemies * widthDistance) / half;

        }
    }
}
