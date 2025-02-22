using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using CombinableGridButton = ItemCombining.Editor.WindowElements.CombinableGridButton;
using GridButton = ItemCombining.Editor.WindowElements.GridButton;

// ReSharper disable PossibleNullReferenceException
namespace ItemCombining.Editor.WindowPanels
{
    public class EditedCombinationWindowPanel : WindowPanelBase
    {
        [ElementQuery("editor-combination-panel__save-btn")] private Button _saveBtn;
        [ElementQuery("editor-combination-panel__back-btn")] private Button _backBtn;
        [ElementQuery("editor-combination-panel__components-holder")] private ScrollView _componentsHolder;
        [ElementQuery("editor-combination-panel__results-holder")] private ScrollView _resultsHolder;
        
        [ElementQuery("editor-combination-panel__object-picker")] private VisualElement _objectPicker;
        [ElementQuery("ecp_picker_dropdown")] private DropdownField _objectsDropdown;
        [ElementQuery("ecp_picker_cancel-btn")] private Button _cancelObjPickBtn;
        [ElementQuery("ecp_picker_apply-btn")] private Button _applyObjPickBtn;
        
        private readonly Texture2D _addIcon;
        private CombinationDictionary.SerializedCombinationData _combination;
        private Action _onPickerAddPress;
        
        public EditedCombinationWindowPanel(string panelName, CombinationDictionary combinationDictionary, VisualElement rootVisualElement, CombinationDictionaryWindow window) : base(panelName, combinationDictionary, rootVisualElement, window)
        {
            _backBtn.clicked += () =>
            {
                window.ShowPanel<UsagesWindowPanel>();
            };
            
            _saveBtn.clicked += SaveBtnClicked;

            _addIcon = EditorUtility.LoadAsset<Texture2D>("Textures/ic_add.png");

            _objectPicker.visible = false;
            _applyObjPickBtn.clicked += ApplyObjPickBtnClicked;
            _cancelObjPickBtn.clicked += CancelObjPickBtnClicked;
        }

        protected override void OnShown(object[] args)
        {
            _combination = (CombinationDictionary.SerializedCombinationData)args[0];
            Refresh();
        }

        private void Refresh()
        {
            _componentsHolder.contentContainer.Clear();
            _resultsHolder.contentContainer.Clear();
            
            for (var i = 0; i < _combination.Components.Length; i++)
            {
                int index = i;
                _componentsHolder.contentContainer.Add(new CombinableGridButton(_combination.Components[i] as ICombinable, () =>
                {
                    ShowPicker(true, index);
                }, () =>
                {
                    var list = _combination.Components.ToList();
                    list.RemoveAt(index);
                    _combination.Components = list.ToArray();
                    Refresh();
                }));
            }
            for (var i = 0; i < _combination.Results.Length; i++)
            {
                int index = i;
                _resultsHolder.contentContainer.Add(new CombinableGridButton(_combination.Results[i] as ICombinable, () =>
                {
                    ShowPicker(false, index);
                }, () =>
                {
                    var list = _combination.Results.ToList();
                    list.RemoveAt(index);
                    _combination.Results = list.ToArray();
                    Refresh();
                }));
            }
            
            _componentsHolder.contentContainer.Add(new GridButton(_addIcon, "Add new", () => ShowPicker(true, -1)));
            _resultsHolder.contentContainer.Add(new GridButton(_addIcon, "Add new", () => ShowPicker(false, -1)));
        }

        private void SaveBtnClicked()
        {
            Dictionary.Editor_ModifyCombination(_combination);
            Window.ShowPanel<UsagesWindowPanel>();
        }

        private void ApplyObjPickBtnClicked()
        {
            _onPickerAddPress?.Invoke();
            _objectPicker.visible = false;
            Refresh();
        }

        private void CancelObjPickBtnClicked()
        {
            _objectPicker.visible = false;
            Refresh();
        }

        private void ShowPicker(bool isComponent, int elementIndex)
        {
            _objectPicker.visible = true;

            var pickedObjectsInCategory = _combination.Results;
            if (isComponent) 
                pickedObjectsInCategory = _combination.Components;

            if (elementIndex >= 0)
            {
                pickedObjectsInCategory = pickedObjectsInCategory.Except(new[] { pickedObjectsInCategory[elementIndex] }).ToArray();
            }
            
            var objects = Dictionary.GetObjects<ScriptableObject>().Except(pickedObjectsInCategory)
                .Where(i => isComponent ? i is ICombinableComponent : i is ICombinableResult);
            
            _objectsDropdown.choices = objects.Select(i => i.name).ToList();
            _objectsDropdown.index = -1;
            _objectsDropdown.value = "--Pick object--";

            _onPickerAddPress = () =>
            {
                int index = _objectsDropdown.index;
                if (index < 0)
                    return;

                var obj = objects.ToArray()[index];
                switch (elementIndex, isComponent)
                {
                    case (< 0, true):
                        Array.Resize(ref _combination.Components, _combination.Components.Length + 1);
                        _combination.Components[^1] = obj;
                        break;
                    case (> 0, true):
                        _combination.Components[elementIndex] = obj;
                        break;
                    case (< 0, false):
                        Array.Resize(ref _combination.Results, _combination.Results.Length + 1);
                        _combination.Results[^1] = obj;
                        break;
                    case (> 0, false):
                        _combination.Results[elementIndex] = obj;
                        break;
                }
            };
        }
    }
}