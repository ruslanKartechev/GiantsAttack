using GameCore.UI;
using UnityEngine;

namespace GiantsAttack
{
    public interface IBullet
    {
        void SetRotation(Quaternion rotation);
        void Launch(Vector3 from, Vector3 direction, float speed, DamageArgs args, 
            IHitCounter counter, IDamageHitsUI hitsUI);
        void LaunchBlank(Vector3 from, Vector3 direction, float speed);
        void Scale(float scale);
    }
}