using System.Collections;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class BoatDeathEffect : MonoBehaviour, IBoatDeathEffect
    {
        [SerializeField] private SinkingPartsController _sinkingPartsController;
        
        public IBoatCaptain Captain { get; set; }
        public Boat Boat { get; set; }

        public void Die()
        {
            StartCoroutine(Dying());
        }

        private IEnumerator Dying()
        {
            const int framesSkipped = 2;
            Captain.DieRagdoll();
            for(var i = 0; i < framesSkipped; i ++)
                yield return null;
            foreach (var part in Boat.Parts)
            {
                part.BreakIntoPieces();
                // PushPart(part.gameObject);
                for(var i = 0; i < framesSkipped; i ++)
                    yield return null;
            }
            Boat.RootPart.BreakIntoPieces();
            
            void PushPart(GameObject go)
            {
                
                var sinkPart = go.GetComponent<SinkingAnimator>();
                sinkPart.Rb.transform.parent = null;
                sinkPart.Coll.enabled = true;
                sinkPart.Coll.isTrigger = false;
                sinkPart.Rb.isKinematic = false;
                var rand = UnityEngine.Random.insideUnitCircle;
                var vec = new Vector3(rand.x, 1f, rand.y) * 30;
                sinkPart.Rb.AddForce(vec, ForceMode.Impulse);
                sinkPart.Rb.AddTorque(vec, ForceMode.Impulse);
                
                _sinkingPartsController.Animators.Add(sinkPart);
            }
            _sinkingPartsController.ActivateAfterDelay(GlobalConfig.SinkingDelay);
        }

    }
}