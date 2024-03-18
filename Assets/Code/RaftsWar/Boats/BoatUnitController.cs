using System.Collections;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class BoatUnitController : MonoBehaviour
    {
       [SerializeField] private TowerUnit _unit;
       
       public float Radius { get; set; } = 1;
       public TowerUnit Unit => _unit;

        public void Activate()
        {
            if (GlobalConfig.BoatUnitsNoAttack)
                return;
            StartCoroutine(Working());
        }

        public void Stop()
        {
            StopAllCoroutines();
            _unit.Idle();
        }

        private IEnumerator Working()
        {
            var radius = Radius;
            var r2 = radius * radius;
            var tr = transform;
            var isShooting = false;
            ITarget currentTarget = null;
            while (true)
            {
                if (isShooting)
                {
                    var vec = (currentTarget.Point.position- tr.position).XZPlane();
                    var d2 = vec.sqrMagnitude;
                    if (d2 >= r2 || currentTarget.Damageable.IsDead)
                    {
                        isShooting = false;
                        currentTarget = null;
                        _unit.Idle();
                    }
                    else
                    {
                        tr.rotation = Quaternion.LookRotation(vec);
                    }
                    yield return null;
                }
                else
                {
                    if(BoatUtils.GetFirstTarget(_unit.Team, transform.position, radius, out var target, out var isTower))
                    {
                        currentTarget = target;
                        isShooting = true;
                        _unit.Fire(currentTarget);
                        // CLog.LogRed($"Found target: {target.Point.gameObject.name}, IS Tower: {isTower}");
                    }
                }
                yield return null;
            }
        }
    }
}