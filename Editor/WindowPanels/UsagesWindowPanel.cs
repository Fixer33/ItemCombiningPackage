using System;
using UnityEngine;
using UnityEngine.UIElements;
using CombinationPreview = ItemCombining.Editor.WindowElements.CombinationPreview;

// ReSharper disable PossibleNullReferenceException
namespace ItemCombining.Editor.WindowPanels
{
    public class UsagesWindowPanel : WindowPanelBase
    {
        private const string USAGES_PANEL_CONTAINER_NAME = "usages-panel__records-holder";
        private const string USAGES_PANEL_CREATE_BTN_NAME = "usages-panel__create-btn";
        private const string USAGES_PANEL_BACK_BTN_NAME = "usages-panel__back-btn";
        
        [ElementQuery(USAGES_PANEL_CONTAINER_NAME)] private ScrollView _container;
        [ElementQuery(USAGES_PANEL_CREATE_BTN_NAME)] private Button _addBtn;
        [ElementQuery(USAGES_PANEL_BACK_BTN_NAME)] private Button _backBtn;
        private CombinationDictionary.CombinationBinding _binding;
        private ICombinable _selectedObject;
        
        public UsagesWindowPanel(string panelName, CombinationDictionary combinationDictionary, VisualElement rootVisualElement, CombinationDictionaryWindow window) : base(panelName, combinationDictionary, rootVisualElement, window)
        {
            _backBtn.clicked += () =>
            {
                window.ShowPanel<MainWindowPanel>();
            };
            
            _addBtn.clicked += AddBtnClicked;
        }

        protected override void OnShown(object[] args)
        {
            _selectedObject = (ICombinable)args[0];
            _binding = (CombinationDictionary.CombinationBinding)args[1];
            RefreshList();
        }

        protected override void OnShown()
        {
            RefreshList();
        }

        private void RefreshList()
        {
            _container.contentContainer.Clear();
            var combinations = Dictionary.GetCombinationsWith(_selectedObject, _binding);
            foreach (var combination in combinations)
            {
                _container.contentContainer.Add(new CombinationPreview(combination, EditCombinationClicked));
            }
        }

        private void EditCombinationClicked(CombinationDictionary.SerializedCombinationData combination)
        {
            Window.ShowPanel<EditedCombinationWindowPanel>(combination);
        }

        private void AddBtnClicked()
        {
            Window.ShowPanel<EditedCombinationWindowPanel>(new CombinationDictionary.SerializedCombinationData()
            {
                Components = Array.Empty<ScriptableObject>(),
                Results = Array.Empty<ScriptableObject>()
            });
        }
    }
}