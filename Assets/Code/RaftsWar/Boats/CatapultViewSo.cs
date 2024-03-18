using UnityEngine;

namespace RaftsWar.Boats
{
    [CreateAssetMenu(menuName = "SO/CatapultViewSo", fileName = "CatapultViewSo", order = 0)]
    public class CatapultViewSo : ScriptableObject
    {
        public CatapultViewSettings settings;
    }
}