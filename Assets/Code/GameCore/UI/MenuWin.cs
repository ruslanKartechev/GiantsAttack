using System;
using GameCore.Core;
using SleepDev.UIUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI
{
    public class MenuWin : MonoBehaviour, IMenuWin
    {
        [SerializeField] private PopAnimator _popAnimator;
        [SerializeField] private Button _playButton;
        [SerializeField] private TextMeshProUGUI _levelUI;
        private Action _onPlayCallback;
        
        public GameObject Go => gameObject;

        private void OnEnable()
        {
            _playButton.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(OnButtonClick);
        }

        public void On()
        {
            gameObject.SetActive(true);
        }

        public void Off()
        {
            gameObject.SetActive(false);
        }

        public void Show(int level, Action buttonCallback, Action onDone)
        {
            _levelUI.text = $"LEVEL {level}";
            _onPlayCallback = buttonCallback;
            _playButton.interactable = false;
            Show(onDone);
        }
        
        public void Show(Action onDone)
        {
            On();
            _popAnimator.HideAndPlay(() =>
            {
                OnShowAnimated();
                onDone?.Invoke();
            });
        }

        public void Hide(Action onDone)
        { }
        
        private void OnButtonClick()
        {
            _onPlayCallback?.Invoke();
        }

        private void OnShowAnimated()
        {
            _playButton.interactable = true;
        }

#if UNITY_EDITOR
        [ContextMenu("Debug Show")]
        public void E_Show()
        {
            Show(1, () => { }, () => { });
        }
#endif
    }
}