using System;
using UnityEngine;

namespace ItemCombining.Editor.Tests
{
    [CreateAssetMenu(fileName = "Test component", menuName = "Tests/TestComponent", order = 0)]
    public class TestComponent : ScriptableObject, ICombinableComponent
    {
        public string Id => _id;
        
        [SerializeField] private string _id;

        private void OnValidate()
        {
            Validate();
        }

        public TestComponent Validate()
        {
            if (string.IsNullOrEmpty(_id))
                _id = Guid.NewGuid().ToString();
            return this;
        }
    }
}