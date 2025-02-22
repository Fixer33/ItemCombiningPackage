using UnityEditor;
using UnityEngine;

namespace ItemCombining.Editor
{
    [CustomEditor(typeof(CombinationDictionary))]
    public class CombinationDictionaryEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (target is not CombinationDictionary dictionary)
                return;

            if (dictionary.Editor_CanChangeRequireSequence())
            {
                bool requireSequence = dictionary.RequireCorrectSequence;
                bool newVal = GUILayout.Toggle(requireSequence, "Requires correct sequence: ");
                if (requireSequence != newVal)
                    dictionary.Editor_SetRequireSequence(newVal);
            }
            else
            {
                GUILayout.Label($"Requires exact sequence: {dictionary.RequireCorrectSequence}. To edit this property dictionary must be empty");
            }
            
            if (GUILayout.Button("Open editor"))
            {
                CombinationDictionaryWindow.ShowWindow(dictionary);
            }

            GUILayout.Space(100);
            var defaultColor = GUI.color;
            GUI.color = Color.red;
            if (GUILayout.Button("Clear all combinations"))
            {
                dictionary.Editor_ClearAllCombinations();
            }
            if (GUILayout.Button("Clear all data"))
            {
                dictionary.Editor_ClearEverything();
            }
            GUI.color = defaultColor;
        }
    }
}