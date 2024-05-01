using System;
using System.Collections;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class RunawayCrowd : MonoBehaviour, IRunaway
    {
        [SerializeField] private List<Transform> _humanPoints;
        [SerializeField] private float _appearTime;
        [SerializeField] private RunawayHumanSpawner _spawner;
        [SerializeField] private SplineMover _splineMover;
        private List<RunawayHuman> _humans;

        public SplineMover Mover => _splineMover;

        private void Awake()
        {
#if UNITY_EDITOR
            E_Hide();
#endif
        }

        public void Init()
        {
            SpawnAndScare();
        }

        public void BeginMoving()
        {
            StartCoroutine(Moving());
            // _splineMover.MoveAccelerated();
        }

        public void Stop()
        { }

        public void Kill()
        { }


        private void SpawnAndScare()
        {
            _humans = new List<RunawayHuman>(_humanPoints.Count);
            foreach (var point in _humanPoints)
            {
                var human = _spawner.SpawnOne();
                human.Transform.parent = point;
                human.transform.CopyPosRot(point.GetChild(0));
                _humans.Add(human);
                human.RandomizeAnimationOffset();
                human.PlayScared();
            }    
        }
       
        
        private IEnumerator Moving()
        {
            foreach (var hh in _humans)
                hh.PlayRun();
            yield return null;
            var elapsed = 0f;
            var time = _appearTime;
            var t = 0f;
            while (elapsed < time)
            {
                for (var i = 0; i < _humans.Count; i++)
                {
                    _humans[i].Transform.localPosition =
                        Vector3.Lerp(_humanPoints[i].GetChild(0).localPosition, Vector3.zero, t);
                    _humans[i].Transform.localRotation =
                        Quaternion.Lerp(_humanPoints[i].GetChild(0).localRotation, Quaternion.identity, t);
                }
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            for (var i = 0; i < _humans.Count; i++)
            {
                _humans[i].Transform.localPosition = Vector3.zero;
                _humans[i].Transform.localRotation = Quaternion.identity;
            }
        }

#if UNITY_EDITOR
        [Space(10)]
        [SerializeField] private List<Transform> e_current;

        [ContextMenu("BuildFromCurrent")]
        public void BuildFromCurrent()
        {
            if (_humanPoints.Count > 0)
            {
                foreach (var tr in _humanPoints)
                {
                    if(tr != null)
                        DestroyImmediate(tr.gameObject);
                }
            }
            _humanPoints = new List<Transform>();
            byte ind = 0;
            foreach (var curr in e_current)
            {
                var temp = new GameObject($"p_{ind}").transform;
                temp.CopyPosRot(curr);
                temp.parent = transform;
                var startTemp = new GameObject($"p_start_{ind}").transform;
                startTemp.SetParentAndCopy(temp);
                _humanPoints.Add(temp);
                ind++;
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("E_Replace")]
        public void E_Replace()
        {
            var ss = new List<Transform>();
            foreach (var pp in _humanPoints)
            {
                ss.Add(pp.parent);
            }
            _humanPoints = ss;
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        [ContextMenu("E_Hide")]
        public void E_Hide()
        {
            foreach (var pp in _humanPoints)
            {
                if(pp.childCount == 0)
                    continue;
                var chil = pp.GetChild(0);
                if (chil.childCount == 0)
                    continue;
                chil.GetChild(0).gameObject.SetActive(false);
            }
            if(Application.isPlaying)
                UnityEditor.EditorUtility.SetDirty(this);
        }
        
        [ContextMenu("E_Clear")]
        public void Clear()
        {
            foreach (var pp in _humanPoints)
            {
                if(pp.childCount == 0)
                    continue;
                var chil = pp.GetChild(0);
                if (chil.childCount == 0)
                    continue;
                DestroyImmediate(chil.GetChild(0).gameObject);
            }
            if(Application.isPlaying)
                UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

    }
}