using SleepDev;
using UnityEngine;

namespace GameCore.UI
{
    public class ControlsUI : MonoBehaviour, IControlsUI
    {
        [SerializeField] private ProperButton _inputButton;

        public ProperButton InputButton => _inputButton;

        public void On()
        {
            gameObject.SetActive(true);
        }

        public void Off()
        {
            gameObject.SetActive(false);
        }
        
        
    }
}