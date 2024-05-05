using GameCore.UI;
using SleepDev;
using UnityEngine;

namespace GameCore.Core
{
    public class GConLocator : MonoBehaviour, IGConLocator
    {
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private PlayerInput _input;
        [SerializeField] private SceneSwitcher _sceneSwitcher;
        [SerializeField] private SlowMotionManager _slowMotionEffect;
        [SerializeField] private GConLocatorSo _soLocator;
        [SerializeField] private UISpawner _uiSpawner;
        [SerializeField] private GameObjectFactory _factory;
        [SerializeField] private PEPlayer _explosionPlayer;

        public void InitContainer()
        {
            GCon.SceneSwitcher = _sceneSwitcher;
            GCon.LevelManager = _levelManager;
            GCon.SlowMotion = _slowMotionEffect; 
            GCon.Input = _input;
            GCon.UIFactory = _uiSpawner;
            GCon.GOFactory = _factory;
            GCon.ExplosionPlayer = _explosionPlayer;
            _factory.Rebuild();
            _soLocator.InitContainer();
        }
        
    }
    
}