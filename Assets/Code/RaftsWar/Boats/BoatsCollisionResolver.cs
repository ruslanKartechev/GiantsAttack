using System.Collections.Generic;
using UnityEngine;

namespace RaftsWar.Boats
{
    [DefaultExecutionOrder(1000)]
    public class BoatsCollisionResolver : MonoBehaviour
    {
        private static BoatsCollisionResolver _inst;
        public static BoatsCollisionResolver Inst => _inst;
        
        
        private List<BoatCollisionPair> _pairs = new List<BoatCollisionPair>(4);
        
        private void Awake()
        {
            if (_inst == null)
                _inst = this;
        }

        public void Stop()
        {
            enabled = false;
        }

        public void Activate()
        {
            enabled = true;
        }

        public void AddPair(IBoat b1, IBoat b2)
        {
            foreach (var pair in _pairs)
            {
                if (pair.b1 == b1
                    || pair.b2 == b2) 
                    return;
            }
            // CLog.LogGreen($"Added collision pair {b1.RootPart.transform.parent.name} and " +
            //               $"{b2.RootPart.transform.parent.name}");
            _pairs.Add(new BoatCollisionPair(b1, b2));
        }

        private void Update()
        {
            // if (_pairs.Count > 0)
            // {
            //     CLog.LogBlue($"******* handling {_pairs.Count}");
            // }
            foreach (var pair in _pairs)
            {
                pair.b1.HandleCollisionPushback(pair.b2);
                pair.b2.HandleCollisionPushback(pair.b1);
            }
        }

        private void LateUpdate()
        {
            _pairs.Clear();
        }
    }
}