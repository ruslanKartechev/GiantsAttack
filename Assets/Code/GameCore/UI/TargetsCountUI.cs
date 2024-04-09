using DG.Tweening;
using SleepDev;
using TMPro;
using UnityEngine;

namespace GameCore.UI
{
    public class TargetsCountUI : MonoBehaviour, ITargetsCountUI
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Transform _scaleTarget;
        [SerializeField] private float _scaleTime;
        [SerializeField] private float _scaleMagn;
        private int _max;

        public void Show(bool show)
        {
            gameObject.SetActive(show);
        }

        public void SetCount(int max, int current)
        {
            CLog.Log($"SetCount {max}/{current}");
            _max = current;
            _text.text = $"{current}/{max}";
        }

        public void UpdateCount(int current)
        {
            _text.text = $"{current}/{_max}";
            _scaleTarget.DOKill();
            _scaleTarget.DOPunchScale(Vector3.one * _scaleMagn, _scaleTime);
        }
    }
}