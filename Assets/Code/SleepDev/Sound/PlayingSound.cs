using UnityEngine;

namespace SleepDev.Sound
{
    public class PlayingSound
    {
        private AudioSource _source;

        public PlayingSound(AudioSource source)
        {
            _source = source;
        }
        
        public void Stop()
        {
            _source.Stop();
        }

        public void SetLoop(bool loop)
        {
            _source.loop = loop;
        }

        public void SetVolume(float volume)
        {
            _source.volume = volume;
        }

    }
}