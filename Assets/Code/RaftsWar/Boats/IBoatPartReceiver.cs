namespace RaftsWar.Boats
{
    public interface IBoatPartReceiver : ITeamMember
    {
        void TakeBoatPart(BoatPart part);
        bool CanTake();
        Square2D GetAreaSquare();
    }
}