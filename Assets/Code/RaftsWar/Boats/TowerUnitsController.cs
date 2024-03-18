
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SleepDev;
using SleepDev.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace RaftsWar.Boats
{
    public class TowerUnitsController : MonoBehaviour, ITowerUnitsController
    {
        private const float AttackRadiusAdded = 1f;
        
        private TowerLevelSettings _settings;
        private Coroutine _processing;
        private Coroutine _shooting;
        private bool _targetIsTower;
        private bool _isShooting;
        private ITarget _currentTarget;
        private List<TowerUnit> _stash = new List<TowerUnit>(5);

        public List<Transform> SpawnPoints { get; private set; } = new List<Transform>(10);
        public List<Transform> OptionalPoints { get; private set; }
        public List<TowerUnit> Units { get; private set; } = new List<TowerUnit>(25);
        public ITower Tower { get; set; }
        public float Radius { get; set; }

        public void UnloadFromStash()
        {
            foreach (var unit in _stash)
            {
                if (OptionalPoints.Count == 0)
                {
                    CLog.Log($"[UnitsController] no more optional points");
                    return;
                }
                var spawnPoint = OptionalPoints.Random();
                OptionalPoints.Remove(spawnPoint);
                unit.gameObject.SetActive(true);
                unit.transform.SetParentAndCopy(spawnPoint);
                Units.Add(unit);
                if (_isShooting)
                {
                    unit.LookAt(_currentTarget.Point, -1);
                    unit.Fire(_currentTarget);
                }
            }
            _stash.Clear();
        }
        
        public void TakeUnit(BoatUnitController partUnit)
        {
            if (OptionalPoints.Count == 0)
            {
                CLog.Log($"[UnitsController] No points. Taking to stash");
                partUnit.Stop();
                partUnit.gameObject.SetActive(false);
                partUnit.transform.parent = transform;
                _stash.Add(partUnit.Unit);
                return;
            }
            var spawnPoint = GameUtils.FindClosestTo(OptionalPoints, partUnit.transform.position);
            if (spawnPoint == null)
            {
                CLog.LogRed($"[UnitsController] No available spawn point, hiding the unit");
                partUnit.Unit.Hide();
                return;
            }
            OptionalPoints.Remove(spawnPoint);
            MoveNewUnitToPoint(partUnit.Unit, spawnPoint);
        }

        private void MoveNewUnitToPoint(TowerUnit unit, Transform spawnPoint)
        {
            const float time = .36f;
            const float jump = 8f;
            const float scale = .2f;
            var tr = unit.transform;
            tr.parent = spawnPoint;
            tr.DOJump(spawnPoint.position, jump, 1, time).SetEase(Ease.OutQuad);
            tr.DORotate(spawnPoint.rotation.eulerAngles, time);
            tr.DOPunchScale(Vector3.one * scale, time);
            Units.Add(unit);
        }

        public void UpdateSettings(TowerLevelSettings settings)
        {
            _settings = settings;
            Radius = settings.radius;
        }

        public void SetOptionalPoints(List<Transform> optionalSpawnPoints)
        {
            OptionalPoints = optionalSpawnPoints;
        }

        public void SpawnUnitsAtPoints(List<Transform> points)
        {
            for (var spawnPointIndex = 0; spawnPointIndex < points.Count; spawnPointIndex++)
            {
                var p = points[spawnPointIndex];
                var instance = GCon.GOFactory.Spawn(GlobalConfig.ArcherUnitID);
                instance.transform.SetParentAndCopy(p);
                var unit = instance.GetComponent<TowerUnit>();
                unit.Idle();
                unit.Init(Tower.Team);
                unit.Animate();
                Units.Add(unit);
            }
            SpawnPoints = points;
        }

        public void StopAndClearSpawnedUnits()
        {
            StopAttack();
            StopShooting();
            foreach (var unit in Units)
                unit.Hide();
            Units.Clear();
        }

        public void ActivateAttack()
        {
            if (GlobalConfig.TowersNoAttack)
                return;
            StopAttack();
            StopShooting();
            _processing = StartCoroutine(AttackTargetsSeeking());
        }

        public void StopAttack()
        {
            if(_processing != null)
                StopCoroutine(_processing);
            foreach (var unit in Units)
                unit.Idle();
        }

        public void KillUnits()
        {
            StopAttack();
            foreach (var unit in Units)
                unit.Kill();
        }

        private void SetUnitsStats()
        {
            foreach (var unit in Units)
            {
                unit.FireRate = _settings.fireRate;
                unit.Damage = _settings.unitDamage;
            }
        }

        private void StopShooting()
        {
            if(_shooting != null)
                StopCoroutine(_shooting);
            _isShooting = false;
        }

        private void ShootAt(Vector3 pos, ITarget target)
        {
            SetUnitsStats();
            StopShooting();
            _isShooting = true;
            _shooting = StartCoroutine(AttackingTarget(pos, target));
        }

        private void OnShootingStopped()
        {
            _targetIsTower = false;
            _isShooting = false;
            foreach (var unit in Units)
                unit.Idle();
        }
        
        private IEnumerator AttackTargetsSeeking()
        {
            var center = transform.position.XZPlane();
            while (true)
            {
                while (Units.Count == 0)
                    yield return null;
                for(var i = 0; i < GlobalConfig.CastSkippedFrames; i++)
                    yield return null;
                if (BoatUtils.GetFirstTarget(Tower.Team, center, Radius, out var target, out var isTower) == false)
                    continue;
                if (!_isShooting)
                {
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
            // Initial Rotation
            var rotTime = GlobalConfig.TowerUnitsRotTime;
            foreach (var unit in Units)
            {
                unit.gameObject.SetActive(true);
                unit.LookAt(target.Point, rotTime);
            }
            yield return new WaitForSeconds(rotTime);
            // Call firing animations
            foreach (var unit in Units)
                unit.Fire(target);
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
                doShoot &= damageable.IsAlive;
                yield return null;
            }
            OnShootingStopped();
        }
    }
}