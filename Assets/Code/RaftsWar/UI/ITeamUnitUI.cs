using UnityEngine;

namespace RaftsWar.UI
{
    public interface ITeamUnitUI
    {
        void Die();
        void SetName(string name, Color color);
        // Set value immediately
        void SetHealth(float health);
        // animate to given value
        void UpdateHealth(float health);
        void Show();
    }
}