using System.Collections.Generic;
using System.ComponentModel;

namespace Galaga.Model
{
    /// <summary>
    /// manages the score of the game.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class ScoreManager : INotifyPropertyChanged
    {
        private int score;

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public int Score
        {
            get => this.score;
            set
            {
                if (this.score != value)
                {
                    this.score = value;
                    this.OnPropertyChanged(nameof(this.Score));
                }
            }
        }

        
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
