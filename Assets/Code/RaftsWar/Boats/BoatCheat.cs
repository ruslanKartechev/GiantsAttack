using System.Collections;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class BoatCheat : MonoBehaviour
    {
    
        private static List<Vector3> directions = new()
        {
            new(0, 0, -1),
            new(-1, 0, -1),
            new(-1, 0, 0),
            new(-1, 0, 1),
            new(0, 0, 1),
            new(1, 0, 1),
            new(1, 0, 0),
            new(1, 0, -1),
        };
        private static int _dirInd = 0;

        private static int GetInd()
        {
            if (_dirInd >= directions.Count)
                _dirInd = 0;
            var val = _dirInd;
            _dirInd++;
            return val;
        }
        
        
        [SerializeField] private Boat _boat;

        public void GiveParts()
        {
            StopAllCoroutines();
            StartCoroutine(Giving());
        }

        private IEnumerator Giving()
        {
            const float delay = .25f;
            var count = 4;
            for (var i = 0; i < count; i++)
            {
                var newPart = Spawn();
                newPart.ColliderOff();
                var pos = _boat.RootPart.Point.position + directions[GetInd()] * (newPart.Radius * 2.2f * (i+1));
                newPart.transform.position = pos;
                if (_boat.ConnectNewBP(newPart) == false)
                {
                    CLog.LogRed($"[BoatCheat] Cannot connect no more...");
                    Destroy(newPart.gameObject);
                    yield break;
                }
                yield return new WaitForSeconds(delay);
            }
        }

        private BoatPart Spawn()
        {
            var bp = GCon.GOFactory.Spawn<BoatPart>(GlobalConfig.BoatPartID);
            return bp;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                GiveParts();    
            }
        }
    }
}