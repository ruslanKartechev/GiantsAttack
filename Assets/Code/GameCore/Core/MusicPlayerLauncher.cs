using SleepDev.Sound;
using UnityEngine;

namespace GameCore.Core
{
    [DefaultExecutionOrder(200)]
    public class MusicPlayerLauncher : MonoBehaviour
    {
        [SerializeField] private BootSettings _bootSettings;   
        [SerializeField] private SimpleMusicPlayer _musicPlayer;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_musicPlayer == null)
            {
                _musicPlayer = GetComponent<SimpleMusicPlayer>();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif

        private void Start()
        {
            if(_bootSettings.playMusicOnStart)
                _musicPlayer.BeginPlaying();
        }
    }
}