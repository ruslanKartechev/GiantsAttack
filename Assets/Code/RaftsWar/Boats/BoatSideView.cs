using UnityEngine;

namespace RaftsWar.Boats
{
    public class BoatSideView : MonoBehaviour, IBoatSideView
    {
        [SerializeField] private BoatSideViewParts _parts;

        public void SetParts(BoatSideViewParts part)
        {
            _parts = part;
        }
        
        public void ShowSide()
        {
            _parts.SideOn();
        }

        public void ShowSideAndCorners()
        {
            _parts.AllOn();
        }

        public bool IsSideOn => _parts.side.gameObject.activeSelf;

        public void ShowCorner1()
        {
            _parts.corners[0].gameObject.SetActive(true);
        }

        public void ShowCorner2()
        {
            _parts.corners[1].gameObject.SetActive(true);
        }

        public void HideCorners()
        {
            _parts.corners[0].gameObject.SetActive(false);
            _parts.corners[1].gameObject.SetActive(false);
        }

        public void HideSide()
        {
            // CLog.LogYellow($"{transform.parent.parent.name} Hide side");
            _parts.SideOff();
        }

        public void HideSideAndCorners()
        {
            // CLog.LogYellow($"{transform.parent.parent.name}  {gameObject.name} Hide side and corners");
            _parts.AllOff();
        }

        public void Refresh()
        {
            // CLog.LogWhite($"{transform.parent.parent.name}  {gameObject.name} refreshed");
            _parts.AllOn();
        }

        public void SetView(IBoatViewSettings settings)
        {
            foreach (var rend in _parts.renderers)
                rend.sharedMaterials = settings.SideMaterial;
        }
    }
}