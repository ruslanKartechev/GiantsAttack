using UnityEngine;

namespace RaftsWar.Boats
{
    public class CatapultMagazineReceiver : MonoBehaviour, IBoatPartReceiver
    {
        private RaftAcceptArea _acceptArea;
        private CatapultMagazine _magazine;

        public void Init(Team team, CatapultMagazine magazine, RaftAcceptArea acceptArea)
        {
            Team = team;
            _magazine = magazine;
            _acceptArea = acceptArea;
            _acceptArea.SetSquareToLevel(0);
        }

        public Team Team { get; private set; }
        
        public void TakeBoatPart(BoatPart part)
        {
            _magazine.Take(part);
        }

        public bool CanTake()
        {
            return true;
        }

        public Square2D GetAreaSquare()
        {
            return _acceptArea.CurrentSquare;
        }
    }
}