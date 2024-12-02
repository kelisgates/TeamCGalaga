using System.Runtime.Serialization;

namespace Galaga.Model
{
    /// <summary>
    /// defines the high score class
    /// </summary>
    [DataContract]
    public class HighScore
    {
        #region Data Memebers

        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        /// <value>
        /// The name of the player.
        /// </value>
        [DataMember]
        public string PlayerName { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        [DataMember]
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        [DataMember]
        public int Level { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="HighScore"/> class.
        /// </summary>
        public HighScore()
        {
            this.PlayerName = null;
            this.Score = 0;
            this.Level = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HighScore"/> class.
        /// </summary>
        /// <param name="playerName">Name of the player.</param>
        /// <param name="score">The score.</param>
        /// <param name="level">The level.</param>
        public HighScore(string playerName, int score, int level)
        {
            this.PlayerName = playerName;
            this.Score = score;
            this.Level = level;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{this.PlayerName} / {this.Score} / {this.Level}";
        }

        #endregion
    }
}
