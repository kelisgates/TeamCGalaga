using Galaga.View.Sprites.EnemeyL4Sprites;
using Galaga.View.Sprites.EnemyL1Sprites;
using Galaga.View.Sprites.EnemyL2Sprites;
using Galaga.View.Sprites.EnemyL3Sprites;
using Galaga.View.Sprites;
using System.Collections.Generic;
using Galaga.View.Sprites.BossSprite;

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
        Level4,
        /// <summary>
        /// The bonus enemy
        /// </summary>
        Bonus,
        /// <summary>
        /// The boss
        /// </summary>
        Boss
    }

    /// <summary>
    /// ship factory class
    /// </summary>
    public static class ShipFactory
    {
        /// <summary>
        /// Creates the enemy ship.
        /// </summary>
        /// <param name="level">The Level.</param>
        /// <returns></returns>
        public static ICollection<BaseSprite> CreateEnemyShip(EnemyType level)
        {
            ICollection<BaseSprite> sprites = new List<BaseSprite>();

            return findAndCreateEnemy(level, sprites);
        }

        private static ICollection<BaseSprite> findAndCreateEnemy(EnemyType level, ICollection<BaseSprite> sprites)
        {
            switch (level)
            {
                case EnemyType.Level1:
                    sprites.Add(new EnemyL1Sprite());
                    sprites.Add(new EnemyL1SpriteTwo());
                    break;
                case EnemyType.Level2:
                    sprites.Add(new EnemyL2Sprite());
                    sprites.Add(new EnemyL2SpriteTwo());
                    break;
                case EnemyType.Level3:
                    sprites.Add(new EnemyL3Sprite());
                    sprites.Add(new EnemyL3SpriteTwo());
                    break;
                case EnemyType.Level4:
                    sprites.Add(new EnemyL4Sprite());
                    sprites.Add(new EnemyL4SpriteTwo());
                    break;
                case EnemyType.Boss:
                    sprites.Add(new BossSprite());
                    sprites.Add(new BossSprite2());
                    break;
            }

            return sprites;
        }

    }


}
