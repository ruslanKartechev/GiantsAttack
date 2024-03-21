using UnityEngine;

namespace GiantsAttack
{
    public interface IHelicopterMover
    {
        MoverSettings Settings { get; set; }
        void SetPath(CircularPath path, Transform lookAtTarget);
        void BeginMovement();
        void StopMovement();
    }
}