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
                me.E_GetParts();
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
            
            EU.Label("Interpolate", Color.white, 'l', true);
            GUILayout.BeginHorizontal();
            if (EU.ButtonSmall("I", Color.green))
            {
                me.E_SetInterpolate();   
                EditorUtility.SetDirty(me);
            }
            if (EU.ButtonSmall("E", Color.yellow))
            {
                me.E_SetExtrapolate();   
                EditorUtility.SetDirty(me);
            }
            if (EU.ButtonSmall("N", Color.red))
            {
                me.E_SetNoInterpolate();   
                EditorUtility.SetDirty(me);
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space((10));

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                EU.Label("Projection", Color.white, 'l', true);
                GUILayout.BeginHorizontal();
                if (EU.ButtonSmall("Y", Color.green))
                {
                    me.E_SetProjection();   
                    EditorUtility.SetDirty(me);
                }       
                if (EU.ButtonSmall("N", Color.red))
                {
                    me.E_SetNoProjection();   
                    EditorUtility.SetDirty(me);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                
                GUILayout.BeginVertical();
                EU.Label("Preprocessing", Color.white, 'l', true);
                GUILayout.BeginHorizontal();
                if (EU.ButtonSmall("Y", Color.green))
                {
                    me.E_SetPreprocessAll(true);
                    EditorUtility.SetDirty(me);
                }       
                if (EU.ButtonSmall("N", Color.red))
                {
                    me.E_SetPreprocessAll(false);
                    EditorUtility.SetDirty(me);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            if (EU.ButtonBig($"Set Layer {me.E_layerToSet}", Color.green))
                me.SetLayer();
            if (EU.ButtonBig($"Set Mass", Color.yellow))
                me.E_SetMassAll();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.Space(100);
            if (EU.ButtonBig($"! DELETE !", Color.red))
                me.E_DestroyAll();
            GUILayout.EndHorizontal();
            
            GUILayout.Space((10));
            base.OnInspectorGUI();
            
       
        }
    }
}
#endif