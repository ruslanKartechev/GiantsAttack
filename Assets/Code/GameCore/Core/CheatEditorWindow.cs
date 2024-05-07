#define GUI
#if UNITY_EDITOR
using SleepDev;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GameCore
{
    public class CheatEditorWindow : EditorWindow
    {
        private const int offsetTop = 15;
        private const int offsetLeft = 15;
        private const int offsetWhenSettings = 80;
        private const int spaceDefault = 6;
        private const int offsetBeforeSettings = 100;
        
        private const int fontsizeHeader = 24;
        private const int fontsizeError = 18;
        private const int fontsizeMain = 16;
        private Color headerColor = Color.yellow;
        private Color btnColorGreen = EU.Aqua;
        private Color btnColorRed = EU.HotPink;
        private Color btnColorNeutral = EU.White;
        
        

        private int addedMoney = 100;
        private EditorLevelSwitcher _switcher;
        private LevelsRepository _levelRepository;
        private ReorderableList _levelsList;
        private Vector2 _scroll;
        private int fromTopOffset = 0;
        private int tier_1_y = 140; // controls buttons, info
        private int tier_2_y = 240; // status
        private int tier_3_y = 300; // action buttons and list 
        private Color backgroundColor1 = Color.white * .32f;
        private Color backgroundColor2 = Color.white * .22f;

        private ICheatApplier _cheatApplier;
        
        
        [MenuItem("SleepDev/CheatManager")]
        public static void ShowWindow()
        {
            // This method is called when the user selects the menu item in the Editor
            EditorWindow wnd = GetWindow<CheatEditorWindow>();
            wnd.titleContent = new GUIContent("CheatManager");
        }

#if GUI
        public void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            _cheatApplier = new DummyCheatApplier();
        }
        

        public void OnGUI()
        {
            var rect1 = new Rect(new Vector2(0,0), new Vector2(position.width, 100));
            var rect2 = new Rect(new Vector2(0,100), new Vector2(position.width, 100));
            var rect3 = new Rect(new Vector2(0,200), new Vector2(position.width, 100));
            var rect4 = new Rect(new Vector2(0,300), new Vector2(position.width, 100));
            EditorGUI.DrawRect(rect1, backgroundColor1);
            EditorGUI.DrawRect(rect2, backgroundColor2);
            EditorGUI.DrawRect(rect3, backgroundColor1);
            EditorGUI.DrawRect(rect4, backgroundColor2);
            ShowHeader(rect1);
            ShowAddLevelControls(rect2);
            ShowMoney(rect3);
            ShowButtons(rect4);
        }

        private void ShowHeader(Rect rect)
        {
            GUILayout.BeginArea(rect);
            GUILayout.Space(20);
            EU.Label("Cheat Manager", headerColor, fontsizeHeader);
            EU.Label("[implementation is game specific]", headerColor, fontsizeMain);

            GUILayout.EndArea();
        }

        private void ShowAddLevelControls(Rect rect)
        {
            GUILayout.BeginArea(EU.SetRectX(rect, offsetLeft));
            GUILayout.Space(spaceDefault);
            GUILayout.BeginHorizontal();
            if (EU.BtnMid2("Win", btnColorGreen, fontsizeMain))
                _cheatApplier.LevelWin();
            
            GUILayout.Space(spaceDefault);
            if (EU.BtnMid2("Fail", btnColorRed, fontsizeMain))
                _cheatApplier.LevelFail();
            
            GUILayout.Space(spaceDefault);
            if (EU.BtnMid2("Reload", btnColorNeutral, fontsizeMain))
                _cheatApplier.Reload();
            
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (EU.BtnMid2("Freeze", btnColorNeutral, fontsizeMain))
            {
                _cheatApplier.Freeze();
            }
            GUILayout.Space(spaceDefault);
            if (EU.BtnMid2("Play", btnColorNeutral, fontsizeMain))
            {
                _cheatApplier.Play();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
   
        private void ShowMoney(Rect rect)
        {
            GUILayout.BeginArea(EU.SetRectX(rect, offsetLeft));
            GUILayout.Space(spaceDefault);
            EU.Label($"Added or remove money", headerColor, fontsizeMain);
            GUILayout.Space(spaceDefault);
            GUILayout.BeginHorizontal();
            if (EU.BtnMid2($"+ {addedMoney}", btnColorGreen, fontsizeMain))
            {
                _cheatApplier.AddMoney(addedMoney);
            }            
            GUILayout.Space(2 * spaceDefault);
            if (EU.BtnMid2($"- {addedMoney}", btnColorRed, fontsizeMain))
            {
                _cheatApplier.RemoveMoney(addedMoney);
            }
            GUILayout.Space(5);
            addedMoney = EditorGUILayout.IntField(addedMoney, GUI.skin.textField, GUILayout.Width(150));
            if(addedMoney < 0)
                addedMoney = 0;
            
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        public void ShowButtons(Rect rect)
        {
            GUILayout.BeginArea(EU.SetRectX(rect, offsetLeft));
            GUILayout.Space(spaceDefault);
            GUILayout.BeginHorizontal();
            if (EU.BtnMidWide2($"Kill Enemies", btnColorGreen, fontsizeMain))
                _cheatApplier.AddMoney(addedMoney);
            GUILayout.Space(spaceDefault);
            if (EU.BtnMidWide2("Kill Player", btnColorRed, fontsizeMain))
                _cheatApplier.RemoveMoney(addedMoney);
            GUILayout.EndHorizontal();

            GUILayout.Space(spaceDefault);

            GUILayout.BeginHorizontal();
            if (EU.BtnMidWide2("Next stage", btnColorGreen, fontsizeMain))
                _cheatApplier.RemoveMoney(addedMoney);
            GUILayout.EndHorizontal();
            
            GUILayout.EndArea();
        }
#endif

    }
}
#endif
