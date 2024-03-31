using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace GameCore.UI
{
    public class AimUI : MonoBehaviour, IAimUI
    {
        private const float animTime = .25f;
        [SerializeField] private Transform _aim;
        private Coroutine _rotating;
        
        public void Show(bool animated)
        {
            gameObject.SetActive(true);
            if (animated)
            {
                _aim.transform.DOKill();
                _aim.transform.localScale = Vector3.zero;
                _aim.transform.DOScale(Vector3.one, animTime);
            }
        }

        public void Hide(bool animated)
        {
            if (animated)
            {
                _aim.transform.DOScale(Vector3.zero, animTime).OnComplete(() =>
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

        public void BeginRotation(float speed)
        {
            StopRotation();
            _rotating = StartCoroutine(Rotation(speed));
        }

        public void StopRotation()
        {
            if(_rotating != null)
                StopCoroutine(_rotating);
        }

        public Vector3 GetScreenPos()
        {
            return _aim.position;
        }

        private IEnumerator Rotation(float speed)
        {
            var elapsed = 0f;
            var accelerationTime = .5f;
            var s = 0f;
            while (true)
            {
                s = Mathf.Lerp(0f, speed, elapsed / accelerationTime);
                var angles = _aim.localEulerAngles;
                angles.z += s * Time.deltaTime;
                _aim.localEulerAngles = angles;
                elapsed += Time.deltaTime;
                yield return null;
            }
        }   
    }
}