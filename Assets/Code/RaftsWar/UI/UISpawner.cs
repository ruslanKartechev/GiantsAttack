using UnityEngine;

namespace RaftsWar.UI
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

        private IUIScreen _startMenu;
        private IUIScreen _gameplayMenu;
        private IUIScreen _completedMenu;
        private IUIScreen _failedMenu;
        private IUIScreen _tutorial;
        private IUIScreen _pause;

        public void Clear()
        {
            _startMenu = _gameplayMenu = _completedMenu = _failedMenu = _tutorial = _pause = null;
        }


        public IUIScreen Load(string name)
        {
            var prefab = Resources.Load<GameObject>(_pathTo + name);
            var instance = Instantiate(prefab);
            return instance.GetComponent<IUIScreen>();
        }

        public IUIScreen GetGameplayMenu()
        {
            if (_gameplayMenu != null)
                return _gameplayMenu;
            _gameplayMenu = Load(_gameplayName);
            return _gameplayMenu;
        }
        
        public IUIScreen GetStartMenu()
        {
            if (_startMenu != null)
                return _startMenu;
            _startMenu = Load(_startName);
            return _startMenu;
        }
        
        public IUIScreen GetCompletedMenu()
        {
            if (_completedMenu != null)
                return _completedMenu;
            _completedMenu = Load(_completedName);
            return _completedMenu;
        }
                
        public IUIScreen GetFailedMenu()
        {
            if (_failedMenu != null)
                return _failedMenu;
            _failedMenu = Load(_failedName);
            return _failedMenu;
        }

        public IUIScreen GetTutorialUI()
        {
            if (_tutorial != null)
                return _tutorial;
            _tutorial = Load(_tutorialName);
            return _tutorial;
        }
        
        public IUIScreen GetPauseUI()
        {
            if (_pause != null)
                return _pause;
            _pause = Load(_pauseMenuName);
            return _pause;
        }
    }
}