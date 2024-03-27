using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI
{
    public class BodyPartUI : MonoBehaviour, IBodyPartUI
    {
        [SerializeField] private Image _image;
        private bool _isAnimating;
        
        public List<Color> ColorsByLevel { get; set; }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if(_image == null)
                _image = GetComponent<Image>();
            UnityEditor.EditorUtility.SetDirty(_image);
        }
#endif
        
        public void SetDamageLevel(int level)
        {
            _image.color = ColorsByLevel[level];
        }

        public void SetNonDamageable()
        {
            _image.color = Color.gray;
        }
        
        public void Animate()
        {
            if (_isAnimating)
                return;
            _isAnimating = true;
            _image.transform.localScale = Vector3.one;
            _image.transform.DOPunchScale(Vector3.one * .1f, .1f).OnComplete(() =>
            {
                _isAnimating = false;
            });
        }
    }
}