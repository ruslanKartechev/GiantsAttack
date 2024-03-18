using System;
using System.Collections;
using System.Collections.Generic;
using RaftsWar.UI;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class CreosEnemyBoat : Boat, ITeamPlayer
    {
        [SerializeField] private GameObject _captainGo;
        [SerializeField] private BoatHealth _health;
        [SerializeField] private EnemyNavAgent _agent;
        private Coroutine _moving;
        private Coroutine _movingToPoint;
        private BoatSettings _boatSettings;
        private IBoatCaptain _captain;
        private IBoatDeathEffect _boatDeathEffect;
        private bool _isDead;
        private List<Transform> _roamingPoints;

        public void Init(Team team, List<Transform> roamingPoints)
        {
            _roamingPoints = roamingPoints;
            _captain = _captainGo.GetComponent<IBoatCaptain>();
            _boatSettings = team.BoatSettings;
            InitBoat(_boatSettings, team, _health);
            _health.Init(_settings.health);
            _health.CanDamage = true;
            _health.OnDied += (t) => {Kill();};
            _rootPart.Damageable = _health;
            _canConnect = false;
            _captain.SetView(team.UnitsView);
        }
        
        public event Action<ITeamPlayer> OnDied;
        public bool IsDead => _isDead;
        public ITarget Target => _rootPart;
        public Transform DeadCameraPoint { get; }
        public void StopPlayer()
        {
            
        }

        public void ActivatePlayer()
        {
            CLog.LogGreen($"[{gameObject.name}] Boat activated");
            TeamsTargetsManager.Inst.AddPlayer(this);
            StopAllCoroutines();
            StartCoroutine(Roaming());
        }

        public void Kill()
        {
            _health.CanDamage = false;
            _isDead = true;
            StopAllCoroutines();
            _boatDeathEffect = gameObject.GetComponent<IBoatDeathEffect>();
            _boatDeathEffect.Captain = _captain;
            _boatDeathEffect.Boat = this;
            _boatDeathEffect.Die();
        }

        public void SetTeamUnitUI(ITeamUnitUI ui)
        {
        }

        private IEnumerator Roaming()
        {
            var index = 0;
            while (true)
            {
                var point = _roamingPoints[index];
                yield return MovingToPoint(point);
                index++;
                if(index >= _roamingPoints.Count)
                    index = 0;
            }
        }

        public float Speed => _settings.moveSpeed;

        private IEnumerator MovingToPoint(Transform point)
        {
            var index = 0;
            var tr = transform;
            while (true)
            {
                var vec = point.position - tr.position;
                var magn = vec.magnitude;
                vec /= magn;
                _rb.velocity = vec * Speed;
                _captain.Rotate(vec);
                if (magn < .2f)
                {
                    yield break;
                }
                yield return null;
            }
        }

   
    }
}