using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Galaga.View.Sprites;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    /// <summary>
    /// Represents an enemy in the game.
    /// </summary>
    public class Enemy: GameObject
    {
        #region Data members

        private const int SpeedXDirection = 3;
        private const int SpeedYDirection = 0;

        #endregion

        #region Properties        
        /// <summary>
        /// Gets the enemy level one.
        /// </summary>
        /// <value>
        /// The enemy level one.
        /// </value>
        public EnemySprite EnemyLevelOne { get; }

        

        #endregion

        #region Constructors

        public Enemy()
        { 
            

            Sprite = new EnemySprite();
            
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void SetPosition(BaseSprite sprite, double x, double y)
        {
            Canvas.SetLeft(sprite, x);
            Canvas.SetTop(sprite, y);
        }

        /// <summary>
        /// Adds to canvas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="sprite">The sprite.</param>
        public void AddToCanvas(Canvas canvas, BaseSprite sprite)
        {
            if (!canvas.Children.Contains(sprite))
            {
                canvas.Children.Add(sprite);
            }
        }

        #endregion


    }
}
