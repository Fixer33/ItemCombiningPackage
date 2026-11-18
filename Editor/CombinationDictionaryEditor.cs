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
        
        private static Texture2D _primaryTex;
        private static Texture2D _primaryHoverTex;

        private static Texture2D MakeTex(Color color)
        {
            var tex = new Texture2D(1, 1) { hideFlags = HideFlags.HideAndDontSave };
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }

        private static void DrawOpenEditorButton(CombinationDictionary dictionary)
        {
            if (_primaryTex == null) _primaryTex = MakeTex(new Color32(0x3B, 0x82, 0xF6, 255));
            if (_primaryHoverTex == null) _primaryHoverTex = MakeTex(new Color32(0x5B, 0x9B, 0xFF, 255));

            var style = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold,
                fixedHeight = 28,
                alignment = TextAnchor.MiddleCenter
            };
            style.normal.background = _primaryTex;
            style.hover.background = _primaryHoverTex;
            style.active.background = _primaryHoverTex;
            style.normal.textColor = style.hover.textColor = style.active.textColor = Color.white;

            if (GUILayout.Button("Open editor", style))
            {
                dictionary.Editor_Validate();
                CombinationDictionaryWindow.ShowWindow(dictionary);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            if (target is not CombinationDictionary dictionary)
                return;

            EditorGUILayout.LabelField("Combination Dictionary", EditorStyles.boldLabel);
            var combinationsProp = serializedObject.FindProperty("_combinations");
            if (combinationsProp != null)
                EditorGUILayout.LabelField($"{combinationsProp.arraySize} combinations defined", EditorStyles.miniLabel);

            DrawOpenEditorButton(dictionary);
            GUILayout.Space(8);

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