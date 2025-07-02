using UnityEngine;
using UnityEngine.UIElements;
using ScrollViewCombinableRecord = ItemCombining.Editor.WindowElements.ScrollViewCombinableRecord;

// ReSharper disable PossibleNullReferenceException
namespace ItemCombining.Editor.WindowPanels
{
    public class MainWindowPanel : WindowPanelBase
    {
        private const string ADD_COMPONENT_BTN_NAME = "AddComponentBtn";
        private const string ADD_RESULT_BTN_NAME = "AddResultBtn";
        private const string COMPONENTS_SCROLL_VIEW_NAME = "ComponentsScrollView";
        private const string RESULTS_SCROLL_VIEW_NAME = "ResultsScrollView";
        
        [ElementQuery(ADD_COMPONENT_BTN_NAME)] private Button _addComponentBtn;
        [ElementQuery(ADD_RESULT_BTN_NAME)] private Button _addResultBtn;
        [ElementQuery(COMPONENTS_SCROLL_VIEW_NAME)] private ScrollView _componentsScrollView;
        [ElementQuery(RESULTS_SCROLL_VIEW_NAME)] private ScrollView _resultsScrollView;
        
        public MainWindowPanel(string panelName, CombinationDictionary combinationDictionary, VisualElement rootVisualElement, CombinationDictionaryWindow window) : base(panelName, combinationDictionary, rootVisualElement, window)
        {
            _addComponentBtn.clicked += () => ShowObjectCapture(CombinationDictionary.CombinationBinding.Component);
            _addResultBtn.clicked += () => ShowObjectCapture(CombinationDictionary.CombinationBinding.Result);
        }

        protected override void OnShown()
        {
            RefreshScrollLists();
        }

        private void RefreshScrollLists()
        {
            _componentsScrollView.contentContainer.Clear();
            _resultsScrollView.contentContainer.Clear();
            var objects = Dictionary.GetObjects<ScriptableObject>(null);
            foreach (var scriptableObject in objects)
            {
                var combinableType = scriptableObject.GetType();
                if (scriptableObject is ICombinableComponent component)
                {
                    bool isPossibleInputType = true;
                    if (Dictionary.PossibleInputTypes is { Length: > 0 })
                    {
                        isPossibleInputType = false;
                        foreach (var dictionaryPossibleInputType in Dictionary.PossibleInputTypes)
                        {
                            if (combinableType != dictionaryPossibleInputType.Get())
                                continue;

                            isPossibleInputType = true;
                            break;
                        }
                        
                    }
                    
                    if (isPossibleInputType)
                        _componentsScrollView.Add(new ScrollViewCombinableRecord(component,
                            obj => UsagesClicked(obj, CombinationDictionary.CombinationBinding.Component), DeleteClicked));
                }
                if (scriptableObject is ICombinableResult result)
                {
                    bool isPossibleOutputType = true;
                    if (Dictionary.PossibleOutputTypes is { Length: > 0 })
                    {
                        isPossibleOutputType = false;
                        foreach (var dictionaryPossibleOutputType in Dictionary.PossibleOutputTypes)
                        {
                            if (combinableType != dictionaryPossibleOutputType.Get())
                                continue;

                            isPossibleOutputType = true;
                            break;
                        }
                    }
                    
                    if (isPossibleOutputType)
                        _resultsScrollView.Add(new ScrollViewCombinableRecord(result,
                            obj => UsagesClicked(obj, CombinationDictionary.CombinationBinding.Result), DeleteClicked));
                }
            }
        }

        private void ShowObjectCapture(CombinationDictionary.CombinationBinding binding)
        {
            Window.ShowPanel<ObjectCaptureWindowPanel>(binding);
        }

        private void DeleteClicked(ICombinable obj)
        {
            Dictionary.Editor_RemoveObject(obj.ScriptableObject);
            RefreshScrollLists();
        }

        private void UsagesClicked(ICombinable obj, CombinationDictionary.CombinationBinding binding)
        {
            Window.ShowPanel<UsagesWindowPanel>(obj, binding);
        }
    }
}