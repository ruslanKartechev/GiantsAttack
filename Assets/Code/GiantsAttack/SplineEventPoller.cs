using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace GiantsAttack
{
    public class SplineEventPoller : MonoBehaviour
    {
        [SerializeField] private List<EventData> _eventData;
        [SerializeField] private bool _autoStart;
        [SerializeField] private SplineMover _targetMover;
        private int _index;
        private Coroutine _working;

        public SplineMover targetMover
        {
            get => _targetMover;
            set => _targetMover = value;
        }

        [System.Serializable]
        private class EventData
        {
            [Range(0f,1f)] public float percent;
            public List<SplineEvent> events;
        }


        private void Start()
        {
            if(_autoStart)
                Begin();
        }

        public void Begin()
        {
            Stop();
            _working = StartCoroutine(Working());
        }

        public void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }

        private IEnumerator Working()
        {
            var loop = true;
            while (loop)
            {
                if (_eventData[_index].percent <= _targetMover.InterpolationT)
                {
                    foreach (var @event in _eventData[_index].events)
                    {
                        @event.Activate();
                    }
                    _index++;
                    if (_index >= _eventData.Count)
                    {
                        // CLog.Log($"[SplineEventPoller] All events passed");
                        loop = false;
                        yield break;
                    }
                    
                }
                yield return null;
            }
        }
        
#if UNITY_EDITOR
        [Space(20)]
        public bool e_doDraw = true;
        public float e_radius;
        public Color e_color;
        public Color e_color2;

        public void OnDrawGizmos()
        {
            if(e_doDraw)
                Draw();
        }

        private void Draw()
        {
            if (_targetMover == null || _targetMover.spline == null)
                return;
            var spline = _targetMover.spline;
            var oldColor = Gizmos.color;
            Gizmos.color = e_color;
            var col1 = true;
            foreach (var data in _eventData)
            {
                if (col1)
                    Gizmos.color = e_color;
                else
                    Gizmos.color = e_color2;
                col1 = !col1;
                var pos = spline.Spline.EvaluatePosition(data.percent);
                Gizmos.DrawSphere(spline.transform.TransformPoint(pos), e_radius);
            }
            Gizmos.color = oldColor;
        }
#endif

    }
}