using System;
using UnityEngine;

namespace GameCore.UI
{
    public class RouletteMenu : MonoBehaviour, IUIScreen
    {
        [SerializeField] private RouletteUI _roulette;

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
            onDone.Invoke();
        }

        public void Hide(Action onDone)
        {
            Off();
        }
        
    }
}