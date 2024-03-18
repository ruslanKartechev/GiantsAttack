using UnityEngine;

namespace RaftsWar.Boats
{
    [CreateAssetMenu(menuName = "SO/EnemyBehaviourPattern", fileName = "EnemyBehaviourPatternSO", order = 0)]
    public class EnemyBehaviourPatternSO : ScriptableObject
    {
        [SerializeField] private EnemyBehaviourPattern _pattern;
        public EnemyBehaviourPattern Pattern => _pattern;
    }
}