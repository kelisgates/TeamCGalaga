
using System;
using System.Diagnostics;
using Windows.Media.Core;
using Windows.Media.Devices;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Galaga.Model
{
    public class SoundManager
    {
        //TODO: Implement SoundManager

        private readonly MediaPlayer playerFireSound;
        private readonly MediaPlayer enemyFireSound;
        private readonly MediaPlayer enemyHitSound;
        private readonly MediaPlayer playerHitSound;
        private readonly MediaPlayer playerDeathSound;
        private readonly MediaPlayer enemyDeathSound;

        public SoundManager()
        {
            this.playerFireSound = new MediaPlayer();
            this.enemyFireSound = new MediaPlayer();
            this.enemyHitSound = new MediaPlayer();
            this.playerHitSound = new MediaPlayer();
            this.playerDeathSound = new MediaPlayer();
            this.enemyDeathSound = new MediaPlayer();
        }

        public void PlayPlayerFireSound()
        {
            this.playerFireSound.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Sounds/playerFire.wav"));
            this.playerFireSound.Play();
        }

        public void PlayEnemyFireSound()
        {
            this.enemyFireSound.Source = MediaSource.CreateFromUri(new Uri("ms-appx///Sounds/enemyFire.wav"));
            this.enemyFireSound.Play();
        }

        public void PlayEnemyHitSound()
        {
            this.enemyHitSound.Source = MediaSource.CreateFromUri(new Uri("ms-appx///Sounds/enemyHit.wav"));
            this.enemyHitSound.Play();
        }

        public void PlayPlayerHitSound()
        {
            this.playerHitSound.Source = MediaSource.CreateFromUri(new Uri("ms-appx///Sounds/playerHit.wav"));
            this.playerHitSound.Play();
        }

        public void PlayPlayerDeathSound()
        {
            this.playerDeathSound.Source = MediaSource.CreateFromUri(new Uri("ms-appx///Sounds/playerDeath.wav"));
            this.playerDeathSound.Play();
        }

        public void PlayEnemyDeathSound()
        {
            this.enemyDeathSound.Source = MediaSource.CreateFromUri(new Uri("ms-appx///Sounds/enemyDeath.wav"));
            this.enemyDeathSound.Play();
        }

    }
}
