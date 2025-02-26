using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ItemCombining.Editor.WindowElements
{
    [UxmlElement]
    public partial class CombinableGridButton : GridButton
    {
        private readonly Texture2D _replaceIcon;
        private readonly ICombinable _combinable;
        
        public CombinableGridButton()
        {
            
        }
        
        public CombinableGridButton(ICombinable combinable, Action onClick, Action onRemovePress) : base(null, combinable.ScriptableObject.name, onClick)
        {
            _combinable = combinable;
            
            RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            RegisterCallback<PointerLeaveEvent>(OnPointerLeave);

            Button removeBtn = this.Q<Button>("combinable-grid-btn__remove-btn");
            removeBtn.clicked += onRemovePress;

            _replaceIcon = EditorUtility.LoadAsset<Texture2D>("Textures/ic_replace.png");
            SetImage(GetImage());
        }

        protected override VisualTreeAsset GetTemplateScheme() => EditorUtility.LoadAsset<VisualTreeAsset>("Markup/CombinableGridButton.uxml");

        private void OnPointerEnter(PointerEnterEvent evt)
        {
            SetImage(_replaceIcon);
            SetText("-Click to replace-");
        }

        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            SetImage(GetImage());
            SetText(_combinable.ScriptableObject.name);
        }

        private Texture2D GetImage()
        {
            if (_combinable.Editor_Icon != null)
                return _combinable.Editor_Icon;
            
            return EditorUtility.LoadAsset<Texture2D>("Textures/ic_default_item.png");
        }
    }
}