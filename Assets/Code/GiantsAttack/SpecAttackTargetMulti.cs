using System.Collections.Generic;
using UnityEngine;

namespace GiantsAttack
{
    public class SpecAttackTargetMulti : MonoBehaviour, ISpecAttackTarget
    {
        [SerializeField] private List<GameObject> _targetGos;
        private List<ISpecAttackTarget> _targets;

        private void Awake()
        {
            _targets = new List<ISpecAttackTarget>(_targetGos.Count);
            foreach (var go in _targetGos)
            {
                var tt = go.GetComponent<ISpecAttackTarget>();
                if (tt == null)
                {
                    Debug.LogError($"tt is NULLLLLL {go.gameObject.name}");
                }
                _targets.Add(tt);
            }
        }

        public void OnStageBegan()
        {
            foreach (var t in _targets)
                t.OnStageBegan();
        }

        public void OnAttackBegan()
        {
            foreach (var t in _targets)
                t.OnAttackBegan();
        }

        public void OnAttack()
        {
            foreach (var t in _targets)
                t.OnAttack();
        }

        public void OnCompleted()
        {
            foreach (var t in _targets)
                t.OnCompleted();
        }
    }
}