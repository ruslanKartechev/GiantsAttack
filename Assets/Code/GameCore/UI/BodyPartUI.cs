using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI
{
    public class BodyPartUI : MonoBehaviour, IBodyPartUI
    {
        [SerializeField] private Image _image;
        
        public List<Color> ColorsByLevel { get; set; }
        
        public void SetDamageLevel(int level)
        {
            _image.color = ColorsByLevel[level];
        }
    }
}