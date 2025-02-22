using System;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ItemCombining.Editor.Tests
{
    public abstract class CombinationDictionaryTestBase
    {
        protected void InitializeDictionaryWithObjects(out CombinationDictionary dictionary, 
            out ICombinableComponent obj1, out ICombinableComponent obj2, out ICombinableComponent obj3)
        {
            dictionary = ScriptableObject.CreateInstance<CombinationDictionary>();
            obj1 = ScriptableObject.CreateInstance<TestComponent>().Validate();
            obj2 = ScriptableObject.CreateInstance<TestComponent>().Validate();
            obj3 = ScriptableObject.CreateInstance<TestComponent>().Validate();
        }
        
        protected void InitializeDictionaryWithObjectsAndResults(out CombinationDictionary dictionary, 
            out ICombinableComponent obj1, out ICombinableComponent obj2, out ICombinableComponent obj3, 
            out ICombinableResult result1, out ICombinableResult result2)
        {
            InitializeDictionaryWithObjects(out dictionary, out obj1, out obj2, out obj3);
            dictionary.Editor_AddObject(obj1);
            dictionary.Editor_AddObject(obj2);
            dictionary.Editor_AddObject(obj3);
            
            result1 = ScriptableObject.CreateInstance<TestResult>().Validate();
            result2 = ScriptableObject.CreateInstance<TestResult>().Validate();
            dictionary.Editor_AddObject(result1);
            dictionary.Editor_AddObject(result2);
        }

        protected void DestroyObjects(ScriptableObject so, params ICombinable[] objects)
        {
            Object.DestroyImmediate(so);
            for (var i = 0; i < objects.Length; i++)
            {
                Object.DestroyImmediate(objects[i].ScriptableObject);
            }
        }
        
        [Test]
        public void GetResult_NoComponents_ThrowsException()
        {
            CombinationDictionary dictionary = ScriptableObject.CreateInstance<CombinationDictionary>();

            try
            {
                Assert.Throws<ArgumentException>(() => dictionary.Get());
            }
            finally
            {
                Object.DestroyImmediate(dictionary);
            }
        }
        
        [Test]
        public void GetResult_NonExistentCombination_ReturnsNull()
        {
            InitializeDictionaryWithObjects(out var dictionary, out var obj1, out var obj2, out var obj3);
            
            try
            {
                Assert.IsNull(dictionary.Get(obj1, obj2, obj3));
            }
            finally
            {
                DestroyObjects(dictionary, obj1, obj2, obj3);
            }
        }
    }
}