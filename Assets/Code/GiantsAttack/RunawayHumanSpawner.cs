using System.Collections;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class RunawayHumanSpawner : StageListener
    {
        [SerializeField] private float _spawnDelay = 0f;
        [SerializeField] private float _scale = 1f;
        [SerializeField] private List<SpawnData> _spawnData;
        [SerializeField] private List<RunawayHuman> _humanPrefabs;
        
        public static byte PrefabInd = 0;
        
        [System.Serializable]
        private class SpawnData
        {
            public float delay;
            public Transform spawnPoint;
            public Transform endPoint;
            public float moveTime;
        }

        public override void OnActivated()
        {
            StartCoroutine(Spawning()); 
        }

        public override void OnStopped()
        {
        }

        public override void OnCompleted()
        {
        }

        private IEnumerator Spawning()
        {
            yield return new WaitForSeconds(_spawnDelay);
            foreach (var data in _spawnData)
            {
                var d = data;
                Delay(() => { Spawn(d);}, data.delay);
            }
        }

        private void Spawn(SpawnData data)
        {
            if (PrefabInd >= _humanPrefabs.Count - 1)
                PrefabInd = 0;
            var ind = PrefabInd;
            PrefabInd++;
            var prefab = _humanPrefabs[ind];
            var instance = Instantiate(prefab, transform);
            instance.transform.localScale = new Vector3(_scale,_scale,_scale);
            instance.transform.CopyPosRot(data.spawnPoint);
            instance.MoveToPoint(data.endPoint, data.moveTime, instance.Hide);
        }

    }
}