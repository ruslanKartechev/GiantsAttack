using System.Collections;
using SleepDev.Utils;
using UnityEngine;
using SleepDev;

namespace RaftsWar.Boats
{
    public class BoatPartsManager : MonoBehaviour
    {
        [Space(10)]
        [SerializeField] private int _minCount = 8;
        [SerializeField] private int _countToSpawn = 4;
        [SerializeField] private BoatPartsSpawner _defaultPartsSpawner;
        [Space(10)]
        [SerializeField] private BoatPartsSpawner _unitsSpawner;
        [SerializeField] private int _minCountUnits = 8;
        [SerializeField] private int _countToSpawnUntis = 1;
        [Space(10)]
        [SerializeField] private float _checkDelay = 1f;
        [SerializeField] private float _spawnDelay = 1f;
        [SerializeField] private BoatViewSettingsSo _defultBoatView;
        [SerializeField] private UnitViewSettingsSo _defaultUnitView;
        private AvailableBoatParts _availablePool;
        // private AvailableBoatParts _poolUnits;
        private Coroutine _countChecking;
        private BoatPart _lastReturnedPart;
        private int _unitsCount;

        private void Start()
        {
            _availablePool = new AvailableBoatParts();
            // _poolUnits = new AvailableBoatParts();
            RaftsDataContainer.BoatPartsPlane = _defaultPartsSpawner.Plane;
            RaftsDataContainer.DefaultBoatsView = _defultBoatView.settings;
            RaftsDataContainer.DefaultUnitsView = _defaultUnitView.settings;
            _defaultPartsSpawner.DefaultView = _unitsSpawner.DefaultView = _defultBoatView;
            // default bps
            var spawned = _defaultPartsSpawner.SpawnInitialParts(false);
            foreach (var p in spawned)
            {
                _availablePool.Add(p);
                p.OnBecameAvailable += OnBecameAvailable;
            }
            // bp with units
            spawned = _unitsSpawner.SpawnInitialParts(true);
            foreach (var p in spawned)
            {
                _availablePool.Add(p);
                _unitsCount++;
                p.OnBecameAvailable += OnBecameAvailableUnits;
            }

            StartCheck();
        }

        public void Stop()
        {
            CLog.Log($"[PartsManager] Stopped");
            if(_countChecking != null)
                StopCoroutine(_countChecking);
        }

        public void StartCheck()
        {
            CLog.Log($"[PartsManager] Started check");
            Stop();
            _countChecking = StartCoroutine(CountChecking());
        }

        public BoatPart GetRandomAvailable()
        {
            var bp = _availablePool.Parts.Random();
            var it = 0;
            const int itMax = 10;
            while (bp == _lastReturnedPart && it < itMax)
            {
                bp = _availablePool.Parts.Random();
                it++;
            }
            _lastReturnedPart = bp;
            return bp;
        }
        
        private void OnBecameAvailable(BoatPart part, bool available)
        {
            if (available)
                _availablePool.Add(part);
            else
            {
                part.OnBecameAvailable -= OnBecameAvailable;
                _availablePool.Remove(part);
                _defaultPartsSpawner.ClearPointFor(part);
            }
        }
        
        private void OnBecameAvailableUnits(BoatPart part, bool available)
        {
            if (available)
            {
                _availablePool.Add(part);
                _unitsCount++;
            }
            else
            {
                part.OnBecameAvailable -= OnBecameAvailableUnits;
                _availablePool.Remove(part);
                _unitsSpawner.ClearPointFor(part);
                _unitsCount--;
            }
        }

        private IEnumerator CountChecking()
        {
            yield return null;
            while (true)
            {
                yield return new WaitForSeconds(_checkDelay);
                // CLog.LogWhite($"[PartsManager] has {_availablePool.Count - _unitsCount}, min count {_minCount}");
                if (_availablePool.Count - _unitsCount <= _minCount)
                {
                    for (var i = 0; i < _countToSpawn; i++)
                    {
                        var part = _defaultPartsSpawner.SpawnFreshBoatPartAtRandomPoint(false);
                        if (part == null)
                        {
                            break;
                        }
                        part.OnBecameAvailable -= OnBecameAvailable;
                        part.OnBecameAvailable += OnBecameAvailable;
                        _availablePool.Add(part);
                        yield return new WaitForSeconds(_spawnDelay);
                    }
                }
                if (_unitsCount <= _minCountUnits)
                {
                    for (var i = 0; i < _countToSpawnUntis; i++)
                    {
                        var part = _defaultPartsSpawner.SpawnFreshBoatPartAtRandomPoint(true);
                        if (part == null)
                            break;
                        part.OnBecameAvailable -= OnBecameAvailable;
                        part.OnBecameAvailable += OnBecameAvailable;
                        _availablePool.Add(part);
                        _unitsCount++;
                        yield return new WaitForSeconds(_spawnDelay);
                    }
                }

            }
        }


#if UNITY_EDITOR
        public void E_GetSpawnPoints()
        {
            _defaultPartsSpawner.E_GetSpawnPoints();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        public void E_HighlightInitialPoints()
        {
            _defaultPartsSpawner.E_SetEditorPointsColors();
            _unitsSpawner.E_SetEditorPointsColors();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        public void E_GetAllParts()
        {
            var parts  = GameUtils.GetFromAllChildren<BoatPart>(transform);
            _availablePool.SetParts(parts);
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
}