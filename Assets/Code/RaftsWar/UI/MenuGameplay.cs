using System;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace RaftsWar.UI
{
    public class MenuGameplay : MonoBehaviour, IGameplayMenu
    {
        [SerializeField] private ProperButton _inputButton;
        [SerializeField] private JoystickUI _joystickUI;
        [SerializeField] private NamesUIManager _namesUIManager;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private UIDamagedEffect _damagedEffect;
        public ProperButton InputButton => _inputButton;
        public JoystickUI JoystickUI => _joystickUI;
        public NamesUIManager NamesUIManager => _namesUIManager;
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