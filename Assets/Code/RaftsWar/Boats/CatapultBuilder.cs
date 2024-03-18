using System;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class CatapultBuilder : MonoExtended, IBoatPartReceiver
    {
        [SerializeField] private BoatPartsProgBar _progBar;
        [SerializeField] private TowerBlocksBuilder _builder;
        [SerializeField] private TowerBuildingAnimator _buildingAnimator;
        [SerializeField] private RaftAcceptArea _acceptArea;

        private bool _canTake = false;
        private Action _upgradeCallback;
        public Team Team { get; set; }
        public IUnloadPointsManager UnloadPoints { get; private set; }


        public void Init(TowerRaftSettings raftSettings, Team team, Action upgradeCallback)
        {
            Team = team;
            _builder.BuildArea(raftSettings, 0);
            _builder.OnCanUpgrade += Upgrade;
            _upgradeCallback = upgradeCallback;
            _progBar.On();
            _progBar.SetCount(_builder.Stored, _builder.Area);
            var gridOutPusher = new TowerGridOutPusher(_builder, transform);
            gridOutPusher.CheckAndPushOut();
            var unloadPointsManager = new UnloadPointsManager(_builder, 1);
            unloadPointsManager.AddLatestPoints();
            UnloadPoints = unloadPointsManager;
            _buildingAnimator.ShowAt(_builder);
            _acceptArea.SetSquareToLevel(0);
        }
        
        public void AllowTaking(bool allow)
        {
            _canTake = allow;
        }

        private void Upgrade()
        {
            _upgradeCallback.Invoke();
        }

        public void TakeBoatPart(BoatPart part)
        {
            if(part.HasUnit)
                part.Unit.Unit.Hide();
            
            _builder.TakeBuildingBlock(part.GetComponent<IBuildingBlock>());
            _progBar.SetCount(_builder.Stored, _builder.Area);
            _canTake = _builder.CanAccept;
        }

        public bool CanTake()
        {
            return _canTake;
        }

        public Square2D GetAreaSquare()
        {
            return _acceptArea.CurrentSquare;
        }

        public void Disable()
        {
            _progBar.Off();
            _canTake = false;
            _buildingAnimator.Hide();
        }
    }
    
}