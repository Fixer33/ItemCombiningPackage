using System;
using UnityEngine;

namespace ItemCombining.Editor.Tests
{
    [CreateAssetMenu(fileName = "Test result", menuName = "Tests/TestResult", order = 0)]
    public class TestResult : ScriptableObject, ICombinableResult
    {
        public string Id => _id;
        
        [SerializeField] private string _id;

        private void OnValidate()
        {
            Validate();
        }

        public TestResult Validate()
        {
            if (string.IsNullOrEmpty(_id))
                _id = Guid.NewGuid().ToString();
            return this;
        }
    }
}