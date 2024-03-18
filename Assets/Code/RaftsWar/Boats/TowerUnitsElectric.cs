using System.Collections;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class TowerUnitsElectric : MonoBehaviour, ITowerUnitsController
    {
        private const float AttackRadiusAdded = 1f;
        
        [SerializeField] private List<TowerElectricShooter> _shooters;
        private TowerLevelSettings _settings;
        private Coroutine _processing;
        private Coroutine _shooting;
        private bool _targetIsTower;
        private bool _isShooting;
        private ITarget _currentTarget;

        public float Radius { get; set; }

        public ITower Tower { get; set; }
        
        public void UpdateSettings(TowerLevelSettings settings)
        {
            _settings = settings;
            Radius = settings.radius;
            SetStats();
        }

        public void SetOptionalPoints(List<Transform> optionalSpawnPoints)
        { }

        public void SpawnUnitsAtPoints(List<Transform> newSpawnPoints)
        { }

        public void ActivateAttack()
        {
            if (GlobalConfig.TowersNoAttack)
                return;
            StopAttack();
            SetStats();
            _processing = StartCoroutine(AttackTargetsSeeking());   
        }

        public void StopAttack()
        {
            if(_processing != null)
                StopCoroutine(_processing);
        }

        public void KillUnits()
        {
            StopAllCoroutines();
            _isShooting = false;
        }

        public void TakeUnit(BoatUnitController partUnit)
        {
            partUnit.gameObject.SetActive(false);
        }

        public void UnloadFromStash()
        { }
        
        private void StopShooting()
        {
            if(_shooting != null)
                StopCoroutine(_shooting);
            _isShooting = false;
        }
        
        private void ShootAt(Vector3 pos, ITarget target)
        {
            StopShooting();
            _shooting = StartCoroutine(AttackingTarget(pos, target));
        }

        private void SetStats()
        {
            foreach (var shooter in _shooters)
            {
                shooter.Damage = _settings.unitDamage;
                shooter.Speed = GlobalConfig.ArrowSpeed;
                shooter.Team = Tower.Team;
            }
        }
        
        private IEnumerator AttackTargetsSeeking()
        {
            var center = transform.position.XZPlane();
            while (true)
            {
                while (Tower.Level == 0)
                    yield return null;
                for(var i = 0; i < GlobalConfig.CastSkippedFrames; i++)
                    yield return null;
                if (BoatUtils.GetFirstTarget(Tower.Team, center, Radius, out var target, out var isTower) == false)
                    continue;
                if (!_isShooting)
                {
                    _isShooting = true;
                    _targetIsTower = isTower;
                    _currentTarget = target;
                    ShootAt(center, target);
                }
                else
                {
                    if (!_targetIsTower && isTower)
                    {
                        _targetIsTower = true;
                        _currentTarget = target;
                        ShootAt(center,target);
                    }
                }
            }
        }

        private IEnumerator AttackingTarget(Vector3 centerPlanePos, ITarget target)
        {
            var doShoot = true;
            var rad2 = Mathf.Pow(Radius + AttackRadiusAdded, 2);
            Vector3 targetPos;
            // Checking distance and if target is alive 
            var damageable = target.Damageable;
            while (doShoot)
            {
                targetPos = target.Point.position.XZPlane();
                var d2 = (centerPlanePos - targetPos).sqrMagnitude;
                if (d2 > rad2 || !damageable.CanDamage)
                {
                    OnShootingStopped();
                    yield break;
                }
                else
                {
                    for (var i = 0; i < Tower.Level; i++)
                        _shooters[i].Fire(target);
                }
                doShoot &= damageable.CanDamage;
                yield return new WaitForSeconds(.5f / (_settings.fireRate));
                yield return null;
            }
            OnShootingStopped();
        }
        
        private void OnShootingStopped()
        {
            _targetIsTower = false;
            _isShooting = false;
        }
    }
}