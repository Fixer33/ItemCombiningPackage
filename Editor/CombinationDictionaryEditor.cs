using UnityEditor;
using UnityEngine;

namespace ItemCombining.Editor
{
    [CustomEditor(typeof(CombinationDictionary))]
    public class CombinationDictionaryEditor : UnityEditor.Editor
    {
        private SerializedProperty _inputTypesProp;
        private SerializedProperty _outputTypesProp;

        private void OnEnable()
        {
            _inputTypesProp = serializedObject.FindProperty("_possibleInputTypes");
            _outputTypesProp = serializedObject.FindProperty("_possibleOutputTypes");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
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
            
            EditorGUILayout.PropertyField(_inputTypesProp, new GUIContent("Possible Input Types"), true);
            EditorGUILayout.PropertyField(_outputTypesProp, new GUIContent("Possible Output Types"), true);
            
            if (GUILayout.Button("Open editor"))
            {
                dictionary.Editor_Validate();
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
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}