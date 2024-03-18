namespace RaftsWar.Boats
{
    [System.Serializable]
    public class EnemyBehaviourPattern
    {
        public float damagedRunDistance = 5f;
        public int targetRaftsCount;
        public bool goForClosestRaft;
    }
}