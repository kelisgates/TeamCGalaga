
using System;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace Galaga.Model
{
    /// <summary>
    /// Manages the playing of the game sound effects. 
    /// </summary>
    public class SoundManager
    {
        #region Data Members

        private readonly MediaPlayer playerFireSound;
        private readonly MediaPlayer enemyFireSound;
        private readonly MediaPlayer enemyHitSound;
        private readonly MediaPlayer playerHitSound;
        private readonly MediaPlayer gameOverSound;
        private readonly MediaPlayer gameWonSound;
        private readonly MediaPlayer bonusEnemySound;
        private readonly MediaPlayer powerUpActiveSound;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundManager"/> class.
        /// </summary>
        public SoundManager()
        {
            this.playerFireSound = new MediaPlayer();
            this.enemyFireSound = new MediaPlayer();
            this.enemyHitSound = new MediaPlayer();
            this.playerHitSound = new MediaPlayer();
            this.gameOverSound = new MediaPlayer();
            this.gameWonSound = new MediaPlayer();
            this.bonusEnemySound = new MediaPlayer{IsLoopingEnabled = true};
            this.powerUpActiveSound = new MediaPlayer { IsLoopingEnabled = true };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Plays the player fire sound.
        /// </summary>
        public void PlayPlayerFireSound()
        {
            this.playerFireSound.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Sounds/playerFire.wav"));
            this.playerFireSound.Play();
        }

        /// <summary>
        /// Plays the enemy fire sound.
        /// </summary>
        public void PlayEnemyFireSound()
        {
            this.enemyFireSound.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Sounds/enemyFire.wav"));
            this.enemyFireSound.Play();
        }

        /// <summary>
        /// Plays the enemy hit sound.
        /// </summary>
        public void PlayEnemyHitSound()
        {
            this.enemyHitSound.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Sounds/enemyHit.wav"));
            this.enemyHitSound.Play();
        }

        /// <summary>
        /// Plays the player death sound.
        /// </summary>
        public void PlayPlayerDeathSound()
        {
            this.playerHitSound.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Sounds/playerDeath.wav"));
            this.playerHitSound.Play();
        }

        /// <summary>
        /// Plays the game over sound.
        /// </summary>
        public void PlayGameOverSound()
        {
            this.gameOverSound.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Sounds/gameOver.wav"));
            this.gameOverSound.Play();
        }

        /// <summary>
        /// Plays the sound effect when the game is won.
        /// </summary>
        public void PlayGameWonSound()
        {
            this.gameWonSound.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Sounds/gameWon.wav"));
            this.gameWonSound.Play();
        }

        /// <summary>
        /// Plays the bonus eneny sound.
        /// </summary>
        public void PlayBonusEnenySound()
        {
            this.bonusEnemySound.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Sounds/bonusEnemy.wav"));
            this.bonusEnemySound.Play();
        }

        /// <summary>
        /// Stops the bonus enemy sound.
        /// </summary>
        public void StopBonusEnemySound()
        {
            this.bonusEnemySound.Pause();
        }

        /// <summary>
        /// Plays the power up sound.
        /// </summary>
        public void PlayPowerUpSound()
        {
            this.powerUpActiveSound.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Sounds/powerUpActive.wav"));
            this.powerUpActiveSound.Play();
        }

        /// <summary>
        /// Stops the power up sound.
        /// </summary>
        public void StopPowerUpSound()
        {
            this.powerUpActiveSound.Pause();
        }

        #endregion

    }
}
