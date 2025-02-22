using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace ItemCombining.Editor.WindowPanels
{
    public abstract class WindowPanelBase
    {
        protected VisualElement Root { get; private set; }
        protected CombinationDictionary Dictionary { get; private set; }
        protected VisualElement Panel { get; private set; }
        protected CombinationDictionaryWindow Window { get; private set; }
        
        protected WindowPanelBase(string panelName, 
            CombinationDictionary combinationDictionary, VisualElement rootVisualElement, CombinationDictionaryWindow window)
        {
            Root = rootVisualElement;
            Dictionary = combinationDictionary;
            Window = window;
            Panel = rootVisualElement.Q<VisualElement>(panelName);
            Panel.visible = false;
            
            QueryElements();
        }
        
        private void QueryElements()
        {
            var fieldsToQuery = GetElementsToQuery();

            foreach (var queryInfo in fieldsToQuery)
            {
                queryInfo.field.SetValue(this, Root.Q(queryInfo.queryName));
            }
        }
        
        private (FieldInfo field, string queryName)[] GetElementsToQuery()
        {
            List<(FieldInfo field, string name)> res = new();
            var fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            
            foreach (var field in fields)
            {
                var queryAttribute = field.GetCustomAttribute<ElementQuery>();
                if (queryAttribute == null) 
                    continue;

                string queryName = queryAttribute.GetName(field);
                
                var element = Root.Q(queryName);
                if (element != null && field.FieldType.IsInstanceOfType(element))
                {
                    field.SetValue(this, element);
                    res.Add((field, queryName));
                }
                else
                {
                    Debug.LogError($"Element '{queryAttribute.ElementName}' not found or incompatible with field '{field.Name}'");
                }
            }

            return res.ToArray();
        }

        public void Show()
        {
            Panel.visible = true;
            OnShown();
        }
        
        public void Show(params object[] args)
        {
            Panel.visible = true;
            OnShown(args);
        }

        public void Hide()
        {
            Panel.visible = false;
            OnHidden();
        }
        
        public void OnInspectorUpdate()
        {
            OnUpdate();
        }

        protected virtual void OnShown(){}
        protected virtual void OnShown(object[] args){}
        protected virtual void OnHidden(){}
        protected virtual void OnUpdate(){}
        
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        protected class ElementQuery : Attribute
        {
            public readonly string ElementName;

            public ElementQuery()
            {
                ElementName = null;
            }

            public ElementQuery(string elementName)
            {
                ElementName = elementName;
            }

            public string GetName(FieldInfo field)
            {
                if (string.IsNullOrEmpty(ElementName))
                {
                    string name = field.Name;
                    if (name.Length < 2)
                        return field.Name;

                    if (name[0] == '_')
                    {
                        string firstCapital = name[1].ToString().ToUpper();
                        name = name.Substring(2, name.Length - 2);
                        name = firstCapital + name;
                    }

                    return name;
                }

                return ElementName;
            }
        }
    }
}