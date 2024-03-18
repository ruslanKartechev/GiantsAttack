using DG.Tweening;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class TowerAnimator : MonoBehaviour
    {
        [SerializeField] private Transform _scalable;
        [SerializeField] private float _punchScale;
        [SerializeField] private float _scaleTime;
        private bool _isAnimating;
        
        public void Animate()
        {
            // if (_isAnimating)
            //     return;
            // _isAnimating = true;
            _scalable.DOKill();
            _scalable.localScale = Vector3.one;
            _scalable.DOPunchScale( Vector3.one * _punchScale, _scaleTime);
        }
        
    }
}