using UnityEngine;

namespace GiantsAttack
{
    [CreateAssetMenu(menuName = "SO/EnemyID", fileName = "EnemyID", order = 0)]
    public class EnemyID : ScriptableObject
    {
        public string id;
    }
}