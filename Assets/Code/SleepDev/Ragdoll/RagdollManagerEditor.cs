#if UNITY_EDITOR
using SleepDev.Utils.EditorUtils;
using UnityEditor;
using UnityEngine;

namespace SleepDev.Ragdoll
{
    [CustomEditor(typeof(Ragdoll))]
    public class RagdollManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var me = target as Ragdoll;

            EU.Label("Ragdoll", Color.white, 'c', true);
            GUILayout.BeginHorizontal();
            if (EU.ButtonMidSize("Get", Color.cyan))
            {
                me.GetAllParts();
                EditorUtility.SetDirty(me);
            }
            if (EU.ButtonMidSize("On", Color.green))
            {
                 me.Activate();   
                 EditorUtility.SetDirty(me);
            }
            if (EU.ButtonMidSize("Off", Color.red))
            {
                me.Deactivate();   
                EditorUtility.SetDirty(me);
            }
            if (EU.ButtonMidSize("Colls", Color.yellow))
            {
                me.SetCollidersOnly();
                EditorUtility.SetDirty(me);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space((10));
            
            EU.Label("Interpolate", Color.white, 'c', true);
            GUILayout.BeginHorizontal();
            if (EU.ButtonSmall("I", Color.green))
            {
                me.SetAllInterpolate();   
                EditorUtility.SetDirty(me);
            }
            if (EU.ButtonSmall("E", Color.yellow))
            {
                me.SetAllExtrapolate();   
                EditorUtility.SetDirty(me);
            }
            if (EU.ButtonSmall("N", Color.red))
            {
                me.SetAllNoInterpolate();   
                EditorUtility.SetDirty(me);
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space((10));

            EU.Label("Projection", Color.white, 'c', true);
            GUILayout.BeginHorizontal();
            if (EU.ButtonSmall("Y", Color.green))
            {
                me.SetProjection();   
                EditorUtility.SetDirty(me);
            }       
            if (EU.ButtonSmall("N", Color.red))
            {
                me.SetNoProjection();   
                EditorUtility.SetDirty(me);
            }
            GUILayout.EndHorizontal();
            
            EU.Label("Preprocessing", Color.white, 'c', true);
            GUILayout.BeginHorizontal();
            if (EU.ButtonSmall("Y", Color.green))
            {
                me.SetAllPreprocess(true);
                EditorUtility.SetDirty(me);
            }       
            if (EU.ButtonSmall("N", Color.red))
            {
                me.SetAllPreprocess(false);
                EditorUtility.SetDirty(me);
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            if (EU.ButtonBig($"Set Layer {me.layerToSet}", Color.green))
                me.SetLayer();
            
            GUILayout.Space((10));

            GUILayout.BeginHorizontal();
            GUILayout.Space(100);
            if (EU.ButtonBig($"! CLEAR !", Color.red))
                me.DestroyAll();
            GUILayout.EndHorizontal();
            
            GUILayout.Space((10));
            base.OnInspectorGUI();
            
       
        }
    }
}
#endif