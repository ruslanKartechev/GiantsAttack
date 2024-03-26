using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace GameCore.UI
{
    public class ShootAtTargetUI : MonoBehaviour, IShootAtTargetUI
    {
        [SerializeField] private float _scalingTime;
        [SerializeField] private float _scale;
        [SerializeField] private RectTransform _movable;
        [SerializeField] private GameObject _block;
        private Coroutine _working;
        private Sequence _scalingSequence;
        
        public void ShowAndFollow(Transform target)
        {
            _block.SetActive(true);
            _movable.localScale = Vector3.one;
            _scalingSequence = DOTween.Sequence();
            _scalingSequence.Append(_movable.DOScale(Vector3.one * _scale, _scalingTime));
            _scalingSequence.Append(_movable.DOScale(Vector3.one, _scalingTime));
            _scalingSequence.SetLoops(-1);
            _working = StartCoroutine(Tracking(target));
        }

        public void Hide()
        {
            _block.SetActive(false);
            if(_scalingSequence != null)
                _scalingSequence.Kill();
            _movable.localScale = Vector3.one;
            if(_working != null)
                StopCoroutine(_working);
        }
        
        private IEnumerator Tracking(Transform target)
        {
            var cam = Camera.main;
            while (true)
            {
                _movable.position = cam.WorldToScreenPoint(target.position);
                yield return null;
            }
        }

     
    }
}