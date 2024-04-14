using SleepDev.Vibration;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI
{
    [System.Serializable]
    public class VibrationButton
    {
        [SerializeField] private Button _btn;
        [SerializeField] private GameObject _offIcon;
        
        public void OnEnable()
        {
            _btn.onClick.AddListener(OnClick);
            _offIcon.gameObject.SetActive(!VibrationManager.VibrManager.IsOn);
        }

        public void OnDisable()
        {
            _btn.onClick.RemoveListener(OnClick);
        }
        
        private void OnClick()
        {
            VibrationManager.VibrManager.SetStatus(!VibrationManager.VibrManager.IsOn);
            _offIcon.gameObject.SetActive(!VibrationManager.VibrManager.IsOn);
        }
    }
}