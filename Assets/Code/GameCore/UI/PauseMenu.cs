using System;
using GameCore.Levels;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI
{
    public class PauseMenu : MonoBehaviour, IUIScreen
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _soundButton;
        [SerializeField] private Button _termsButton;
        [SerializeField] private Canvas _canvas;
        
        public void On()
        { }

        public void Off()
        { }

        public void Show(Action onDone)
        {
            gameObject.SetActive(true);
            _canvas.enabled = true;
            Time.timeScale = 0;
        }

        public void Hide(Action onDone)
        {
            Close();
        }

        public GameObject Go => gameObject;


        private void OnEnable()
        {
            _closeButton.onClick.AddListener(Close);
            _nextButton.onClick.AddListener(SkipLevel);
            _restartButton.onClick.AddListener(Restart);
            _soundButton.onClick.AddListener(Sound);
            _termsButton.onClick.AddListener(Terms);
        }
        
        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(Close);
            _nextButton.onClick.RemoveListener(SkipLevel);
            _restartButton.onClick.RemoveListener(Restart);
            _soundButton.onClick.RemoveListener(Sound);
            _termsButton.onClick.RemoveListener(Terms);
        }

        private void Restart()
        {
            CLog.Log($"[PauseMenu] restart level");
            Close();
            LevelUtils.CallReplay();
        }

        private void Close()
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }

        private void SkipLevel()
        {
            CLog.Log($"[PauseMenu] skip level");
            Close();
            LevelUtils.CallNextLevel();
        }

        private void Terms()
        {
            CLog.Log($"[PauseMenu] term button");
        }

        private void Sound()
        {
            CLog.Log($"[PauseMenu] sound button");
        }

    }
}