using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class BrokenBoatPart : MonoBehaviour
    {
        private static List<Vector3> PushForces = new()
        {
            new(0,0,1),
            new(1,0,-1),
            new(-1,0,-1),
            new(-1,0,1),
            new(.5f,0,.5f),
            new(.8f,0,-.5f),
            new(.7f,0,.85f),
            new(.5f,0,-.5f),
        };

        private static int _forceInd;
        [SerializeField] private SinkingConfig _config;
        [SerializeField] private List<SinkingAnimator> _sinking;
        [SerializeField] private List<Rigidbody> _rigidbodies;
        [SerializeField] private List<Renderer> _renderers;


        public void SetView(IBoatViewSettings viewSettings)
        {
            foreach (var rend in _renderers)
                rend.sharedMaterials = viewSettings.SideMaterial;
        }
        
        public void Push()
        {
            var force = GlobalConfig.BoatBreakForce;
            foreach (var rb in _rigidbodies)
            {
                var vec = rb.transform.localPosition
                          + PushForces[_forceInd]
                          + Vector3.up;
                vec *= force;
                rb.AddForce(vec, ForceMode.Impulse);
                rb.AddTorque(vec, ForceMode.Impulse);
                _forceInd++;
                if (_forceInd == PushForces.Count)
                    _forceInd = 0;
            }

            foreach (var sinking in _sinking)
            {
                sinking.Config = _config;
                sinking.IsActive = true;
            }
        }
        
    }
}