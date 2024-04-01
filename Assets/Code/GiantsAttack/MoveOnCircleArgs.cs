namespace GiantsAttack
{
    [System.Serializable]
    public struct MoveOnCircleArgs
    {
        public float angleMoveSpeed;
        public float moveToStartPointSpeed;
        public float rotateToStartAngleSpeed;
        public bool moveToStartPoint;
        public bool refreshAngleOnStart;

    }
}