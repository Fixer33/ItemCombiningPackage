using ItemCombining.Editor.WindowPanels;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ItemCombining.Editor
{
    public class CombinationDictionaryWindow : EditorWindow
    {
        private const string MAIN_PANEL_NAME = "Main";
        private const string CAPTURE_OBJECT_PANEL_NAME = "CaptureObjectPanel";
        private const string USAGES_PANEL_NAME = "UsagesPanel";
        private const string EDITED_COMBINATION_PANEL_NAME = "EditedCombinationPanel";
        
        private static CombinationDictionary _combinationDictionary;

        private MainWindowPanel _mainPanel;
        private WindowPanelBase[] _panels;
        private WindowPanelBase _activePanel;
        
        public static void ShowWindow(CombinationDictionary dictionary)
        {
            _combinationDictionary = dictionary;
            var window = GetWindow<CombinationDictionaryWindow>();
            window.titleContent = new GUIContent("Combination dictionary");
            window.Show();
        }

        private void OnInspectorUpdate()
        {
            _activePanel?.OnInspectorUpdate();
            
        }

        private void CreateGUI()
        {
            // var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Editor/MyWindow.uxml");
            var visualTree = EditorUtility.LoadAsset<VisualTreeAsset>("Markup/CombinationDictionaryWindow.uxml");
            if (visualTree != null)
            {
                VisualElement root = rootVisualElement;
                visualTree.CloneTree(root);
            }
            
            var styleSheet = EditorUtility.LoadAsset<StyleSheet>("Styles/CombinationDictionaryWindow.uss");
            if (styleSheet != null)
            {
                rootVisualElement.styleSheets.Add(styleSheet);
            }

            _panels = new WindowPanelBase[]
            {
                new MainWindowPanel(MAIN_PANEL_NAME, _combinationDictionary, rootVisualElement, this),
                new ObjectCaptureWindowPanel(CAPTURE_OBJECT_PANEL_NAME, _combinationDictionary, rootVisualElement, this),
                new UsagesWindowPanel(USAGES_PANEL_NAME, _combinationDictionary, rootVisualElement, this),
                new EditedCombinationWindowPanel(EDITED_COMBINATION_PANEL_NAME, _combinationDictionary, rootVisualElement, this),
            };
            
            ShowPanel<MainWindowPanel>();
        }

        public void ShowPanel<T>() where T : WindowPanelBase
        {
            foreach (var windowPanelBase in _panels)
            {
                if (windowPanelBase is not T)
                    continue;
                
                _activePanel?.Hide();
                _activePanel = windowPanelBase;
                _activePanel.Show();
            }
        }
        
        public void ShowPanel<T>(params object[] args) where T : WindowPanelBase
        {
            foreach (var windowPanelBase in _panels)
            {
                if (windowPanelBase is not T)
                    continue;
                
                _activePanel?.Hide();
                _activePanel = windowPanelBase;
                _activePanel.Show(args);
            }
        }
    }
}