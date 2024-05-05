#define HAS_SDK
using System.Collections;
using System.Globalization;
using System.Threading;
using MadPixelAnalytics;
using MAXHelper;
using SleepDev;
using SleepDev.Saving;
using SleepDev.Vibration;
using UnityEngine;

namespace GameCore.Core
{
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private BootSettings _bootSettings;
        [SerializeField] private GameObject _fpsCanvasPrefab;
        [SerializeField] private GameObject _poolsManagerGo;
        [SerializeField] private GameObject _soundManager;
        [SerializeField] private SimpleMusicPlayer _musicPlayer;
#if HAS_SDK
        [SerializeField] private AnalyticsManager _analytics;
        [SerializeField] private AdsManager _ads;
#endif
        private VibrationManager _vibrationManager;
        
        public static void SetUSCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        public static void InitSound(ISoundManager soundManager)
        {
            soundManager.Init(GCon.PlayerData.SoundStatus, GCon.PlayerData.SoundVolume);
        }

        public static VibrationManager InitVibration()
        {
            return new VibrationManager(GCon.PlayerData.VibrationStatus);
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            StartCoroutine(Working());
        }

        private IEnumerator Working()
        {
            GlobalState.NoBootSceneMode = false;
            GlobalState.DevSceneMode = false;
            InitFramerate();
            SetUSCulture();
            InitContainer();
            InitSaves();
            _vibrationManager = InitVibration();
            var soundManager = _soundManager.GetComponent<ISoundManager>();
            InitSound(soundManager);
            if(_bootSettings.playMusicOnStart)
                _musicPlayer.BeginPlaying();
            if (_bootSettings.ShowFPSCanvas)
            {
                // CLog.LogWhite("[GM] fps canvas");
                var fps = Instantiate(_fpsCanvasPrefab, transform);
            }
            yield return null;
            if (_poolsManagerGo.TryGetComponent<IObjectPoolsManager>(out var poolsManager))
                poolsManager.BuildPools();
            yield return null;
            CLog.LogWhite("[GM] SDK Initiate");
#if HAS_SDK
            _ads.InitApplovin();
            yield return new WaitUntil(AdsManager.Ready);
            InitAnalytics();
            yield return null;
            PlayGame();
#else
            PlayGame();
#endif
        }

        private void InitFramerate()
        {
            CLog.LogWhite($"[GM] Init frame rate");
            const int maxFrameRate = 120;
            if(_bootSettings.CapFPS)
                Application.targetFrameRate = _bootSettings.FpsCap;
            else 
                Application.targetFrameRate = maxFrameRate;
        }

        private void InitContainer()
        {
            var containerLocator = gameObject.GetComponent<IGConLocator>();
            containerLocator.InitContainer();
        }

        private void InitSaves()
        {
            CLog.LogWhite($"[GM] Init saves");
            if (_bootSettings.ClearAllSaves)
                GCon.DataSaver.Clear();
            var dataInit = gameObject.GetComponent<ISaveInitializer>();
            dataInit.InitSavedData();    
            
            if (_bootSettings.doPeriodicSave)
            {
                var saver = gameObject.GetComponent<IPeriodicDataSaver>();
                saver.SetInterval(_bootSettings.dataSavePeriod);
                saver.Begin();
            }
        }
        
        private void InitAnalytics()
        {
#if HAS_SDK
            CLog.LogWhite( $"[GM] Init analytics {_bootSettings.InitAnalytics}");
            if(!_bootSettings.InitAnalytics)
                return;
            _analytics.Init();
            _analytics.SubscribeToAdsManager();
#endif
        }

        private void OnApplicationQuit()
        {
            GCon.DataSaver.Save();
        }
        

        private void PlayGame()
        {       
            CLog.LogWhite($"[GM] Play Game");
            try
            {
                GCon.LevelManager.LoadCurrent();
            }
            catch (System.Exception ex)
            {
                Debug.Log($"Exception {ex.Message}\n{ex.StackTrace}");
            }   
        }
    }
}
