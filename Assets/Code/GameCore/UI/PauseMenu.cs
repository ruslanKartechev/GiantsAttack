using System;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI
{
    public class PauseMenu : MonoBehaviour, IUIScreen
    {
        [SerializeField] private VibrationButton _vibration;
        [SerializeField] private SoundButton _sound;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _termsButton;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private SoundSo _clickSound;

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
            _termsButton.onClick.AddListener(Terms);
            _sound.OnEnable();
            _vibration.OnEnable();
        }
        
        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(Close);
            _nextButton.onClick.RemoveListener(SkipLevel);
            _restartButton.onClick.RemoveListener(Restart);
            _termsButton.onClick.RemoveListener(Terms);
            _sound.OnDisable();
            _vibration.OnDisable();
        }

        private void Restart()
        {
            CLog.Log($"[PauseMenu] restart level");
            _clickSound.Play();
            Close();
            LevelUtils.CallReplay();
        }

        private void Close()
        {
            _clickSound.Play();
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }

        private void SkipLevel()
        {
            CLog.Log($"[PauseMenu] skip level");
            _clickSound.Play();
            Close();
            LevelUtils.CallNextLevel();
        }

        private void Terms()
        {
            CLog.Log($"[PauseMenu] term button");
            _clickSound.Play();
        }
        

    }
}