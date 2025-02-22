using UnityEngine;

namespace ItemCombining
{
    /// <summary>
    /// Base interface for all scriptable objects that are used in CombinationDictionary
    /// </summary>
    public interface ICombinable
    {
        public ScriptableObject ScriptableObject => this as ScriptableObject;
        
        public string Id { get; }
        
#if UNITY_EDITOR
        public virtual Texture2D Editor_Icon => null;
#endif
    }
}