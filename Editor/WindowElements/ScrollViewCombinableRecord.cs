using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ItemCombining.Editor.WindowElements
{
    [UxmlElement]
    public partial class ScrollViewCombinableRecord : VisualElement
    {
        public ScrollViewCombinableRecord()
        {
            
        }
        
        public ScrollViewCombinableRecord(ICombinable obj,
            Action<ICombinable> usagesClicked, Action<ICombinable> deleteClicked)
        {
            var visualTree = EditorUtility.LoadAsset<VisualTreeAsset>("Markup/ScrollViewCombinableRecord.uxml");
            if (visualTree != null)
            {
                visualTree.CloneTree(this);
            }
            
            var styleSheet = EditorUtility.LoadAsset<StyleSheet>("Styles/ScrollViewCombinableRecord.uss");
            if (styleSheet != null)
            {
                this.styleSheets.Add(styleSheet);
            }

            Button deleteButton = this.Q<Button>("combinable-record__delete-button");
            Button showUsages = this.Q<Button>("combinable-record__usage-button");
            Label nameLabel = this.Q<Label>("combinable-record__item-name");
            VisualElement iconHolder = this.Q<VisualElement>("combinable-record__icon-holder");

            nameLabel.text = obj.ScriptableObject.name;
            iconHolder.style.backgroundImage = obj.Editor_Icon != null ? obj.Editor_Icon :
                EditorUtility.LoadAsset<Texture2D>("Textures/ic_default_item.png");

            showUsages.clicked += () => usagesClicked?.Invoke(obj);
            deleteButton.clicked += () => deleteClicked?.Invoke(obj);
        }
        
    }
}