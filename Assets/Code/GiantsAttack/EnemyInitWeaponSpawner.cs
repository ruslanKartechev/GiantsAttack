using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class EnemyInitWeaponSpawner : MonoBehaviour, IEnemyInitiator
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private TransformData _positionData;

        #if UNITY_EDITOR
        [Space(10), Header("Editor")]
        public GameObject enemyGo;

        [ContextMenu("E_Spawn")]
        public void E_Spawn()
        {
            if (enemyGo == null)
                return;
            if (enemyGo.TryGetComponent<IMonster>(out var enemy))
            {
                var parent = enemy.Hand;
                var instance = UnityEditor.PrefabUtility.InstantiatePrefab(_prefab, parent) as GameObject;
                _positionData.SetLocal(instance.transform);
            }
        }
        
        #endif
        
        public void InitEnemy(IMonster enemy)
        {
            var parent = enemy.Hand;
            var instance = Instantiate(_prefab,parent);
            _positionData.SetLocal(instance.transform);
        }
    }
}