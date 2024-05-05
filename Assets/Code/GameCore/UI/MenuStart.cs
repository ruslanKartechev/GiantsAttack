﻿using System;
using GameCore.Core;
using SleepDev;
using TMPro;
using UnityEngine;

namespace GameCore.UI
{
    public class MenuStart : MonoBehaviour, IMenuStart
    {
        [SerializeField] private PopAnimator _popAnimator;
        [SerializeField] private ProperButton _playButton;
        [SerializeField] private SoundSo _clickSound;
        [SerializeField] private LocationProgressBar _progressBar;
        [SerializeField] private ObjectiveUI _objectiveUI;
        [SerializeField] private TextMeshProUGUI _levelNumberText;
        private Action _callback;
        
        public GameObject Go => gameObject;
        
        private void OnEnable()
        {
            _playButton.OnDown += OnPlay;
        }

        private void OnDisable()
        {
            _playButton.OnDown -= OnPlay;
        }

        public void On()
        {
            gameObject.SetActive(true);
        }

        public void Off()
        {
            gameObject.SetActive(false);
        }

        public void Show(Action onDone)
        {
            _levelNumberText.text = $"LEVEL {GCon.PlayerData.LevelTotal + 1}";
            _playButton.interactable = false;
            _popAnimator.ZeroAndPlay(() =>
            {
                ActivateButtons();
                onDone?.Invoke();
            });
        }

        public void Hide(Action onDone)
        {
            Off();
            onDone?.Invoke();
        }

        public void Show(Action playCallback, Action onShown, string objective)
        {
            _callback = playCallback;
            _objectiveUI.SetObjectiveText(objective);
            _progressBar.Init();
            Show(onShown);
        }

        public void ShowObjective(Action callback)
        {
            _objectiveUI.Play(callback);
        }
        
        private void ActivateButtons()
        {
            _playButton.interactable = true;
        }
        
        private void OnPlay()
        {
            _clickSound.Play();
            _callback?.Invoke();
        }

 
        
    }
    
}