using UnityEngine;

namespace RaftsWar.Boats
{
    public interface IBuildingBlock
    {
        Vector3 CellSize { get; }
        Transform Transform { get; }
        void SetScale(float scale);
        void Hide();
        void Show();
        void Take();
        void SetSide(ESquareSide side);
        void HideRampSide(ESquareSide side);
        void SetYScale(float yScale);
        void Destroy();
    }
}