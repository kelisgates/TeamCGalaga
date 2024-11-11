using Galaga.View.Sprites;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Galaga.View.Sprites.EnemeyL4Sprites;
using Galaga.View.Sprites.EnemyL1Sprites;
using Galaga.View.Sprites.EnemyL2Sprites;
using Galaga.View.Sprites.EnemyL3Sprites;
using System;

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
        #region Data Members

        /// <summary>
        /// list of enemies in game
        /// </summary>
        public readonly List<NonAttackEnemy> Enemies;

        private readonly Canvas canvas;
        private DispatcherTimer animationTimer;
        private readonly GameManager manager;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyManager"/> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="manager">The game manager object</param>
        public EnemyManager(Canvas canvas, GameManager manager)
        {
            this.Enemies = new List<NonAttackEnemy>();
            this.manager = manager;
            this.canvas = canvas;

            this.initializeAnimationTimer();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Places the enemy.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="score">The score.</param>
        /// <param name="canvasMiddle">The canvas middle.</param>
        /// <param name="y">The y.</param>
        /// <param name="numOfEnemies">The number of enemies.</param>
        /// <param name="isAttackEnemy">if set to <c>true</c> [is attack enemy].</param>
        /// <param name="player">The player.</param>
        public void PlaceEnemy(EnemyType level, int score, double canvasMiddle, double y, int numOfEnemies,
            bool isAttackEnemy, Player player = null)
        {
            for (int i = 0; i < numOfEnemies; i++)
            {
                var widthDistance = 100;
                List<BaseSprite> sprites = new List<BaseSprite>();

                this.findAndCreateEnemy(level, sprites);

                var xPosition = this.getStartPoint(numOfEnemies, canvasMiddle) + (i * widthDistance);

                this.checkIfAttackOrNonAttackEnemy(score, y, isAttackEnemy, player, sprites, xPosition);
            }
        }

        #endregion

        #region Private Methods

        private void findAndCreateEnemy(EnemyType level, List<BaseSprite> sprites)
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
        }

        private void checkIfAttackOrNonAttackEnemy(int score, double y, bool isAttackEnemy, Player player, List<BaseSprite> sprites,
            double xPosition)
        {
            if (isAttackEnemy)
            {
                var attackEnemy = new AttackEnemy(this.manager, sprites, score, this.canvas, player)
                {
                    X = xPosition,
                    Y = y
                };
                this.Enemies.Add(attackEnemy);
            }
            else
            {
                var nonAttackEnemy = new NonAttackEnemy(sprites, score)
                {
                    X = xPosition,
                    Y = y
                };
                this.Enemies.Add(nonAttackEnemy);
            }
        }

        private void initializeAnimationTimer()
        {
            var seconds = 20;
            this.animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(seconds)
            };
            this.animationTimer.Tick += this.OnAnimationTick;
            this.animationTimer.Start();
        }

        private void OnAnimationTick(object sender, object e)
        {
            foreach (var enemy in this.Enemies)
            {
                enemy.UpdateImage();
            }
        }

        private double getStartPoint(double numOfEnemies, double canvasMiddle)
        {
            var widthDistance = 100;
            var half = 2.0;
            return canvasMiddle - (numOfEnemies * widthDistance) / half;

        }

        #endregion
    }

}
