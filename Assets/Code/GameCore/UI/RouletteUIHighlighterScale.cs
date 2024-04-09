using DG.Tweening;
using UnityEngine;

namespace GameCore.UI
{
    public class RouletteUIHighlighterScale : RouletteUIHighlighter
    {
        public float timeUp;
        public float timeDown;
        public float scale;
        public float scaleNormal;
        public RectTransform target;
        
        public override void OnSelected()
        {
            target.DOKill();
            target.DOScale(Vector3.one * (scaleNormal * scale), timeUp);
        }

        public override void OnDeselected()
        {
            target.DOKill();
            target.DOScale(Vector3.one * scaleNormal, timeDown);
        }
    }
}