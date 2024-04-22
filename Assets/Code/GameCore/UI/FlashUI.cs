using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI
{
    public class FlashUI : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private float _time;
        [SerializeField] private Vector2 _alpha;
        [SerializeField] private Ease _ease;

        [ContextMenu("Play")]
        public void Play()
        {
            _image.DOKill();
            var color = _image.color;
            color.a = _alpha.x;
            _image.color = color;
            _image.DOFade(_alpha.y, _time).SetEase(_ease);
        }
    }
}