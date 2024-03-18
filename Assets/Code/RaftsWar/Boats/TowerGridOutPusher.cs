using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class TowerGridOutPusher
    {
        private ITowerBlocksBuilder _builder;
        private Transform _root;

        public TowerGridOutPusher(ITowerBlocksBuilder builder, Transform root)
        {
            _root = root;
            _builder = builder;
        }
        
        public void CheckAndPushOut()
        {
            var square = _builder.LatestGrid.WorldSquare;
            var center = square.Center;
            var size = new Vector3(square.Width/2f, 
                10f, square.Height/2f);
// #if UNITY_EDITOR
//             var ul = center;
//             ul.x -= size.x;
//             ul.z += size.z;
//             var ur = center;
//             ur.x += size.x;
//             ur.z += size.z;
//             var ll = center;
//             ll.x -= size.x;
//             ll.z -= size.z;
//             Debug.DrawLine(ul + Vector3.up, ur + Vector3.up, Color.red, 10f);
//             Debug.DrawLine(ul + Vector3.up, ll + Vector3.up, Color.red, 10f);
// #endif            
            var colls = Physics.OverlapBox(center, size, 
                Quaternion.identity, GlobalConfig.DamageableMask);
            // CLog.LogRed($"********** Overlaps count: {colls.Length}");
            if (colls.Length == 0)
                return;
            
            var pushedBoats = new List<IBoat>(2);
            foreach (var coll in colls)
            {
                // CLog.LogRed($"********* TRYING COLLIDER {coll.gameObject.name}");
                if (coll.gameObject.TryGetComponent<BoatPart>(out var bp))
                {
                    if (bp.HostBoat == null)
                    {
                        // CLog.LogYellow($"{coll.gameObject.name} HOST IS NULL");
                        continue;
                    }

                    if (pushedBoats.Contains(bp.HostBoat))
                    {
                        // CLog.LogYellow($"{coll.gameObject.name} Already been pushed");
                        continue;
                    }
                    // if(bp.HostBoat == null || pushedBoats.Contains(bp.HostBoat))
                        // continue;
                    // CLog.LogGreen($"[PushOut] Pushing out a boat {bp.gameObject.name}");
                    pushedBoats.Add(bp.HostBoat);
                    bp.HostBoat.PushOutFromBlockedArea(square, _root);
                    // CLog.LogRed($"************* Push boat end");
                }
                // else
                // {
                //     CLog.LogYellow($"********* NO BOAT PART ON  {coll.gameObject.name}");
                // }
            }
        }
    }
}