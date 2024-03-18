using UnityEngine;

namespace RaftsWar.Boats
{
    public class BuildingBlock_Test : MonoBehaviour, IBuildingBlock
    {
        [SerializeField] private Vector3 _size;
        
        public Vector3 CellSize => _size;

        public Transform Transform => transform;
        
        public void SetScale(float scale)
        {
            transform.localScale = _size * scale;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Take()
        { }

        public void SetSide(ESquareSide side)
        {
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }
       
        public void HideRampSide(ESquareSide side)
        { }

        public void SetYScale(float yScale)
        { }

        public void Destroy()
        {
            gameObject.SetActive(false);
        }
    }
}