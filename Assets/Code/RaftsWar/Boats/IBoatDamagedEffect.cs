namespace RaftsWar.Boats
{
    public interface IBoatDamagedEffect
    {
        Boat Boat { get; set; }
        void Play();
        void Stop();
        void Restore(BoatPart part);
    }
}