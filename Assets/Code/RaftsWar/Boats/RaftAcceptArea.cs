using System.Collections.Generic;
using UnityEngine;
using SleepDev;

namespace RaftsWar.Boats
{
    public class RaftAcceptArea : MonoBehaviour
    {
        [SerializeField] private List<SquareData> _squareData;
        public Square2D CurrentSquare { get; private set; }
        
        [System.Serializable]
        public class SquareData
        {
            public Transform topLeft;
            public Transform topRight;
            public Transform botRight;
            public Transform botLeft;

            public Square2D GetSquare()
            {
                return new Square2D(topLeft.position.ToXZPlane(), 
                    topRight.position.ToXZPlane(),
                    botLeft.position.ToXZPlane(),
                    botRight.position.ToXZPlane());
            }
        }
        
        public void SetSquareToLevel(int level)
        {
            CurrentSquare = _squareData[level].GetSquare();
        }
        
        #if UNITY_EDITOR
        public bool g_draw;
        public float g_offsetY = 2;
        public Color g_color;
        [Space(10)]
        [Header("Current index")]
        public int test_index;
        [Space(20)]
        [Header("Dimensions")]
        public float e_left;
        public float e_right;
        public float e_top;
        public float e_bot;
        
        private void OnDrawGizmos()
        {
            if (g_draw == false)
                return;
            E_DrawSquareGizmos();
        }

        [ContextMenu("E_DrawSquareGizmos")]
        public void E_DrawSquareGizmos()
        {
            if (CurrentSquare == null)
                return;
            var p1 = CurrentSquare.TopLeftCorner.ToVec3XZ(g_offsetY);
            var p2 = CurrentSquare.TopRightCorner.ToVec3XZ(g_offsetY);
            var p3 = CurrentSquare.BotRightCorner.ToVec3XZ(g_offsetY);
            var p4 = CurrentSquare.BotLeftCorner.ToVec3XZ(g_offsetY);
            var oldColor = Gizmos.color;
            Gizmos.color = g_color;
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p4, p1);
            Gizmos.color = oldColor;
        }

        [ContextMenu("Draw at test index")]
        public void E_DrawTest()
        {
            test_index = Mathf.Clamp(test_index, 0, _squareData.Count - 1);
            CurrentSquare = _squareData[test_index].GetSquare();
           
            var p1 = CurrentSquare.TopLeftCorner.ToVec3XZ(g_offsetY);
            var p2 = CurrentSquare.TopRightCorner.ToVec3XZ(g_offsetY);
            var p3 = CurrentSquare.BotRightCorner.ToVec3XZ(g_offsetY);
            var p4 = CurrentSquare.BotLeftCorner.ToVec3XZ(g_offsetY);
            var time = 3f;
            Debug.DrawLine(p1, p2, g_color, time);
            Debug.DrawLine(p2, p3, g_color, time);
            Debug.DrawLine(p3, p4, g_color, time);
            Debug.DrawLine(p4, p1, g_color, time);
        }

        [ContextMenu("E_BuildAtIndex")]
        public void E_BuildAtIndex()
        {
            var data = _squareData[test_index];
            data.topLeft.localPosition = new Vector3(e_left, 0f, e_top);
            data.topRight.localPosition = new Vector3(e_right, 0f, e_top);
            data.botRight.localPosition = new Vector3(e_right, 0f, e_bot);
            data.botLeft.localPosition = new Vector3(e_left, 0f, e_bot);
            UnityEditor.EditorUtility.SetDirty(this);
            E_DrawTest();
        }

        public void E_NextInd()
        {
            test_index++;
            test_index = Mathf.Clamp(test_index, 0, 4);
        }
        
        public void E_PrevInd()
        {
            test_index--;
            test_index = Mathf.Clamp(test_index, 0, 4);
        }

#endif
    }
}