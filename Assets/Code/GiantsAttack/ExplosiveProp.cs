using GameCore.Cam;
using GameCore.Core;
using GameCore.UI;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class ExplosiveProp : StageListener
    {
        [SerializeField] private bool _useAimAtUI;
        [SerializeField] private float _maxDistanceToEnemy;
        [SerializeField] private float _damageToEnemy;
        [SerializeField] private float _maxHealth;
        [SerializeField] private float _force;
        [SerializeField] private Transform _pushDir;
        [SerializeField] private ExplosiveVehicle _explosive;
        [SerializeField] private PropHealth _health;
        [SerializeField] private ParticleSystem _pool;
        public override void OnActivated()
        {
            _health.SetMaxHealth(_maxHealth);
            _health.SetDamageable(true);
            _health.OnDead += OnDead;
            _health.OnFirstDamaged += OnFirstDamaged;
            if (_useAimAtUI)
            {
                var ui = GCon.UIFactory.GetGameplayMenu() as IGameplayMenu;
                ui.ShootAtTargetUI.ShowAndFollow(_pushDir);
            }
        }

        private void OnFirstDamaged(IDamageable obj)
        {
            _health.OnFirstDamaged -= OnFirstDamaged;
            _pool.gameObject.SetActive(true);
            _pool.Play();
        }

        public override void OnStopped()
        {
            HideUI();
        }

        public override void OnCompleted()
        {
            HideUI();
        }

        private void HideUI()
        {
            if (!_useAimAtUI)
                return;
            var ui = GCon.UIFactory.GetGameplayMenu() as IGameplayMenu;
            if(ui.ShootAtTargetUI.CurrentTarget == _pushDir)
                ui.ShootAtTargetUI.Hide();
        }
        
        private void OnDead(IDamageable obj)
        {
            _health.OnDead -= OnDead;
            _explosive.Explode(_pushDir.forward * _force);
            CameraContainer.Shaker.PlayDefault();
            HideUI();
            var distanceToEnemy = (Enemy.Point.position - transform.position).XZDistance2();
            if (distanceToEnemy < _maxDistanceToEnemy * _maxDistanceToEnemy)
            {
                var section = Enemy.BodySectionsManager.GetRandomSection();
                section.targets.Random().Damageable.TakeDamage(new DamageArgs(_damageToEnemy, transform.position, transform.forward, true));
            }
        }
    }
}