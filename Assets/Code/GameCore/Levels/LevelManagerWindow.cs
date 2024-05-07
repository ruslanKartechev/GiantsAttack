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
        private const int offsetTop = 15;
        private const int offsetLeft = 15;
        private const int offsetWhenSettings = 80;
        private const int spaceDefault = 5;
        private const int offsetBeforeSettings = 100;
        
        private const int fontsizeHeader = 24;
        private const int fontsizeError = 18;
        private const int fontsizeLevels = 10;
        private const int fontsizeMain = 16;

        private const int listElementHeight = 60;
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
        private Color headerColor = Color.yellow;
        private Color btnColorGreen = EU.Aqua;
        private Color btnColorRed = EU.HotPink;
        private Color btnColorNeutral = EU.White;
        
        [MenuItem("SleepDev/Level Manager")]
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
                EU.LabelRect(rect, "LEVELS", fontsizeMain, Color.white, 'c', true);
            };
            _levelsList.elementHeight = listElementHeight;
            _levelsList.drawElementCallback = (rect, index, active, focused) =>
            {
                if (index >= repo.Count || index < 0)
                    return;
                var posRect = new Rect(rect.position.x, rect.position.y, elementWith, 20);
                EU.LabelRect(posRect, $"Order {index}", fontsizeLevels + 2, Color.white, 'c', true);
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
            if (_levelRepository == null)
            {
                EU.Label("No object set...", EU.Red, fontsizeError, 'c', true);
                return false;
            }
            Init();
            return true;
        }

        public void OnGUI()
        {
            var rectBand1 = new Rect(new Vector2(0, 0), new Vector2(position.width, tier_1_y + (showSettings ? offsetWhenSettings : 0)));
            var rectBand2 = new Rect(new Vector2(0, tier_1_y + fromTopOffset), new Vector2(position.width, 100));
            var rectBand3 = new Rect(new Vector2(0, tier_2_y + fromTopOffset), new Vector2(position.width, 60));
            var rectBand4 = new Rect(new Vector2(0, tier_3_y + fromTopOffset), new Vector2(position.width, 600));
            EditorGUI.DrawRect(rectBand1, backgroundColor1); // header + settings
            EditorGUI.DrawRect(rectBand2, backgroundColor2); // buttons + info
            EditorGUI.DrawRect(rectBand3, backgroundColor1); // status
            EditorGUI.DrawRect(rectBand4, backgroundColor2);
            ShowHeader(rectBand1);
            
            if (!TryGetRepository())
                return;
            try
            {
                _switcher.CorrectIndex();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"exception caught: {ex.Message}");
                return;
            }

            ShowSettings(rectBand1);

            var rectLeft = new Rect(new Vector2(offsetLeft,tier_1_y + fromTopOffset), new Vector2(150, 100));
            var rectRight = new Rect(new Vector2(offsetLeft + 150 + 10, tier_1_y + fromTopOffset), new Vector2(400, 100));
            ShowControlsButtons(rectLeft);
            ShowCurrentLevelInfo(rectRight);
            var rectStatus = new Rect(rectBand3);
            rectStatus.x += offsetLeft;
            ShowStatus(rectStatus);
            
            var rectBand4Left = new Rect(new Vector2(offsetLeft, tier_3_y + fromTopOffset), new Vector2(170, 400));
            var rectBand4Right = new Rect(new Vector2(offsetLeft + 150, tier_3_y + fromTopOffset), new Vector2(250, 400));
            ShowActionButtons(rectBand4Left);
            ShowLevelsList(rectBand4Right);
        }

        private void ShowHeader(Rect rect)
        {
            GUILayout.BeginArea(rect);
            GUILayout.Space(offsetTop);
            EU.Label("Level Manager", headerColor, fontsizeHeader);
            GUILayout.Space(offsetTop);
            
            _levelRepository = (LevelsRepository)EU.ObjectField("Repository", fontsizeMain, Color.white, 100, _levelRepository, typeof(LevelsRepository));
            GUILayout.EndArea();
        }
        

        private void ShowSettings(Rect rect)
        {
            GUILayout.BeginArea(rect);
            GUILayout.Space(offsetBeforeSettings);
            // showSettings = EditorGUILayout.Toggle("Settings: ", showSettings, GUI.skin.toggle);
            showSettings = EU.Toggle("Settings: ", fontsizeMain, Color.white, 90, showSettings);
            fromTopOffset = showSettings ? offsetWhenSettings : 0;
            if (showSettings)
            {
                var settingsRect = new Rect(new Vector2(offsetLeft * 2, 130), new Vector2(600, settingRectHeight));
                GUILayout.BeginArea(settingsRect);
                EditorLevelSwitcher.LoadOnSelect = EU.Toggle("Auto Load: ",fontsizeMain, Color.white, 120, EditorLevelSwitcher.LoadOnSelect);
                EditorLevelSwitcher.ClearOnLoad = EU.Toggle("Clear On Load: ",fontsizeMain, Color.white, 120, EditorLevelSwitcher.ClearOnLoad);
                EditorLevelSwitcher.LevelsPath = EU.TextField("LevelsPath: ",fontsizeMain, Color.white, 120, EditorLevelSwitcher.LevelsPath);

                // EditorLevelSwitcher.LoadOnSelect = EditorGUILayout.Toggle("Auto Load", EditorLevelSwitcher.LoadOnSelect, GUI.skin.toggle);
                // EditorLevelSwitcher.ClearOnLoad = EditorGUILayout.Toggle("Clear On Load", EditorLevelSwitcher.ClearOnLoad, GUI.skin.toggle);
                // EditorLevelSwitcher.LevelsPath = EditorGUILayout.TextField("LevelsPath: ", EditorLevelSwitcher.LevelsPath, GUILayout.Width(400));
                GUILayout.EndArea();
            }
            GUILayout.EndArea();
        }
        
        
        private void ShowCurrentLevelInfo(Rect rect)
        {
            GUILayout.BeginArea(rect);
            var level = _levelRepository.GetLevel(_switcher.Index);
            var rectCol1 = new Rect(new Vector2(0, 0), new Vector2(75, 150));
            var rectCol2 = new Rect(new Vector2(75, 0), new Vector2(200, 150));
            var defColElement = new EU.EU_ColumnElement("entry", fontsizeMain, EU.White);
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
            if (EU.BtnSmallSquare("<<", btnColorGreen, fontsizeMain))
                _switcher.PrevLevel();
            GUILayout.Space(spaceDefault);
            if (EU.BtnSmallSquare(">>", btnColorGreen, fontsizeMain))
                _switcher.NextLevel();
            GUILayout.EndHorizontal();

            GUILayout.Space(spaceDefault * 2);

            GUILayout.BeginHorizontal();
            if (EU.BtnMidSmallHeight("Load", btnColorGreen, fontsizeMain))
                _switcher.LoadCurrentLevel();
            GUILayout.Space(spaceDefault);
            if (EU.BtnMidSmallHeight("Clear", btnColorRed, fontsizeMain))
                _switcher.Clear();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        
        private void ShowStatus(Rect rect)
        {

            GUILayout.BeginArea(rect);
            var level = _levelRepository.GetLevel(_switcher.Index);
            var defColElement = new EU.EU_ColumnElement("-", fontsizeMain, Color.white, 'l', true);
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
            GUILayout.Space(spaceDefault);
            if (EU.BtnMidWide2("Check all", btnColorNeutral, fontsizeMain))
                _switcher.CheckAll();
            GUILayout.Space(spaceDefault);
            if (EU.BtnMidWide2("Resources", btnColorNeutral, fontsizeMain))
                _switcher.AddFromResources();
            GUILayout.Space(spaceDefault);
            if (EU.BtnMidWide2("Try Order", btnColorNeutral, fontsizeMain))
                _switcher.TryOrder();
            GUILayout.Space(spaceDefault);
            if (EU.BtnMidWide2("Randomize", btnColorNeutral, fontsizeMain))
                _switcher.Randomize();
            GUILayout.Space(spaceDefault);
            GUILayout.EndArea();
        }


        private void ShowLevelsList(Rect rect)
        {
            if (_levelsList == null)
            {
                EU.Label("Levels list was not created...", btnColorRed, fontsizeMain);
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
