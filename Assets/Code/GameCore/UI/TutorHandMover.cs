using System.Collections;
using UnityEngine;

namespace GameCore.UI
{
    public class TutorHandMover : MonoBehaviour
    {
        [SerializeField] private RectTransform _hand;
        [SerializeField] private GameObject _go;
        [SerializeField] private Vector2 _lengthLimits;
        [SerializeField] private float _handMaxY;
        [SerializeField] private float _movePeriod;
        [SerializeField] private AnimationCurve _heightCurve;
        private Coroutine _handAnimating;

        public void ShowHandAnimation()
        {
            _go.SetActive(true);
            _handAnimating = StartCoroutine(HandAnimating());
        }

        public void HideHand()
        {
            _go.SetActive(false);
            if(_handAnimating != null)
                StopCoroutine(_handAnimating);
        }

        private IEnumerator HandAnimating()
        {
            while (true)
            {
                yield return MovingHand(_lengthLimits.x, _lengthLimits.y, _handMaxY);
                yield return MovingHand(_lengthLimits.y, _lengthLimits.x, _handMaxY);        
            }
        }
        
        private IEnumerator MovingHand(float xFrom, float xTo, float yMax)
        {
            var elapsed = 0f;
            var time = _movePeriod;
            var t = 0f;
            while (elapsed < time)
            {
                t = elapsed / time;
                // var w = Mathf.Lerp(0f, 2 * Mathf.PI, t);
                // var y = Mathf.Sin(w) * yMax;
                var y = _heightCurve.Evaluate(t) * yMax;
                var pos = new Vector3(Mathf.Lerp(xFrom, xTo, t), y, 0f);
                _hand.anchoredPosition = pos;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            _hand.position = new Vector3(xTo, 0f, 0f);
        }
    }
}