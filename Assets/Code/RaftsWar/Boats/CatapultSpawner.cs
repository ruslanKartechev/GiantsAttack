using DG.Tweening;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class CatapultSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _catapultSpawn;

        public ICatapult CatapultInstance { get; private set; }
        public Transform SpawnPoint
        {
            get => _catapultSpawn;
            set => _catapultSpawn = value;
        }


        public bool CheckCondition(int towerLevel)
        {
            return towerLevel >= 4;
        }

        public void SpawnCatapult(Team team)
        {
            CLog.LogGreen($"[Tower] Spawning catapult");
            var catapult = GCon.GOFactory.Spawn<ICatapult>(GlobalConfig.CatapultID);
            catapult.SetPosition(SpawnPoint);
            catapult.Init(team);
            catapult.Show(true);
            CatapultInstance = catapult;
        }

        #if UNITY_EDITOR
        public void E_Spawn(Team team)
        {
            if (_catapultSpawn == null)
            {
                CLog.LogRed($"[CatapultSpawner] Catapult spawn not set");
                return;
            }
            var catGo = GameObjectFactory.GetDefault().SpawnAsPrefab(GlobalConfig.CatapultID);
            var catapult = catGo.GetComponent<Catapult>();
            catapult.transform.CopyPosRot(_catapultSpawn);
            catapult.Init(team);
            catGo.transform.parent = _catapultSpawn;
            CatapultInstance = catapult;
        }
        #endif
    }
}