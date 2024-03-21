using DG.Tweening;
using UnityEngine;

namespace GameCore.UI
{
    public class AimUI : MonoBehaviour, IAimUI
    {
        [SerializeField] private Transform _aim;
        
        public void Show(bool animated)
        {
            gameObject.SetActive(true);
            if (animated)
            {
                _aim.transform.DOKill();
                _aim.transform.localScale = Vector3.zero;
                _aim.transform.DOScale(Vector3.one, .5f);
            }
        }

        public void Hide(bool animated)
        {
            if (animated)
            {
                _aim.transform.DOScale(Vector3.zero, .5f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            }
            else
                gameObject.SetActive(false);
        }

        public void SetPosition(Vector3 screenPos)
        {
            _aim.transform.position = screenPos;
        }
    }
}