using System.Collections.Generic;
using UnityEngine;

namespace RaftsWar.Boats
{
    [System.Serializable]
    public class BoatSideViewParts
    {
        public Transform side;
        public List<Transform> corners = new List<Transform>();
        public List<Renderer> renderers = new List<Renderer>();

        public void AllOn()
        {
            foreach (var ct in corners)
                ct.gameObject.SetActive(true);
            side.gameObject.SetActive(true);
        }
        
        public void AllOff()
        {
            foreach (var ct in corners)
                ct.gameObject.SetActive(false);
            side.gameObject.SetActive(false);
        }

        public void SideOn()
        {
            side.gameObject.SetActive(true);
        }
        
        public void SideOff()
        {
            side.gameObject.SetActive(false);
        }


    }
}