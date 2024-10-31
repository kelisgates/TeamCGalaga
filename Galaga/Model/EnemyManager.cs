using Galaga.View.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Galaga.Model;

namespace Galaga.Model
{

    public enum EnemyType
    {
        Level1,
        Level2,
        Level3
    }


    



    public class EnemyManager
    {
        public readonly List<NonAttackEnemy> enemies;
        private Canvas canvas;


        public EnemyManager(Canvas canvas)
        {
            this.enemies = new List<NonAttackEnemy>();
            this.canvas = canvas;
        }


        public void placeNonAttackEnemy(EnemyType level,int score, double canvasMiddle, double y, int numOfEnemies)
        {
            
            for (int i = 0; i < numOfEnemies; i++)
            {
                var widthDistance = 100;
                if (level == EnemyType.Level1)
                {
                    var enemySprite = new NonAttackEnemy(new EnemyL1Sprite(), score)
                    {
                        X = this.getStartPoint((double)numOfEnemies, canvasMiddle) + (i * widthDistance),
                        Y = y
                    };

                    this.enemies.Add(enemySprite);
                }
                else if (level == EnemyType.Level2)
                {
                    var enemySprite = new NonAttackEnemy(new EnemyL2Sprite(), score)
                    {
                        X = this.getStartPoint((double)numOfEnemies, canvasMiddle) + (i * widthDistance),
                        Y = y
                    };

                    this.enemies.Add(enemySprite);
                }
               
            }
        }

        public void placeAttackEnemy(EnemyType level, int score, double canvasMiddle, double y, int numOfEnemies, Player player)
        {
            for (int i = 0; i < numOfEnemies; i++)
            {
                var widthDistance = 100;

                if (level == EnemyType.Level3)
                {
                    var enemySprite = new AttackEnemy(new EnemyL3Sprite(), score, this.canvas, player)
                    {
                        X = this.getStartPoint((double)numOfEnemies, canvasMiddle) + (i * widthDistance),
                        Y = y
                    };

                    this.enemies.Add(enemySprite);
                }

            }
        }


        private double getStartPoint(double numOfEnemies, double canvasMiddle)
        {
            var widthDistance = 100;
            return canvasMiddle - (numOfEnemies * widthDistance) / 2.0;

        }
    }
}
