using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class ChoppedMeshSpawner : MonoBehaviour
    {
        [SerializeField] private float _pushForce;
        [SerializeField] private List<Data> _parts;
        [SerializeField] private ChoppedMesh _prefab;
        [SerializeField] private List<GameObject> _disableTargets;

        [System.Serializable]
        private class Data
        {
            public Transform refBone;
            public int partChildIndex;
        }

        public void Play()
        {
            PlayWithForce(_pushForce);
        }
        
        public void PlayWithForce(float force)
        {
            foreach (var go in _disableTargets)
                go.SetActive(false);
            var instance = Instantiate(_prefab, transform.position, transform.rotation, transform);
            for (var i = 0; i < _parts.Count; i++)
            {
                var rb = instance.Rbs[i];
                rb.transform.CopyPosRot(_parts[i].refBone);
                rb.isKinematic = false;
                rb.AddForce(rb.transform.localPosition.normalized * force, ForceMode.VelocityChange);
            }
        }

        
        
#if UNITY_EDITOR
        
#endif
        
    }
}