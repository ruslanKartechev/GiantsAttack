using System;
using System.Collections;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    [DefaultExecutionOrder(1000)]
    public class TowerSignaller1 : MonoBehaviour
    {
        public float partsGivingDelay = .5f;
        public float partsGivingRadius = 5f;
        [Range(0,3)] public int startDirection = 0;
        [Space(10)]
        [SerializeField] private Team _team;
        [SerializeField] private Tower _tower;
        [SerializeField] private List<BoatPart> _parts1;
        [SerializeField] private List<BoatPart> _parts2;
        [SerializeField] private List<BoatPart> _parts3;
        [SerializeField] private List<BoatPart> _parts4;


        private void OnValidate()
        {
            if(_tower == null)
                _tower = gameObject.GetComponent<Tower>();
        }

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            _directionIndex = startDirection;
            var teamsTargetsManager = new TeamsTargetsManager();
            _tower.Init(_team);
        }
        
        /// <summary>
        /// For timeline
        /// </summary>
        public void Tower1()
        {
            CLog.LogRed($"Tower 1");
            StartCoroutine(GivingFromList(_parts1));
        }

              
        /// <summary>
        /// For timeline
        /// </summary>
        public void Tower2()
        {
            CLog.LogRed($"Tower 2");
            StartCoroutine(GivingFromList(_parts2));
        }

      
        /// <summary>
        /// For timeline
        /// </summary>
        public void Tower3()
        {
            CLog.LogRed($"Tower 3");
            StartCoroutine(GivingFromList(_parts3));
        }
        
        /// <summary>
        /// For timeline
        /// </summary>
        public void Tower4()
        {
            CLog.LogRed($"Tower 3");
            StartCoroutine(GivingFromList(_parts4));
        }

        
        private static List<Vector3> directions = new()
        {
            new(0, 0, -1),
            // new(-1, 0, -1),
            new(-1, 0, 0),
            // new(-1, 0, 1),
            new(0, 0, 1),
            // new(1, 0, 1),
            new(1, 0, 0),
            // new(1, 0, -1),
        };

        private int _directionIndex = 0;

        private int GetInd()
        {
            var ind = _directionIndex;
            _directionIndex++;
            if(_directionIndex >= directions.Count)
                _directionIndex = 0;
            return ind;
        }
        
        private IEnumerator GivingFromList(List<BoatPart> parts)
        {
            foreach (var part in parts)
            {
                var newPart = part;
                newPart.ColliderOff();
                if (_tower.CanTake() == false)
                {
                    CLog.LogRed($"[TowerSignaller] Cannot connect no more...");
                    Destroy(newPart.gameObject);
                    yield break;
                }
                _tower.TakeBoatPart(newPart);
                yield return new WaitForSeconds(partsGivingDelay);
            }
        }
        
        private IEnumerator Giving()
        {
            var count = 4;
            for (var i = 0; i < count; i++)
            {
                var newPart = Spawn();
                newPart.ColliderOff();
                var pos = transform.position + directions[GetInd()] * partsGivingRadius;
                newPart.transform.position = pos;
                if (_tower.CanTake() == false)
                {
                    CLog.LogRed($"[TowerSignaller] Cannot connect no more...");
                    Destroy(newPart.gameObject);
                    yield break;
                }
                _tower.TakeBoatPart(newPart);
                yield return new WaitForSeconds(partsGivingDelay);
            }
        }

        private BoatPart Spawn()
        {
            var bp = GCon.GOFactory.Spawn<BoatPart>(GlobalConfig.BoatPartID);
            return bp;
        }
     
    }
}