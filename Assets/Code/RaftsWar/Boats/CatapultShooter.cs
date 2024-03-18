using System.Collections;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class CatapultShooter : MonoBehaviour
    {
        private CatapultView _view;
        private Team _team;
        private ITarget _currentTarget;
        private CatapultMagazine _magazine;
        private bool _canTake;
        private Vector3 _currentTargetPos;
        private CatapultSettings _settings;
        
        public void Init(CatapultView view, Team team, CatapultMagazine magazine)
        {
            CLog.LogWhite($"[CatapultShooter] Created");
            view.Animator.SetShootSpeed(team.CatapultSettings.shootAnimationSpeed);
            view.Animator.Callback = LaunchProjectile;
            _settings = team.CatapultSettings;
            _view = view;
            _team = team;
            _magazine = magazine;
            view.Animator.Idle();
        }

        public void Activate()
        {
            CLog.LogWhite($"[CatapultShooter] Activated");
            _canTake = true;
            _view.Animator.Idle();
            StopAllCoroutines();
            StartCoroutine(Shooting());
        }

        private Vector3 GetDebugTarget()
        {
            return transform.position + (-transform.forward + transform.right) * 20f;
        }

        private void LaunchProjectile()
        {
#if UNITY_EDITOR
            if (_currentTarget == null)
            {
                CLog.LogRed($"Trying to shoot catapult when target == null");
                return;
            }
#endif
            // var pr = CatapultProjectilePool.Inst.GetObject();
            if (_magazine.HasProjectiles() == false)
                return;
            var pr = _magazine.GetProjectile();
            pr.Init(_team);
            pr.Launch(_view.ShootFrom, _currentTarget, 
                _settings.projectileSettings.speed, _settings.projectileSettings.damage);
        }

        private IEnumerator Shooting()
        {
            while (true)
            {
                var target = GetClosestTarget();
                if (target != null && _magazine.HasProjectiles())
                {
                    _currentTarget = target;
                    yield return Rotating(target.Point.position);
                    yield return AttackingTarget(target);
                }
                yield return new WaitForSeconds(.5f);
            }
        }

        private IEnumerator AttackingTarget(ITarget target)
        {
            _view.Animator.Shoot();
            while (true)
            {
                if (target.Damageable.IsDead || _magazine.HasProjectiles() == false)
                {
                    _view.Animator.Idle();
                    yield break;
                }
                yield return null;
            }
        }
        
        private ITarget GetClosestTarget()
        {
            var minD2 = float.MaxValue;
            ITarget res = null;
            foreach (var target in TeamsTargetsManager.Inst.Targets)
            {
                if(target.Team == _team)
                    continue;
                var d2 = (target.Point.position - transform.position).sqrMagnitude;
                if (d2 < minD2)
                {
                    minD2 = d2;
                    res = target;
                }
            }
// #if UNITY_EDITOR
//             if (res == null)
//                 CLog.LogRed($"Catapult target is null...");
// #endif
            return res;
        }
        
        private IEnumerator Rotating(Vector3 endPoint)
        {
            var tr = _view.Orientation;
            var lookVec = (endPoint - tr.position).XZPlane();
            var rot2 = Quaternion.LookRotation(lookVec);
            var rot1 = tr.rotation;
            var elapsed = Time.deltaTime;
            var time = .22f;
            var t = elapsed / time;
            while (t <= 1f)
            {
                tr.rotation = Quaternion.Lerp(rot1, rot2, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
        }
        
    }
}