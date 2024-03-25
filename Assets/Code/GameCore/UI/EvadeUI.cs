using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI
{
    public class EvadeUI : MonoBehaviour
    {
        [SerializeField] private float _punchTime;
        [SerializeField] private float _punchScale;
        [SerializeField] private float _nextDelay;
        [Header("Arrows"), Tooltip("Sequence: up, right, down ,left")]
        [SerializeField] private List<DirectionArrows> _directions;
        private DirectionArrows _current;
        private Coroutine _animating;
        
        [System.Serializable]
        public class DirectionArrows
        {
            public GameObject root;
            public List<Image> images;
        }

        public void AnimateUp()
        {
            Animate(0);
        }
        
        public void AnimateDown()
        {
            Animate(2);
        }

        public void AnimateRight()
        {
            Animate(1);
        }

        public void AnimateLeft()
        {
            Animate(3);
        }

        public void AnimateByDirection(EDirection2D direction2D)
        {
            switch (direction2D)
            {
                case EDirection2D.Up:
                    Animate(0);
                    break;
                case EDirection2D.Right:
                    Animate(1);
                    break;
                case EDirection2D.Down:
                    Animate(2);
                    break;
                case EDirection2D.Left:
                    Animate(3);
                    break;

            }
        }

        private void Animate(int index)
        {
            Stop();
            _current = _directions[index];
            _animating = StartCoroutine(Animating(_current));
        }

        public void Stop()
        {
            if(_animating != null)
                StopCoroutine(_animating);
            if (_current != null)
            {
                _current.root.SetActive(false);
            }
        }

        private IEnumerator Animating(DirectionArrows arrows)
        {
            arrows.root.SetActive(true);
            while (true)
            {
                for (var i = 0; i < arrows.images.Count; i++)
                {
                    StartCoroutine(Scaling(arrows.images[i].transform, (1+_punchScale), _punchTime));
                    yield return new WaitForSecondsRealtime(_nextDelay);
                }
            }
        }

        /// <summary>
        ///  USED TO PUNCH ON UNSCALED DELTA TIME
        /// </summary>
        private IEnumerator Scaling(Transform target, float scale, float time)
        {
            var t = 0f;
            var elapsed = 0f;
            time /= 2;
            while (t <= 1f)
            {
                target.localScale = Vector3.one * Mathf.Lerp(1, scale, t);
                elapsed += Time.unscaledDeltaTime;
                t = elapsed / time;
                yield return null;
            }
            t = elapsed = 0f;
            while (t <= 1f)
            {
                target.localScale = Vector3.one * Mathf.Lerp(scale, 1f, t);
                elapsed += Time.unscaledDeltaTime;
                t = elapsed / time;
                yield return null;
            }
            target.localScale = Vector3.one;
        }
    }
}