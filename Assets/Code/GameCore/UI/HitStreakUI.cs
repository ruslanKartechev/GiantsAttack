using DG.Tweening;
using TMPro;
using UnityEngine;

namespace GameCore.UI
{
    public class HitStreakUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private float _maxAngle;
        private bool _hiding;
        
        private const float HideTime = .33f;

        public void Show(int count)
        {
            if (_hiding)
            {
                _hiding = false;
                _text.gameObject.SetActive(true);
                _text.transform.DOKill();
                _text.transform.localScale = Vector3.one;
            }
            _text.text = $"x{count}";
            var aa = _text.transform.localEulerAngles;
            aa.z = UnityEngine.Random.Range(-_maxAngle, _maxAngle);
            _text.transform.localEulerAngles = aa;
        }

        public void Hide()
        {
            _hiding = true;
            _text.transform.DOScale(Vector3.zero, HideTime).OnComplete(() =>
            {
                _text.gameObject.SetActive(false);
            });
        }
    }
}