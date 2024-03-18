using System;
using SleepDev;
using SleepDev.UIUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RaftsWar.UI
{
    public class MenuStart : MonoBehaviour, IMenuStart
    {
        [SerializeField] private PopAnimator _popAnimator;
        [SerializeField] private Button _playButton;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private MoneyUI _moneyUI;
        private Action _callback;
        
        public GameObject Go => gameObject;
        private void OnEnable()
        {
            _playButton.onClick.AddListener(OnPlay);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(OnPlay);
        }

        public void On()
        {
            UpdateStats();
            gameObject.SetActive(true);
        }

        public void Off()
        {
            gameObject.SetActive(false);
        }

        public void Show(Action onDone)
        {
            UpdateStats();
            _playButton.interactable = false;
            _popAnimator.HideAndPlay(() =>
            {
                ActivateButtons();
                onDone?.Invoke();
            });
        }

        public void Hide(Action onDone)
        {
            Off();
            onDone?.Invoke();
        }

        public void Show(Action playCallback, Action onShown)
        {
            _callback = playCallback;
            Show(onShown);
        }
        
        private void ActivateButtons()
        {
            _playButton.interactable = true;
        }
        
        private void OnPlay()
        {
            _callback?.Invoke();
        }
        
        private void UpdateStats()
        {
            var level = GCon.PlayerData.LevelTotal + 1;
            _levelText.text = $"LEVEL {level}";
        }
    }
}