using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class EnemySpawner : MonoBehaviour, IEnemySpawner
    {
        [SerializeField] private float _scale = 1f;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private List<ArmorDataSo> _armorData;
        [Header("Prespawned")] 
        [SerializeField] private bool _usePrespawned;
        [SerializeField] private GameObject _preSpawned;
        
        public IMonster SpawnEnemy(EnemyID id)
        {
            IMonster result = null;
            if (_usePrespawned)
            {
                result = _preSpawned.GetComponent<IMonster>();
            }
            else
            {
                var prefab = Resources.Load<GameObject>($"Prefabs/Enemies/{id.id}");
                var inst = Instantiate(prefab, _spawnPoint);
                inst.transform.CopyPosRot(_spawnPoint);
                inst.transform.localScale = Vector3.one * _scale;
                result = inst.GetComponent<IMonster>();
            }
            result.SetArmorData(_armorData);
            return result;
        }
    }
}