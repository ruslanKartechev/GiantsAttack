using System;
using SleepDev;
using SleepDev.Sound;
using SleepDev.UIUtils;
using UnityEngine;

namespace GameCore.UI
{
    public class MenuStart : MonoBehaviour, IMenuStart
    {
        [SerializeField] private PopAnimator _popAnimator;
        [SerializeField] private ProperButton _playButton;
        [SerializeField] private SoundSo _clickSound;
        private Action _callback;
        
        public GameObject Go => gameObject;
        
        private void OnEnable()
        {
            _playButton.OnDown += OnPlay;
        }

        private void OnDisable()
        {
            _playButton.OnDown -= OnPlay;
        }

        public void On()
        {
            gameObject.SetActive(true);
        }

        public void Off()
        {
            gameObject.SetActive(false);
        }

        public void Show(Action onDone)
        {
            _playButton.interactable = false;
            _popAnimator.ZeroAndPlay(() =>
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
            _clickSound.Play();
            _callback?.Invoke();
        }
        
    }
}