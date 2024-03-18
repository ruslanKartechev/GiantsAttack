using System.Collections.Generic;
using SleepDev;
using SleepDev.Utils;
using UnityEditor;
using UnityEngine;

namespace RaftsWar.Boats
{
    [System.Serializable]
    public class BoatPartsSpawner : MonoBehaviour
    {
        private const int InitialPoolSize = 30;
        private const float AnimateTime = .2f;
        
        [SerializeField] private Transform _parent;
        [SerializeField] private List<Transform> _initialSpawnPoints;
        [SerializeField] private List<Transform> _possibleSpawnPoints;
        private BoatViewSettingsSo _defaultView;
        
        private List<Transform> _availablePoints = new List<Transform>(InitialPoolSize);
        private Dictionary<BoatPart, Transform> _partPointMap = new Dictionary<BoatPart, Transform>(InitialPoolSize);
        public Transform Plane => _parent;
        public BoatViewSettingsSo DefaultView
        {
            get => _defaultView;
            set => _defaultView = value;
        }
        
#if UNITY_EDITOR
        [Space(20)] 
        public Color possiblePointColor;
        public Color initialPointColor;
        public string spawnPointName = "sp";
        
        public void E_SetEditorPointsColors()
        {
            SetColor(_possibleSpawnPoints, possiblePointColor);
            SetColor(_initialSpawnPoints, initialPointColor);
            
            void SetColor(List<Transform> points, Color color)
            {
                for (var i = points.Count - 1; i >= 0; i--)
                {
                    var p = points[i];
                    if (p == null)
                    {
                        points.RemoveAt(i);
                        continue;
                    }

                    if (p.TryGetComponent<EditorPoint>(out var ep))
                    {
                        ep.color = color;
                        EditorUtility.SetDirty(ep);
                    }
                }
            }
        }

        public void E_GetSpawnPoints()
        {
            var points = SleepDev.Utils.GameUtils.GetAllChildrenAndRename(_parent, spawnPointName);
            _possibleSpawnPoints.Clear();
            _possibleSpawnPoints.AddRange(points);
            E_SetEditorPointsColors();
            UnityEditor.EditorUtility.SetDirty(this);
        }
       

        [ContextMenu("GizmoOn")]
        public void GizmoOn()
        {
            SetGizmos(true);
        }

        [ContextMenu("Gizmo Off")]
        public void GizmoOff()
        {
            SetGizmos(false);   
        }

        public void SetGizmos(bool on)
        {
            foreach (var point in _possibleSpawnPoints)
            {
                if (point == null)
                    continue;
                if (point.TryGetComponent<EditorPoint>(out var ep))
                    ep.doDraw = on;
            }
        }
#endif

        public void SetParent(Transform parent)
        {
            _parent = parent;
        }

        public IList<BoatPart> SpawnInitialParts(bool withUnit)
        {
            _availablePoints = new List<Transform>(InitialPoolSize);
            _partPointMap = new Dictionary<BoatPart, Transform>(InitialPoolSize);
            var list = new List<BoatPart>();
            foreach (var point in _possibleSpawnPoints)
                _availablePoints.Add(point);
            foreach (var point in _initialSpawnPoints)
            {
                var boatPart = Spawn(point, withUnit);
                // CLog.LogRed($"Spawned a boat part {boatPart.gameObject.name}");
                boatPart.SetView(_defaultView.settings);
                BoatUtils.SetBoatPartFree(boatPart);
                list.Add(boatPart);
                _partPointMap.Add(boatPart, point);
                _availablePoints.Remove(point);
                if(withUnit)
                    boatPart.Unit.Unit.RandomizeRotation();
            }
            return list;
        }

        public BoatPart SpawnFreshBoatPartAtRandomPoint(bool withUnit)
        {
            var point = GetRandomPoint();
            var boatPart = Spawn(point, withUnit);
            if (boatPart == null)
                return null;
            
            _partPointMap.Add(boatPart, point);
            boatPart.SetView(_defaultView.settings);
            BoatUtils.SetBoatPartFree(boatPart);
            BoatUtils.AnimateBoatPartPop(boatPart, AnimateTime);
            return boatPart;
        }
        
        public Transform GetRandomPoint()
        {
            var p = _availablePoints.Random();
            _availablePoints.Remove(p);
            return p;
        }

        public BoatPart Spawn(Transform point, bool withUnit = false)
        {
            if (point == null)
            {
                CLog.LogRed("Null spawn point");
                return null;
            }
            var pool = withUnit ? GCon.BoatPartsPoolsWithUnits : GCon.BoatPartsPool;
            var boatPart = pool.GetObject();
            boatPart.transform.parent = _parent;
            boatPart.Scalable.localScale = Vector3.one;
            boatPart.transform.SetPositionAndRotation(point.position, point.rotation);
            boatPart.gameObject.SetActive(true);
            if(withUnit)
                boatPart.Unit.Unit.RandomizeRotation();
            return boatPart;
        }

        public void ClearPointFor(BoatPart part)
        {
            // CLog.LogBlue($"Cleared part {part.gameObject.name}");
            var point = _partPointMap[part];
            _partPointMap.Remove(part);
            _availablePoints.Add(point);
        }
    }
}