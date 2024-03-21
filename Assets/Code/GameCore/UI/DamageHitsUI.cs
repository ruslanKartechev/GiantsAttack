using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace GameCore.UI
{
    public class DamageHitsUI : MonoBehaviour, IDamageHitsUI
    {
        [SerializeField] private float _radius;
        [SerializeField] private float _duration;
        [SerializeField] private List<TextMeshProUGUI> _texts;
        private int _index;
        private Camera _camera;
        
        private void OnEnable()
        {
            _camera =Camera.main;
        }


        public void ShowHit(Vector3 worldPos, float damage)
        {
            var screenPos = _camera.WorldToScreenPoint(worldPos);
            if (_index >= _texts.Count)
                _index = 0;
            var t = _texts[_index];
            t.text = $"-{damage}";
            t.DOKill();
            t.transform.localScale = Vector3.one;
            t.transform.position = screenPos;
            t.gameObject.SetActive(true);
            var pos = screenPos + (Vector3)UnityEngine.Random.insideUnitCircle * _radius;
            t.transform.DOMove(pos, _duration).OnComplete(() =>
            {
                t.gameObject.SetActive(false);
            });
            t.transform.DOScale(Vector3.one * .5f, _duration);
            _index++;
        }
    }
}