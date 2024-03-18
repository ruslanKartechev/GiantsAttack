using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using SleepDev;

namespace RaftsWar.Boats
{
    public class TowerUnit_Archer : TowerUnit
    {
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int ShootHash = Animator.StringToHash("Shoot");

        [SerializeField] private Animator _animator;
        [SerializeField] private List<Renderer> _renderers;
        [SerializeField] private ArcherArrowLauncher _arrowLauncher;
        [SerializeField] private UnitKilledEffect _killedEffect;
        
        private Coroutine _lookingAt;
        private float _fireSpeed;

        public override float FireRate
        {
            get => _fireSpeed;
            set
            {
                _fireSpeed = value;
                _animator.SetFloat("ShootingSpeed", _fireSpeed);
            }
        }

        public override float Damage
        {
            get => _arrowLauncher.Damage;
            set => _arrowLauncher.Damage = value;
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void Kill()
        {
            StopLookAt();
            _killedEffect.PlayKilled();
        }

        public override void Init(Team team)
        {
            Team = team;
            _viewSettings = team.UnitsView;
            _arrowLauncher.ViewSettings = _viewSettings;
            for (var i = 0; i < _renderers.Count; i++)
                _viewSettings.unitMaterials[i].Set(_renderers[i]);
            _arrowLauncher.Team = team;
        }

        public override void SetView(UnitViewSettings unitViewSettings)
        {
            _viewSettings = unitViewSettings;
            for (var i = 0; i < _renderers.Count; i++)
                _viewSettings.unitMaterials[i].Set(_renderers[i]);
        }


        public override void Animate()
        {
            var tr = transform;
            tr.DOKill();
            var pos = tr.localPosition;
            tr.localPosition += Vector3.up * 5f;
            tr.localScale = Vector3.zero;
            tr.DOLocalMove(pos, .5f);
            tr.DOScale(Vector3.one, .5f);
        }
        
        public override void LookAt(Transform at, float time)
        {
            StopLookAt();
            if (time <= 0)
            {
                transform.rotation = Quaternion.LookRotation(at.position.XZPlane() - transform.position.XZPlane());
                return;
            }
            _lookingAt = StartCoroutine(Rotating(at, time));
        }
        
        public override void Idle()
        {
            // CLog.Log("[Archer] idle");
            StopLookAt();
            _animator.SetTrigger(IdleHash);    
        }

        public override void Fire(ITarget target)
        {
            // CLog.Log("[Archer] fire");
            _arrowLauncher.CurrentTarget = target;
            _animator.ResetTrigger(IdleHash);
            _animator.Play(ShootHash);
        }
        
        private void StopLookAt()
        {
            if(_lookingAt != null)
                StopCoroutine(_lookingAt);
        }

        private IEnumerator Rotating(Transform at, float time)
        {
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            var tr = transform;
            var rot1 = tr.rotation;
            while (t <= 1f)
            {
                var rot2 = Quaternion.LookRotation(
                    at.position.XZPlane() - tr.position.XZPlane());
                tr.rotation = Quaternion.Lerp(rot1, rot2, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            while (true)
            {
                tr.rotation = Quaternion.LookRotation(at.position.XZPlane() - tr.position.XZPlane());
                yield return null;
            }
        }
    }
}