using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable PossibleNullReferenceException
namespace ItemCombining.Editor.WindowPanels
{
    public class ObjectCaptureWindowPanel : WindowPanelBase
    {
        private const string CAPTURE_OBJECT_PANEL_VALID_CAPTURE_CLASS = "captureObjectPanelValidTarget";
        private const string CAPTURE_OBJECT_HEADER_NAME = "CaptureObjectHeader";
        private const string CAPTURE_OBJECT_ADD_OBJECT_BTN_NAME = "AddCapturedObjectButton";
        private const string CAPTURE_OBJECT_CANCEL_BTN_NAME = "CancelObjectCaptureBtn";
        
        [ElementQuery(CAPTURE_OBJECT_ADD_OBJECT_BTN_NAME)] private Button _addBtn;
        [ElementQuery(CAPTURE_OBJECT_CANCEL_BTN_NAME)] private Button _cancelBtn;
        [ElementQuery(CAPTURE_OBJECT_HEADER_NAME)] private Label _header;

        private CombinationDictionary.CombinationBinding _binding;
        private ICombinable _lastSelectedObj;
        
        public ObjectCaptureWindowPanel(string panelName, CombinationDictionary combinationDictionary, VisualElement rootVisualElement, CombinationDictionaryWindow window) : base(panelName, combinationDictionary, rootVisualElement, window)
        {
            _cancelBtn.clicked += () =>
            {
                window.ShowPanel<MainWindowPanel>();
            };
            _addBtn.clicked += () =>
            {
                combinationDictionary.Editor_AddObject(_lastSelectedObj);
                window.ShowPanel<MainWindowPanel>();
            };
        }

        protected override void OnShown(object[] args)
        {
            _binding = (CombinationDictionary.CombinationBinding)args[0];
        }

        protected override void OnUpdate()
        {
            if (Selection.activeObject is ICombinable combinable)
            {
                var combinableType = combinable.GetType();
                
                if ((_binding & CombinationDictionary.CombinationBinding.Result) != 0)
                {
                    if (combinable is not ICombinableResult)
                        return;

                    if (Dictionary.PossibleOutputTypes is { Length: > 0 })
                    {
                        bool isPossibleOutputType = false;
                        foreach (var dictionaryPossibleOutputType in Dictionary.PossibleOutputTypes)
                        {
                            if (combinableType != dictionaryPossibleOutputType.Get())
                                continue;

                            isPossibleOutputType = true;
                            break;
                        }
                        if (isPossibleOutputType == false)
                            return;
                    }
                }

                if ((_binding & CombinationDictionary.CombinationBinding.Component) != 0)
                {
                    if (combinable is not ICombinableComponent)
                        return;
                    
                    if (Dictionary.PossibleInputTypes is { Length: > 0 })
                    {
                        bool isPossibleInputType = false;
                        foreach (var dictionaryPossibleInputType in Dictionary.PossibleInputTypes)
                        {
                            if (combinableType != dictionaryPossibleInputType.Get())
                                continue;

                            isPossibleInputType = true;
                            break;
                        }
                        if (isPossibleInputType == false)
                            return;
                    }
                }
                
                if (_lastSelectedObj == combinable)
                    return;

                _lastSelectedObj = combinable;
                Panel.AddToClassList(CAPTURE_OBJECT_PANEL_VALID_CAPTURE_CLASS);
                if (combinable is ScriptableObject so)
                    _header.text = $"Ready to capture object:\n {so.name}";
                return;
            }

            _lastSelectedObj = null;
            Panel.RemoveFromClassList(CAPTURE_OBJECT_PANEL_VALID_CAPTURE_CLASS);
            _header.text = $"No valid object to capture";
        }
    }
}