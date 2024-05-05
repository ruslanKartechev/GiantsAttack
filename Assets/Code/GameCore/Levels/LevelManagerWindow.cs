#define GUI
#if UNITY_EDITOR
using System.Collections.Generic;
using SleepDev;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GameCore
{
    public class LevelManagerWindow : EditorWindow
    {
        private const int LabelFontSize = 24;
        private const int ErrorFontSize = 18;
        private const int TopOffset = 15;
        private const int DefaultFieldSpace = 5;
        private const int mainFontSize = 16;
        private const int smallFontSize = 12;

        private const int levelsFontSize = 10;
        private const int ListElementMaxHeight = 60;
        private const int leftRectOffset = 15;
        private const int settingsOnOffset = 80;
        private const int settingRectHeight = 100;

        private static bool showSettings;
        private EditorLevelSwitcher _switcher;
        private LevelsRepository _levelRepository;
        private ReorderableList _levelsList;
        private Vector2 _scroll;
        private int fromTopOffset = 0;
        private int tier_1_y = 140; // controls buttons, info
        private int tier_2_y = 240; // status
        private int tier_3_y = 300; // action buttons and list 
        private Color backgroundColor1 = Color.white * .4f;
        private Color backgroundColor2 = Color.white * .3f;

        [MenuItem("SleepDev/LevelManager")]
        public static void ShowWindow()
        {
            // This method is called when the user selects the menu item in the Editor
            EditorWindow wnd = GetWindow<LevelManagerWindow>();
            wnd.titleContent = new GUIContent("LevelManager");
        }

#if GUI
        public void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            if (_switcher == null || _switcher.LevelRepository == null)
            {
                var possiblePaths = new List<string>()
                {
                    $"{nameof(LevelsRepository)}",
                    $"Levels/{nameof(LevelsRepository)}",
                    $"Config/{nameof(LevelsRepository)}",
                    $"Config/Levels/{nameof(LevelsRepository)}"
                };
                foreach (var path in possiblePaths)
                {
                    var repo = Resources.Load<LevelsRepository>(path);
                    if (repo != null)
                    {
                        _levelRepository = repo;
                        _switcher = new EditorLevelSwitcher(_levelRepository);
                    }
                }
            }

            if (_levelsList == null && _switcher != null)
                BuildList();
        }
        
        
        private void BuildList()
        {
            var repo = _levelRepository;
            var serObj = new SerializedObject(_switcher.LevelRepository);
            var serProp = serObj.FindProperty("_levels");
            const int elementWith = 180;
            
            _levelsList = new ReorderableList(serObj, serProp, true, true, true, true);
            _levelsList.list = repo.Levels;
            _levelsList.drawHeaderCallback = (rect) =>
            {
                EU.LabelRect(rect, "LEVELS", mainFontSize, Color.white, 'c', true);
            };
            _levelsList.elementHeight = ListElementMaxHeight;
            _levelsList.drawElementCallback = (rect, index, active, focused) =>
            {
                if (index >= repo.Count || index < 0)
                    return;
                var posRect = new Rect(rect.position.x, rect.position.y, elementWith, 20);
                EU.LabelRect(posRect, $"Order {index}", levelsFontSize + 2, Color.white, 'c', true);
                var data = repo.Levels[index];
                posRect.y += 20;
                data.LevelName = EditorGUI.TextField(posRect, data.LevelName);
                posRect.y += 20;
                data.SceneName = EditorGUI.TextField(posRect, data.SceneName);
            };
            _levelsList.onAddCallback = (list) =>
            {
                repo.Levels.Add(new LevelData($"scene_{repo.Count - 1}", $"level_{repo.Count - 1}"));
                list.Select(repo.Count - 1);
                BuildList();
            };
            _levelsList.onRemoveCallback = (list) =>
            {
                var ind = list.index;
                if (ind >= _levelRepository.Count || ind < 0)
                    return;
                repo.Levels.RemoveAt(ind);
                list.Select(repo.Count - 1);
                BuildList();
            };
        }

        private bool TryGetRepository()
        {
            _levelRepository = (LevelsRepository)EditorGUILayout.ObjectField("Repository", _levelRepository, typeof(LevelsRepository),
                    true);
            if (_levelRepository == null)
            {
                EU.Label("No object set...", EU.Red, ErrorFontSize, 'c', true);
                return false;
            }
            Init();
            return true;
        }

        public void OnGUI()
        {
            var topRect = new Rect(new Vector2(0, 0), new Vector2(1000, tier_1_y + (showSettings ? settingsOnOffset : 0)));
            EditorGUI.DrawRect(topRect, backgroundColor2);
            GUILayout.Space(TopOffset);
            EU.Label("Level Manager", EU.Lime, LabelFontSize);
            GUILayout.Space(TopOffset);
            if (!TryGetRepository())
                return;
            try
            {
                _switcher.CorrectIndex();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"exception caught");
                return;
            }

            ShowSettings();

            var t1Left = new Rect(new Vector2(leftRectOffset,tier_1_y + fromTopOffset), new Vector2(150, 100));
            var t1Right = new Rect(new Vector2(leftRectOffset + 150 + 10, tier_1_y + fromTopOffset), new Vector2(400, 100));
            EditorGUI.DrawRect(new Rect(new Vector2(0, tier_1_y + fromTopOffset), new Vector2(1000, t1Right.height)), backgroundColor1);
            ShowControlsButtons(t1Left);
            ShowCurrentLevelInfo(t1Right);
            var t2Rect = new Rect(new Vector2(leftRectOffset, tier_2_y + fromTopOffset), new Vector2(600, 60));
            
            EditorGUI.DrawRect(new Rect( new Vector2(0, tier_2_y + fromTopOffset), new Vector2(1000, 60)), backgroundColor2);
            ShowStatus(t2Rect);
            EditorGUI.DrawRect(new Rect(new Vector2(0, tier_3_y + fromTopOffset), new Vector2(1000,400)), backgroundColor1);
            var t3Left = new Rect(new Vector2(leftRectOffset, tier_3_y + fromTopOffset), new Vector2(170, 400));
            ShowActionButtons(t3Left);
            var t3Right = new Rect(new Vector2(leftRectOffset + 150, tier_3_y + fromTopOffset), new Vector2(250, 400));
            ShowLevelsList(t3Right);
        }

        private void ShowSettings()
        {
            GUILayout.Space(15);
            showSettings = EditorGUILayout.Toggle("Settings: ", showSettings, GUI.skin.toggle);
            fromTopOffset = showSettings ? settingsOnOffset : 0;
            if (showSettings)
            {
                var settingsRect = new Rect(new Vector2(leftRectOffset * 2, 130), new Vector2(600, settingRectHeight));
                GUILayout.BeginArea(settingsRect);
                EditorLevelSwitcher.LoadOnSelect = EditorGUILayout.Toggle("Auto Load", EditorLevelSwitcher.LoadOnSelect, GUI.skin.toggle);
                EditorLevelSwitcher.ClearOnLoad = EditorGUILayout.Toggle("Clear On Load", EditorLevelSwitcher.ClearOnLoad, GUI.skin.toggle);
                EditorLevelSwitcher.LevelsPath = EditorGUILayout.TextField("LevelsPath: ", EditorLevelSwitcher.LevelsPath, GUILayout.Width(400));
                GUILayout.EndArea();
            }
        }
        
        
        private void ShowCurrentLevelInfo(Rect rect)
        {
            GUILayout.BeginArea(rect);
            var level = _levelRepository.GetLevel(_switcher.Index);
            var rectCol1 = new Rect(new Vector2(0, 0), new Vector2(75, 150));
            var rectCol2 = new Rect(new Vector2(75, 0), new Vector2(200, 150));
            var defColElement = new EU.EU_ColumnElement("entry", mainFontSize, EU.White);
            var col1 = new EU.EU_Column(rectCol1, defColElement);
            var col2 = new EU.EU_Column(rectCol2, defColElement);
            col1.AddElement("Index:");
            col1.AddElement("Name:");
            col1.AddElement("Scene:");
            col2.AddElement($"{_switcher.Index}");
            col2.AddElement($"{level.LevelName}");
            col2.AddElement($"{level.SceneName}");
            col1.Show();
            col2.Show();
            GUILayout.EndArea();
        }

        private void ShowControlsButtons(Rect rect)
        {
            GUILayout.BeginArea(rect);
            GUILayout.BeginHorizontal();
            if (EU.BtnSmallSquare("<<", EU.DarkCyan, mainFontSize))
                _switcher.PrevLevel();
            GUILayout.Space(DefaultFieldSpace);
            if (EU.BtnSmallSquare(">>", EU.DarkCyan, mainFontSize))
                _switcher.NextLevel();
            GUILayout.EndHorizontal();

            GUILayout.Space(DefaultFieldSpace * 2);

            GUILayout.BeginHorizontal();
            if (EU.BtnMidSmallHeight("Load", EU.DarkCyan, mainFontSize))
                _switcher.LoadCurrentLevel();
            GUILayout.Space(DefaultFieldSpace);
            if (EU.BtnMidSmallHeight("Clear", EU.Red, mainFontSize))
                _switcher.Clear();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        
        private void ShowStatus(Rect rect)
        {

            GUILayout.BeginArea(rect);
            var level = _levelRepository.GetLevel(_switcher.Index);
            var defColElement = new EU.EU_ColumnElement("-", mainFontSize, Color.white, 'l', true);
            var rectCol1 = new Rect(new Vector2(0, 0), new Vector2(150, 50));
            var rectCol2 = new Rect(new Vector2(170, 0), new Vector2(150, 50));
            var col1 = new EU.EU_Column(rectCol1, defColElement);
            var col2 = new EU.EU_Column(rectCol2, defColElement);

            col1.AddElement("Status Level:"); // level
            col1.AddElement("Status Scene:"); // scene
            col2.AddElement(_switcher.GetLevelStatus(level)); // level
            col2.AddElement(_switcher.GetSceneStatus(level)); // scene
            col1.Show();
            col2.Show();
            GUILayout.EndArea();
        }
        
        private void ShowActionButtons(Rect rect)
        {
            GUILayout.BeginArea(rect);
            GUILayout.Space(DefaultFieldSpace);
            if (EU.BtnMidWide2("Check all", EU.White, mainFontSize))
                _switcher.CheckAll();
            GUILayout.Space(DefaultFieldSpace);
            if (EU.BtnMidWide2("Resources", EU.White, mainFontSize))
                _switcher.AddFromResources();
            GUILayout.Space(DefaultFieldSpace);
            if (EU.BtnMidWide2("Try Order", EU.White, mainFontSize))
                _switcher.TryOrder();
            GUILayout.Space(DefaultFieldSpace);
            if (EU.BtnMidWide2("Randomize", EU.White, mainFontSize))
                _switcher.Randomize();
            GUILayout.Space(DefaultFieldSpace);
            GUILayout.EndArea();
        }


        private void ShowLevelsList(Rect rect)
        {
            if (_levelsList == null)
            {
                EU.Label("Levels list was not created...", Color.red, mainFontSize);
                return;
            }
        

            GUILayout.BeginArea(rect);
            _scroll = EditorGUILayout.BeginScrollView(_scroll,
                false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUI.skin.scrollView);
            // _listRect.position = -_scroll;
            _levelsList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

#endif


        // void ListOfSprites()
        // {
        //     var allObjectGuids = AssetDatabase.FindAssets("t:Sprite");
        //     if (allObjectGuids.Length == 0)
        //         return;
        //     var allObjects = new List<Sprite>();
        //     foreach (var guid in allObjectGuids)
        //         allObjects.Add(AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid)));
        //     // Create a two-pane view with the left pane being fixed with
        //     var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
        //     // Add the panel to the visual tree by adding it as a child to the root element
        //     rootVisualElement.Add(splitView);
        //     // A TwoPaneSplitView always needs exactly two child elements
        //     var leftPane = new ListView();
        //     splitView.Add(leftPane);
        //     // Initialize the list view with all sprites' names
        //     leftPane.makeItem = () => new Label();
        //     leftPane.bindItem = (item, index) => { (item as Label).text = allObjects[index].name; };
        //     leftPane.itemsSource = allObjects;
        //     leftPane.onSelectionChange += OnSpriteSelectionChange;
        //     
        //     m_RightPane = new VisualElement();
        //     splitView.Add(m_RightPane);
        // }
        //
        // private void OnSpriteSelectionChange(IEnumerable<object> selectedItems)
        // {
        //     // Clear all previous content from the pane
        //     m_RightPane.Clear();
        //
        //     // Get the selected sprite
        //     var selectedSprite = selectedItems.First() as Sprite;
        //     if (selectedSprite == null)
        //         return;
        //
        //     // Add a new Image control and display the sprite
        //     var spriteImage = new Image();
        //     spriteImage.scaleMode = ScaleMode.ScaleToFit;
        //     spriteImage.sprite = selectedSprite;
        //
        //     // Add the Image control to the right-hand pane
        //     m_RightPane.Add(spriteImage);
        // }
    }
}
#endif
