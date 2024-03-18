using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public interface ITowerRepository
    {
        int Level { get;}
        float Progress { get; set; }
        void Init(int currentLevel, float progress);
        void Upgrade();
        bool CanUpgrade();
        SpriteFillIcon NextTowerSprite();
        GameObject GetCurrentPrefab();
        ITower GetCurrentPrefabTower();
    }
}