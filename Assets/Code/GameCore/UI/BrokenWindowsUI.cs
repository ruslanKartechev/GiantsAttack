using System.Collections.Generic;
using UnityEngine;

namespace GameCore.UI
{
    public class BrokenWindowsUI : MonoBehaviour, IBrokenWindowsUI
    {
        [SerializeField] private List<RectTransform> _images;
        private int _lastIndex;
        private int _currentCount = 0;
        
        public void BreakAll()
        {
            var count = _images.Count - _currentCount;
            _currentCount += count;
            for (var i = 0; i < count; i++)
            {
                _images[_lastIndex].gameObject.SetActive(true);
                _lastIndex++;
            }
        }
        
        public void BreakRandomNumber()
        {
            if (_lastIndex >= _images.Count)
                return;
            var max = _images.Count - _currentCount;
            var count = UnityEngine.Random.Range(0, max + 1);
            _currentCount += count;
            for (var i = 0; i < count; i++)
            {
                _images[_lastIndex].gameObject.SetActive(true);
                _lastIndex++;
            }
        }

    }
}