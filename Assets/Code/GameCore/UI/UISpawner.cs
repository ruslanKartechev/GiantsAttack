using UnityEngine;

namespace GameCore.UI
{
    public class UISpawner : MonoBehaviour
    {
        [SerializeField] private string _pathTo;
        [SerializeField] private string _startName;
        [SerializeField] private string _gameplayName;
        [SerializeField] private string _completedName;
        [SerializeField] private string _failedName;
        [SerializeField] private string _tutorialName;
        [SerializeField] private string _pauseMenuName;
        [SerializeField] private string _controlsName;
        [SerializeField] private string _rouletteName;

        private IUIScreen _startMenu;
        private IUIScreen _gameplayMenu;
        private IUIScreen _completedMenu;
        private IUIScreen _failedMenu;
        private IUIScreen _tutorial;
        private IUIScreen _pause;
        private IUIScreen _roulette;
        private IControlsUI _controlsUI;

        public void Clear()
        {
            _startMenu = _gameplayMenu = _completedMenu = _failedMenu = _tutorial = _pause = _roulette = null;
        }


        public IUIScreen LoadScreen(string name)
        {
            return Load<IUIScreen>(name);
        }
        
        public T Load<T>(string name)
        {
            var prefab = Resources.Load<GameObject>(_pathTo + name);
            var instance = Instantiate(prefab);
            return instance.GetComponent<T>();
        }

        public IUIScreen GetGameplayMenu()
        {
            if (_gameplayMenu != null)
                return _gameplayMenu;
            _gameplayMenu = LoadScreen(_gameplayName);
            return _gameplayMenu;
        }
        
        public IUIScreen GetStartMenu()
        {
            if (_startMenu != null)
                return _startMenu;
            _startMenu = LoadScreen(_startName);
            return _startMenu;
        }
        
        public IUIScreen GetCompletedMenu()
        {
            if (_completedMenu != null)
                return _completedMenu;
            _completedMenu = LoadScreen(_completedName);
            return _completedMenu;
        }
                
        public IUIScreen GetFailedMenu()
        {
            if (_failedMenu != null)
                return _failedMenu;
            _failedMenu = LoadScreen(_failedName);
            return _failedMenu;
        }

        public IUIScreen GetTutorialUI()
        {
            if (_tutorial != null)
                return _tutorial;
            _tutorial = LoadScreen(_tutorialName);
            return _tutorial;
        }
        
        public IUIScreen GetPauseUI()
        {
            if (_pause != null)
                return _pause;
            _pause = LoadScreen(_pauseMenuName);
            return _pause;
        }

        public IUIScreen GetRouletteUI()
        {
            if (_roulette != null)
                return _roulette;
            _roulette = LoadScreen(_rouletteName);
            return _roulette;
        }

        public IControlsUI GetControlsUI()
        {
            return Load<IControlsUI>(_controlsName);
        }
    }
}