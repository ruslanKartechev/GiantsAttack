using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RaftsWar.UI
{
    public class NameUI : MonoBehaviour, ITeamUnitUI
    {
        private const float DieTime = .25f;
        
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private Image _deadIcon;
        [SerializeField] private Image _heartIcon;
        
        private int _currentValue;
        private Coroutine _healthChange;
        
        public Color DeadColor { get; set; }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Die()
        {
            StopHealthChange();
            _nameText.DOColor(DeadColor, DieTime);
            _healthText.gameObject.SetActive(false);
            _heartIcon.enabled = false;
            _deadIcon.enabled = true;
            _deadIcon.transform.DOPunchScale(Vector3.one * .2f, .25f);
            var text = _nameText.text;
            text = $"<s>{text}</s>";
            _nameText.text = text;
        }

        public void SetName(string name, Color color)
        {
            _nameText.color = color;
            _healthText.color = color;
            _nameText.text = name;
        }

        public void SetHealth(float health)
        {
            _currentValue = (int)health;
            _healthText.text = $"{_currentValue}";
        }

        public void UpdateHealth(float health)
        {
            StopHealthChange();
            _healthChange = StartCoroutine(ChangingHealth(_currentValue, (int)health));
            _currentValue = (int)health;
        }

        private void StopHealthChange()
        {
            if(_healthChange != null)
                StopCoroutine(_healthChange);
        }
        
        private IEnumerator ChangingHealth(int val1, int val2)
        {
            var time = .25f;
            var elapsed = 0f;
            var t = elapsed / time;
            while (t <= 1f)
            {
                _healthText.text = $"{(int)Mathf.Lerp(val1, val2, t)}";
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            _healthText.text = $"{val2}";
        }
        
    }
}