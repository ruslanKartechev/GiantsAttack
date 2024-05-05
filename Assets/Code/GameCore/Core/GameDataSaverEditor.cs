#if UNITY_EDITOR
using SleepDev;
using UnityEditor;
using UnityEngine;

namespace GameCore.Core
{
    [CustomEditor(typeof(GameDataSaver))]
    public class GameDataSaverEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as GameDataSaver;
            GUILayout.Space(10);
            if(EU.BtnMid2("Path", Color.cyan))
                me.LogPath();   
            if(EU.BtnMid2("Clear", Color.red))
                me.Clear();
            GUILayout.Space(10);

            if(EU.BtnMid2("Load", Color.green))
                me.Load();
            if(EU.BtnMid2("Save", Color.green))
                me.Save();
            GUILayout.Space(10);
        }
    }
}
#endif