using Galaga.View.Sprites.EnemeyL4Sprites;
using Galaga.View.Sprites.EnemyL1Sprites;
using Galaga.View.Sprites.EnemyL2Sprites;
using Galaga.View.Sprites.EnemyL3Sprites;
using Galaga.View.Sprites;
using System.Collections.Generic;
using Windows.ApplicationModel.VoiceCommands;
using Windows.UI.Xaml.Documents;

namespace Galaga.Model
{
    /// <summary>
    /// ship factory class
    /// </summary>
    public static class ShipFactory
    {
        ///// <summary>
        ///// Initializes a new instance of the <see cref="ShipFactory"/> class.
        ///// </summary>
        //public ShipFactory()
        //{
        //}

        /// <summary>
        /// Creates the player ship.
        /// </summary>
        /// <returns></returns>
        public static GameObject CreatePlayerShip()
        {
            return new Player();
        }

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
            }

            return sprites;
        }
    }


}
