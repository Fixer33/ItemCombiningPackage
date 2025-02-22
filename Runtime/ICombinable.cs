using UnityEngine;

namespace ItemCombining
{
    public interface ICombinable
    {
        public ScriptableObject ScriptableObject => this as ScriptableObject;
        
        public string Id { get; }
        
#if UNITY_EDITOR
        public virtual Texture2D Editor_Icon => null;
#endif
    }
}