using GameCore.Core;
using UnityEngine;

namespace SleepDev
{
    public class ExplosionAndFire : MonoBehaviour
    {
        [SerializeField] private string _explosionId;
        [SerializeField] private Transform _explosionPoint;
        [Space(5)]
        [SerializeField] private string _fireId;
        [SerializeField] private Transform _firePoint;
        [Space(5)]
        [SerializeField] private SoundSo _sound;

        public void PlayAll()
        {
            PlaySound();
            PlayExplosion();
            PlayFire();
        }

        public void PlayExplosion()
        {
            if(_explosionId.Length > 0)
                GCon.ExplosionPlayer.Play(_explosionId, _explosionPoint);
        }

        public void PlayFire()
        {
            if(_fireId.Length > 0)
                GCon.ExplosionPlayer.PlayParented(_fireId, _firePoint);
        }

        public void PlaySound()
        {
            _sound?.Play();
        }
        
        
    }
}