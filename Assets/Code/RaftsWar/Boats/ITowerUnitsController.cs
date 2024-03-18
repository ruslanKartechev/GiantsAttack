using System.Collections.Generic;
using UnityEngine;

namespace RaftsWar.Boats
{
    public interface ITowerUnitsController
    {
        void UpdateSettings(TowerLevelSettings settings);
        public void SetOptionalPoints(List<Transform> optionalSpawnPoints);
        public void SpawnUnitsAtPoints(List<Transform> newSpawnPoints);
        void ActivateAttack();
        void StopAttack();
        void KillUnits();
        ITower Tower { get; set; }
        void TakeUnit(BoatUnitController partUnit);
        void UnloadFromStash();
    }
}