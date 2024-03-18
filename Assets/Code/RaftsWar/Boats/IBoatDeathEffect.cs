namespace RaftsWar.Boats
{
    public interface IBoatDeathEffect
    {
        IBoatCaptain Captain { get; set; }
        Boat Boat { get; set; }
        void Die();
    }
}