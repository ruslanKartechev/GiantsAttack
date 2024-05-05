using GameCore.Core;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI
{
    
    [System.Serializable]
    public class SoundButton
    {
        [SerializeField] private Button _btn;
        [SerializeField] private GameObject _offIcon;
        
        public void OnEnable()
        {
            _btn.onClick.AddListener(OnClick);
            _offIcon.gameObject.SetActive(!SoundContainer.SoundManager.IsOn);
        }

        public void OnDisable()
        {
            _btn.onClick.RemoveListener(OnClick);
        }
        
        private void OnClick()
        {
            SoundContainer.SoundManager.SetStatus(!SoundContainer.SoundManager.IsOn);
            _offIcon.gameObject.SetActive(!SoundContainer.SoundManager.IsOn);
            GCon.PlayerData.SoundStatus = SoundContainer.SoundManager.IsOn;
        }
    }
}