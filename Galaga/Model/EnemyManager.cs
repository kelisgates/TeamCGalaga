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
        private Canvas canvas;
        private DispatcherTimer animationTimer;
        private GameManager manager;

        #endregion

        

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyManager"/> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public EnemyManager(Canvas canvas, GameManager manager)
        {
            this.Enemies = new List<NonAttackEnemy>();
            this.manager = manager;
            this.canvas = canvas;
            this.intializeAnimationTimer();
        }

        private void intializeAnimationTimer()
        {
            this.animationTimer = new DispatcherTimer();
            this.animationTimer.Interval = TimeSpan.FromMilliseconds(20); 
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

        #endregion

        #region Methods
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
                    NonAttackEnemy enemySprite = null;
                    List<BaseSprite> sprites = new List<BaseSprite>();

                    sprites.Add(new EnemyL1Sprite());
                    sprites.Add(new EnemyL1SpriteTwo());
                    enemySprite = new NonAttackEnemy(sprites, score)
                    {
                        X = this.getStartPoint(numOfEnemies, canvasMiddle) + (i * widthDistance),
                        Y = y
                    };
                    this.Enemies.Add(enemySprite);
                    
                }
                else if (level == EnemyType.Level2)
                {
                    NonAttackEnemy enemySprite = null;
                    List<BaseSprite> sprites = new List<BaseSprite>();
                    sprites.Add(new EnemyL2Sprite());
                    sprites.Add(new EnemyL2SpriteTwo());
                    enemySprite = new NonAttackEnemy(sprites, score)
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
                    AttackEnemy enemySprite = null;
                    List<BaseSprite> sprites = new List<BaseSprite>();
                    sprites.Add(new EnemyL3Sprite());
                    sprites.Add(new EnemyL3SpriteTwo());
                    enemySprite = new AttackEnemy(this.manager,sprites, score, this.canvas, player)
                    {
                        X = this.getStartPoint(numOfEnemies, canvasMiddle) + (i * widthDistance),
                        Y = y
                    };
                    this.Enemies.Add(enemySprite);
                    
                    
                } else if (level == EnemyType.Level4)
                {
                    AttackEnemy enemySprite = null;
                    List<BaseSprite> sprites = new List<BaseSprite>();
                    sprites.Add(new EnemyL4Sprite());
                    sprites.Add(new EnemyL4SpriteTwo());
                    enemySprite = new AttackEnemy(this.manager, sprites, score, this.canvas, player)
                    {
                        X = this.getStartPoint(numOfEnemies, canvasMiddle) + (i * widthDistance),
                        Y = y
                    };
                    this.Enemies.Add(enemySprite);
                    
                    
                }

               
            }
        }

        #endregion

        #region Private Methods

        private double getStartPoint(double numOfEnemies, double canvasMiddle)
        {
            var widthDistance = 100;
            var half = 2.0;
            return canvasMiddle - (numOfEnemies * widthDistance) / half;

        }

        #endregion
    }

}
