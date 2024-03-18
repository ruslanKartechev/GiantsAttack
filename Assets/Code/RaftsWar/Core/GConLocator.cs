using System;
using RaftsWar.Boats;
using RaftsWar.UI;
using SleepDev;
using SleepDev.Levels;
using SleepDev.Scenes;
using SleepDev.SlowMotion;
using UnityEngine;

namespace RaftsWar.Core
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

        public void InitContainer()
        {
            GCon.SceneSwitcher = _sceneSwitcher;
            GCon.LevelManager = _levelManager;
            GCon.SlowMotion = _slowMotionEffect; 
            GCon.Input = _input;
            GCon.UIFactory = _uiSpawner;
            GCon.GOFactory = _factory;
            _factory.Rebuild();
            _soLocator.InitContainer();
        }
        
    }
    
}