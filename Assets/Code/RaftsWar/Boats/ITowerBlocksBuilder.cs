using UnityEngine;
using System;
using System.Collections.Generic;


namespace RaftsWar.Boats
{
    public interface ITowerBlocksBuilder
    {
        event Action OnCanUpgrade;
        void BuildArea(TowerRaftSettings buildingSettings, int level);
        void TakeBuildingBlock(IBuildingBlock bb);
        void BuildUpToLevel(Team team, int level);
        List<SquareGrid> Grids { get; }
        SquareGrid LatestGrid { get; }
        Vector3 CellSize { get; }
        Transform Root { get; }
        AreaBlock CurrentArea { get; }
        int Area { get; }
        int Stored { get; }
        bool CanAccept { get; }
        void Destroy();
        void HideAtLevel(int level);
    }
}