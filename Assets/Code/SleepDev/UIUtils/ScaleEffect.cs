using DG.Tweening;
using UnityEngine;

namespace SleepDev.UIUtils
{
    public class ScaleEffect : ButtonClickEffect
    {
        [SerializeField] private Vector3 _punchScale;
        [SerializeField] private float _punchTime = .1f;
        
        
        public override void Play()
        {
            _target.localScale = Vector3.one;
            _target.DOPunchScale(_punchScale, _punchTime);
        }
    }
}