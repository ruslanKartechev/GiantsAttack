using System.Collections;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class SpinAnimator : MonoBehaviour
    {
        [SerializeField] private int _spins;
        [SerializeField] private float _spinTime;
        [SerializeField] private Transform _rotatable;

        public int Spins
        {
            get => _spins;
            set => _spins = value;
        }

        public float SpinTime
        {
            get => _spinTime;
            set => _spinTime = value;
        }


        public void Stop()
        {
            StopAllCoroutines();
        }
        
        public void Spin()
        {
            StartCoroutine(Rotating());
        }

        private IEnumerator Rotating()
        {
            var elapsed = Time.deltaTime;
            var spins = _spins;
            var time = _spinTime;
            var t = elapsed / time;
            var tr = _rotatable;
            var angles = tr.localEulerAngles;
            for (var i = 0; i < spins; i++)
            {
                while (t <= 1f)
                {
                    var angle = Mathf.Lerp(0f, 360f, t);
                    angles.y = angle;
                    tr.localEulerAngles = angles;
                    elapsed += Time.deltaTime;
                    t = elapsed / time;
                    // Debug.Log($"Angle {angle}, elapsed {elapsed}, t {t}");
                    yield return null;
                }   
                t = elapsed = 0f;
            }
            angles.y = 0f;
            tr.localEulerAngles = angles;
        }        
    }
}