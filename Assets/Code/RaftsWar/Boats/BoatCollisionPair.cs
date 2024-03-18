namespace RaftsWar.Boats
{
    public struct BoatCollisionPair
    {
        public IBoat b1;
        public IBoat b2;
        public BoatCollisionPair(IBoat b1, IBoat b2)
        {
            this.b1 = b1;
            this.b2 = b2;
        }
    }
}