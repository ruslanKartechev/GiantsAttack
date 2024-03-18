using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    [System.Serializable]
    public class Square2D
    {
        public Vector2 TopLeftCorner { get; private set; }
        public Vector2 TopRightCorner { get; private set; }
        public Vector2 BotLeftCorner { get; private set; }
        public Vector2 BotRightCorner { get; private set; }
        private float _abab;
        private float _adad;
        private Vector2 _ab;
        private Vector2 _ad;
        public Square2D()
        { }
        
        public Square2D(Vector2 topLeftCorner, Vector2 topRightCorner, Vector2 botLeftCorner, Vector2 botRightCorner)
        {
            TopLeftCorner = topLeftCorner; // A
            TopRightCorner = topRightCorner; // B
            BotLeftCorner = botLeftCorner; // C
            BotRightCorner = botRightCorner; // D
            _ab = topRightCorner - topLeftCorner;
            _ad = botLeftCorner - topLeftCorner;
            _abab = Vector2.Dot(_ab, _ab);
            _adad = Vector2.Dot(_ad, _ad);
           
        }
        
        // (0 < AM * AB < AB * AB)
        // (0 < AM * AD < AD * AD)
        public bool CheckIfInside(Vector2 point)
        {
            // CLog.Log($"Checking inside for {point}, AB* {_abab}, AD* {_adad}");
            var am = point - TopLeftCorner;
            var amab = Vector2.Dot(am, _ab);
            var amad = Vector2.Dot(am, _ad);
            return (0 < amab && amab < _abab)
                   && (0 < amad && amad < _adad);
        }
    }
}