using System;
using System.Text;
using UnityEngine.UIElements;

namespace ItemCombining.Editor.WindowElements
{
    [UxmlElement]
    public partial class CombinationPreview : VisualElement
    {
        private readonly Action<CombinationDictionary.SerializedCombinationData> _editClicked;
        private readonly CombinationDictionary.SerializedCombinationData _combination;
        
        public CombinationPreview()
        {
            
        }

        public CombinationPreview(CombinationDictionary.SerializedCombinationData combination, Action<CombinationDictionary.SerializedCombinationData> editClicked)
        {
            _combination = combination;
            _editClicked = editClicked;
            
            var visualTree = EditorUtility.LoadAsset<VisualTreeAsset>("Markup/CombinationPreview.uxml");
            if (visualTree != null)
            {
                visualTree.CloneTree(this);
            }
            
            var styleSheet = EditorUtility.LoadAsset<StyleSheet>("Styles/CombinationPreview.uss");
            if (styleSheet != null)
            {
                this.styleSheets.Add(styleSheet);
            }
            
            StringBuilder sb = new();
            Label componentsLabel = this.Q<Label>("combination-preview__components-label");
            Label resultsLabel = this.Q<Label>("combination-preview__results-label");
            Button editBtn = this.Q<Button>("combination-preview__edit-btn");
            editBtn.clicked += EditBtnClicked;

            sb.Clear();
            if (combination.Components is { Length: > 0 })
            {
                sb.Append(combination.Components[0].name);
                for (var i = 1; i < combination.Components.Length; i++)
                {
                    sb.Append(", ");
                    sb.Append(combination.Components[i].name);
                }
            }
            componentsLabel.text = sb.ToString();

            sb.Clear();
            if (combination.Results is { Length: > 0 })
            {
                sb.Append(combination.Results[0].name);
                for (var i = 1; i < combination.Results.Length; i++)
                {
                    sb.Append(", ");
                    sb.Append(combination.Results[i].name);
                }
            }
            resultsLabel.text = sb.ToString();
        }

        private void EditBtnClicked()
        {
            _editClicked?.Invoke(_combination);
        }
    }
}