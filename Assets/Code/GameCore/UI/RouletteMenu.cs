using System;
using SleepDev.UIUtils;
using UnityEngine;

namespace GameCore.UI
{
    public class RouletteMenu : MonoBehaviour, IUIScreen
    {
        [SerializeField] private RouletteUI _roulette;
        [SerializeField] private PopAnimator _popAnimator;

        
        public RouletteUI RouletteUI => _roulette;
        public GameObject Go => gameObject;

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
            On();
            _popAnimator.ZeroAndPlay();
            onDone.Invoke();
        }

        public void Hide(Action onDone)
        {
            _popAnimator.PlayBackwards(() =>
            {
                Off();
                onDone.Invoke();
            });
        }
    }
}