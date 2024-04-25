using System.Collections.Generic;
using GameCore.Core;
using UnityEngine;
using UnityEngine.UI;
using SleepDev;

namespace GameCore.UI
{
    public class LocationProgressBar : MonoBehaviour
    {
        [SerializeField] private Image _leftLocIcon;
        [SerializeField] private Image _rightLocIcon;
        [SerializeField] private List<LocationProgressBarLevel> _levels;

        public void Init()
        {
            SetupLevels();
            SetupLocationImages();
        }

        private void SetupLevels()
        {
            var level = GCon.PlayerData.LevelTotal % _levels.Count;
            _levels[level].ShowCurrent();
            for(var i = 0; i < level; i++)
                _levels[i].ShowCompleted();
            for (var i = level + 1; i < _levels.Count; i++)
                _levels[i].ShowFuture();
        }
        
        private void SetupLocationImages()
        {
            var repo = GCon.LevelRepository;
            _leftLocIcon.sprite = EnvironmentState.GetIconForScene(repo.GetLevel(GCon.LevelManager.CurrentIndex).SceneName);
            _rightLocIcon.sprite = EnvironmentState.GetIconForScene(repo.GetLevel(GCon.LevelManager.NextIndex).SceneName);
            if (_leftLocIcon.sprite == null)
                CLog.LogRed($"NULLLLLLLL LEFT");
            if (_rightLocIcon.sprite == null)
                CLog.LogRed($"NULLLLLLLL Right");

        }
    }
}