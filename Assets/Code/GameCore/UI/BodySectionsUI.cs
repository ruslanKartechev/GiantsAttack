using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GameCore.UI
{
    public class BodySectionsUI : MonoBehaviour, IBodySectionsUI
    {
        [SerializeField] private GameObject _block;
        [SerializeField] private List<Color> _colorsByLevel;
        [SerializeField] private List<BodyPartUI> _parts;

        public IBodyPartUI GetBodyPartByID(int id)
        {
            if (_parts.Count <= id)
            {
                CLog.LogRed($"ID out of range");
                return null;
            }
            return _parts[id];
        }

        public void Init()
        {
            foreach (var p in _parts)
                p.ColorsByLevel = _colorsByLevel;
        }

        public void Show()
        {
            _block.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _block.gameObject.SetActive(false);
        }
    }
}