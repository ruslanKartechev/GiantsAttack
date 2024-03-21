using System;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;
using GCon = GameCore.Core.GCon;

namespace GameCore.UI
{
    public class MenuGameplay : MonoBehaviour, IGameplayMenu
    {
        [SerializeField] private JoystickUI _joystickUI;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private UIDamagedEffect _damagedEffect;
        public JoystickUI JoystickUI => _joystickUI;
        public IUIDamagedEffect DamagedEffect => _damagedEffect;

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
            On();
            onDone?.Invoke();
        }

        public void Hide(Action onDone)
        {
            Off();
            onDone?.Invoke();
        }

        public GameObject Go => gameObject;
        
        private void ShowPause()
        {
            var pause = GCon.UIFactory.GetPauseUI();
            pause.Show(() => {});
        }
        
        private void OnEnable()
        {
            _pauseButton.onClick.AddListener(ShowPause);
        }
        private void OnDisable()
        {
            _pauseButton.onClick.RemoveListener(ShowPause);
        }
    }
}