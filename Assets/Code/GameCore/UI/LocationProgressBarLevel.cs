using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI
{
    public class LocationProgressBarLevel : MonoBehaviour
    {
        [SerializeField] private Image _iconCompleted;
        [SerializeField] private Image _iconCurrent;
        // [SerializeField] private Image black;

        public void ShowCurrent()
        {
            _iconCompleted.enabled = false;
            _iconCurrent.enabled = true;
        }

        public void ShowCompleted()
        {
            _iconCompleted.enabled = true;
            _iconCurrent.enabled = false;
        }

        public void ShowFuture()
        {
            _iconCompleted.enabled = false;
            _iconCurrent.enabled = false;
        }
    }
}