using UnityEngine;

namespace RaftsWar.Boats
{
    [System.Serializable]
    public class EnemyTeam : Team
    {
        [Space(10)] 
        public EnemyAIData aiData;
        public EnemyBehaviourPatternSO behaviourPattern;
        public BoatEnemy EnemyBoat { get; set; }

        
        public void InitEnemy(BoatPartsManager partsManager)
        {
            EnemyBoat.Init(this, partsManager, aiData, behaviourPattern.Pattern);
            Player = EnemyBoat;
        }
    }
}