using SleepDev;
using SleepDev.Saving;
using UnityEngine;

namespace GameCore.Core
{
    [CreateAssetMenu(menuName = "SO/" + nameof(GConLocatorSo), fileName = nameof(GConLocatorSo), order = 0)]
    public partial class GConLocatorSo : ScriptableObject
    {
        [SerializeField] private PlayerDataSO _playerData;
        [SerializeField] private LevelsRepository _levelsRepository;
        [SerializeField] private IDataSaver _dataSaver;
        [SerializeField] private GlobalConfig _config;

        public void InitContainer()
        {
            GCon.PlayerData = _playerData.Data;
            GCon.DataSaver = _dataSaver;
            GCon.LevelRepository = _levelsRepository;
            GCon.GlobalConfig = _config;
            _config.SetupStaticFields();
        }
        
        
#if UNITY_EDITOR
        // [Space(30)]
        // [Header("EDITOR MODE")]

        public void ReleaseMode()
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }

        public void DebugMode()
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}