using UnityEditor;
using UnityEngine;

namespace ItemCombining.Editor
{
    internal static class EditorUtility
    {
        public static T LoadAsset<T>(string path) where T : Object
        {
            string loadPath = "Packages/com.fixer33.item-combining/Editor/" + path;
#if PACKAGES_DEV
            loadPath = "Assets/" + loadPath;
#endif

            return AssetDatabase.LoadAssetAtPath<T>(loadPath);
        }
    }
}