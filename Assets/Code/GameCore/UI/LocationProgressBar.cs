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
        [SerializeField] private List<Sprite> _envSprites;
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
            // CLog.LogRed($"Current index {GCon.LevelManager.CurrentIndex}, next index {GCon.LevelManager.NextIndex}");
            var envInd1 = repo.GetEnvironmentIndex(repo.GetLevel(GCon.LevelManager.CurrentIndex).SceneName);
            _leftLocIcon.sprite = _envSprites[envInd1];
            var envInd2 = repo.GetEnvironmentIndex(repo.GetLevel(GCon.LevelManager.NextIndex).SceneName);
            _rightLocIcon.sprite = _envSprites[envInd2];
        }
    }
}