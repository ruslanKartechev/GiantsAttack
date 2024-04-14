using System;
using System.Collections.Generic;
using SleepDev.Sound;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterBaseGun : MonoBehaviour, IHelicopterGun
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _rotatable;
        [SerializeField] private List<HelicopterGunBarrel> _barrels;
        [SerializeField] private SoundSo _reloadSound;
        private Action _onReloaded;
        
        public List<HelicopterGunBarrel> Barrels => _barrels;
        
        public Transform Rotatable => _rotatable;

        public void StopAnimations()
        {
            _animator.enabled = false;
        }
        
        public void PlayGunsInstallAnimation()
        {
            _animator.Play("GunsInstall");
        }

        public void PlayReload(Action onReloaded)
        {
            _onReloaded = onReloaded;
            _animator.Play("Reload");
        }

        public void AnimEvent_OnReloaded()
        {
            _onReloaded?.Invoke();
            _onReloaded = null;
        }

        public void AnimEvent_OnReloadSound()
        {
            _reloadSound.Play();
        }
        
    }
}