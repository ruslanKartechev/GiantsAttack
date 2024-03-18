using System.Collections.Generic;
using UnityEngine;

namespace RaftsWar.Boats
{
    public abstract class TowerUnit : MonoBehaviour, ITeamMember
    {
        protected UnitViewSettings _viewSettings;
        protected static List<float> Angles = new() { 0f, 45, 90f, 135f, 180, 225, 270, 325 };

        public Team Team { get; set; }

        public abstract void Idle();
        public abstract void Fire(ITarget target);
        public abstract void Init(Team team);
        public abstract void Animate();
        public abstract void SetView(UnitViewSettings unitViewSettings);
        public abstract void LookAt(Transform at, float time);
        public abstract float FireRate { get; set; }
        public abstract float Damage { get; set; }
        public abstract void Hide();
        public abstract void Kill();

        public void RandomizeRotation()
        {
            transform.localRotation = Quaternion.Euler(
                new Vector3(0, Angles[UnityEngine.Random.Range(0, Angles.Count-1)], 0));
        }
    }
}