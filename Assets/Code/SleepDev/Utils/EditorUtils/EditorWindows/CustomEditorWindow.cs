// using System.Collections.Generic;
// using System.Linq;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.UIElements;
//
// namespace SleepDev.Utils.EditorUtils.EditorWindows
// {
//     #if UNITY_EDITOR
//     public class CustomEditorWindow : EditorWindow
//     {
//         private VisualElement m_RightPane;
//         
//         [MenuItem("Tools/My Custom Editor")]
//         public static void ShowMyEditor()
//         {
//             // This method is called when the user selects the menu item in the Editor
//             EditorWindow wnd = GetWindow<CustomEditorWindow>();
//             wnd.titleContent = new GUIContent("My Custom Editor");
//         }
//         
//         public void CreateGUI()
//         {
//             rootVisualElement.Add(new Label("MY LABEL"));
//             ListOfSprites();
//         }
//
//         void ListOfSprites()
//         {
//             var allObjectGuids = AssetDatabase.FindAssets("t:Sprite");
//             if (allObjectGuids.Length == 0)
//                 return;
//             var allObjects = new List<Sprite>();
//             foreach (var guid in allObjectGuids)
//                 allObjects.Add(AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid)));
//             // Create a two-pane view with the left pane being fixed with
//             var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
//             // Add the panel to the visual tree by adding it as a child to the root element
//             rootVisualElement.Add(splitView);
//             // A TwoPaneSplitView always needs exactly two child elements
//             var leftPane = new ListView();
//             splitView.Add(leftPane);
//             // Initialize the list view with all sprites' names
//             leftPane.makeItem = () => new Label();
//             leftPane.bindItem = (item, index) => { (item as Label).text = allObjects[index].name; };
//             leftPane.itemsSource = allObjects;
//             leftPane.onSelectionChange += OnSpriteSelectionChange;
//             
//             m_RightPane = new VisualElement();
//             splitView.Add(m_RightPane);
//         }
//         
//         private void OnSpriteSelectionChange(IEnumerable<object> selectedItems)
//         {
//             // Clear all previous content from the pane
//             m_RightPane.Clear();
//
//             // Get the selected sprite
//             var selectedSprite = selectedItems.First() as Sprite;
//             if (selectedSprite == null)
//                 return;
//
//             // Add a new Image control and display the sprite
//             var spriteImage = new Image();
//             spriteImage.scaleMode = ScaleMode.ScaleToFit;
//             spriteImage.sprite = selectedSprite;
//
//             // Add the Image control to the right-hand pane
//             m_RightPane.Add(spriteImage);
//         }
//     }
//     #endif
// }