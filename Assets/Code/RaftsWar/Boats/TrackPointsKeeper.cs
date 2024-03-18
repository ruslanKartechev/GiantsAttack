using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class TrackPointsKeeper
    {
        protected List<Transform> _trackPoints = new List<Transform>(10);
        protected List<Transform> _reserved = new List<Transform>(10);
        private Dictionary<Transform, BoatPart> _map = new Dictionary<Transform, BoatPart>(10);
        protected Transform _parent;

        public IList<Transform> ActivePoints => _trackPoints;
        public Dictionary<Transform, BoatPart> Map => _map; 
        public KeyValuePair<Transform, BoatPart> RootPair { get; private set; }

        
        public TrackPointsKeeper(Transform parent)
        {
            _parent = parent;
        }

        public void AddFirstPoint(BoatPart part)
        {
            var tp = new GameObject("tp").transform;
            tp.position = part.transform.position;
            tp.SetParent(_parent);
            var local = tp.localPosition.XZPlane();
            tp.localPosition = local;
            part.TrackPoint = tp;
            _trackPoints.Add(tp);
            _map.Add(tp, part);
            RootPair = new KeyValuePair<Transform, BoatPart>(tp, part);
        }
        

        public Transform AddPointAt(BoatPart part, Vector3 newTrackPointLocalPos)
        {
            Transform tp = null;
            if (_reserved.Count > 0)
            {
                tp = _reserved[0];
                _reserved.Remove(tp);
                tp.gameObject.SetActive(true);
            }
            else
            {
#if UNITY_EDITOR
                tp = new GameObject($"tp_{part.gameObject.name}").transform;
#else
                tp = new GameObject($"tp").transform;
#endif
            }
            tp.SetParent(_parent);
            tp.localPosition = newTrackPointLocalPos;
            _map.Add(tp, part);
            _trackPoints.Add(tp);
            part.TrackPoint = tp;
            // Debug.DrawLine(tp.position, tp.position + Vector3.up * 7f, Color.magenta, 10f);
            return tp;
        }

        public void RemovePointFor(BoatPart part)
        {
            var tp = part.TrackPoint;
            tp.gameObject.SetActive(false);
            _trackPoints.Remove(tp);
            _reserved.Add(tp);
            _map.Remove(tp);
        }
        
    }
}