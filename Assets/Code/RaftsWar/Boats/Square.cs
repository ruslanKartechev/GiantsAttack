using UnityEngine;

namespace RaftsWar.Boats
{
    public class Square
    {
        /// <summary>
        /// Created a square in XZ plane with given corners (world or local)
        /// </summary>
        public Square(Vector3 topLeftCorner, Vector3 topRightCorner, Vector3 botLeftCorner, Vector3 botRightCorner)
        {
            TopLeftCorner = topLeftCorner;
            TopRightCorner = topRightCorner;
            BotLeftCorner = botLeftCorner;
            BotRightCorner = botRightCorner;
            Center = new Vector3((botLeftCorner.x + botRightCorner.x) / 2f,
                (botLeftCorner.y + topRightCorner.y) / 2f,
                (botLeftCorner.z + topLeftCorner.z) / 2f);
            
            Width = botRightCorner.x - botLeftCorner.x;
            Height = topRightCorner.z - botRightCorner.z;
            
            // #if UNITY_EDITOR
            // Debug.DrawLine(Center, topLeftCorner, Color.black, 10f);
            // Debug.DrawLine(Center, topRightCorner, Color.black, 10f);
            // Debug.DrawLine(Center, botLeftCorner, Color.black, 10f);
            // Debug.DrawLine(Center, botRightCorner, Color.black, 10f);
            // #endif
        }

        public Vector3 TopLeftCorner { get; private set; }
        public Vector3 TopRightCorner { get; private set; }
        public Vector3 BotLeftCorner { get; private set; }
        public Vector3 BotRightCorner { get; private set; }
        public Vector3 Center { get; private set; }
        
        public float Width { get; private set; }
        public float Height { get; private set; }
    }
}