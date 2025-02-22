using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ItemCombining.Editor.WindowElements
{
    [UxmlElement]
    public partial class GridButton : VisualElement
    {
        protected Label TextHolder { get; private set; }
        protected VisualElement ImageHolder { get; private set; }
        
        private readonly Action _onClick;

        public GridButton()
        {
            
        }

        public GridButton(Texture2D image, string text, Action onClick)
        {
            var visualTree = GetTemplateScheme();
            if (visualTree != null)
            {
                visualTree.CloneTree(this);
            }
            
            var styleSheet = EditorUtility.LoadAsset<StyleSheet>("Styles/GridButton.uss");
            if (styleSheet != null)
            {
                this.styleSheets.Add(styleSheet);
            }
            
            _onClick = onClick;
            
            RegisterCallback<ClickEvent>(OnClick);

            ImageHolder = this.Q<VisualElement>("grid-btn__image");
            SetImage(image);

            TextHolder = this.Q<Label>("grid-btn__text");
            SetText(text);
        }

        protected virtual VisualTreeAsset GetTemplateScheme() => EditorUtility.LoadAsset<VisualTreeAsset>("Markup/GridButton.uxml");

        private void OnClick(ClickEvent evt)
        {
            _onClick?.Invoke();
        }

        protected void SetImage(Texture2D image)
        {
            ImageHolder.style.backgroundImage = image;
        }

        protected void SetText(string text)
        {
            TextHolder.text = text;
        }
    }
}