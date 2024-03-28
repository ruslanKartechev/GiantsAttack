using System;
using SleepDev.UIUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI
{
    public class MenuFail : MonoBehaviour, IMenuFail
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
            _onPlayCallback = buttonCallback;
            _levelUI.text = $"LEVEL {level}";
            Show(onDone);
        }

        public void Show(Action onDone)
        {
            On();
            _playButton.interactable = false;
            _popAnimator.HideAndPlay(() =>
            {
                _playButton.interactable = true;
                onDone?.Invoke();
            });
        }

        public void Hide(Action onDone)
        { }
        
        private void OnButtonClick()
        {
            _onPlayCallback?.Invoke();
        }
    
    
#if UNITY_EDITOR
        [ContextMenu("Debug Show")]
        public void E_Show()
        {
            Show(1,() => {}, () => {});
        }
#endif
    }
}