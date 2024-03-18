using UnityEngine;

namespace RaftsWar.Boats
{
    public interface ICatapultProjectile
    {
        void Init(Team team);
        void Launch(Transform fromPoint, ITarget target, float speed, float damage);
        void Hide();
        void Reset();
        GameObject Go { get; }
    }
}