using System.Linq;
using NUnit.Framework;

namespace ItemCombining.Editor.Tests
{
    public class CombinationDictionaryTests : CombinationDictionaryTestBase
    {
        [Test]
        public void GetResult_ExistentCombination_SaveSpecificOrder_InputSpecificOrder_ReturnsResult()
        {
            InitializeDictionaryWithObjectsAndResults(out var dictionary, out var obj1, out var obj2, out var obj3,
                out var result1, out var result2);
            
            dictionary.Editor_ModifyCombination(new CombinationDictionary.SerializedCombinationData()
            {
                Results = new[] { result1.ScriptableObject, result2.ScriptableObject,  },
                Components = new[]{ obj1.ScriptableObject, obj2.ScriptableObject, obj3.ScriptableObject }
            });
            dictionary.Editor_Validate();
            dictionary.Editor_Validate();

            try
            {
                var result = dictionary.Get(obj1, obj2, obj3);
                Assert.IsTrue(result.Contains(result1) && result.Contains(result2));
            }
            finally
            {
                DestroyObjects(dictionary, obj1, obj2, obj3, result1, result2);
            }
        }
        
        [Test]
        public void GetResult_ExistentCombination_SaveSpecificOrder_InputRandomOrder_ReturnsResult()
        {
            InitializeDictionaryWithObjectsAndResults(out var dictionary, out var obj1, out var obj2, out var obj3,
                out var result1, out var result2);
            
            dictionary.Editor_ModifyCombination(new CombinationDictionary.SerializedCombinationData()
            {
                Results = new[] { result1.ScriptableObject, result2.ScriptableObject },
                Components = new[] { obj1.ScriptableObject, obj2.ScriptableObject, obj3.ScriptableObject }
            });
            dictionary.Editor_Validate();
            dictionary.Editor_Validate();

            try
            {
                var result = dictionary.Get(obj3, obj1, obj2);
                Assert.IsTrue(result.Contains(result1) && result.Contains(result2));
            }
            finally
            {
                DestroyObjects(dictionary, obj1, obj2, obj3, result1, result2);
            }
        }
        
        [Test]
        public void GetResult_ExistentCombination_SaveRandomOrder_InputSpecificOrder_ReturnsResult()
        {
            InitializeDictionaryWithObjectsAndResults(out var dictionary, out var obj1, out var obj2, out var obj3,
                out var result1, out var result2);
            
            dictionary.Editor_ModifyCombination(new CombinationDictionary.SerializedCombinationData()
            {
                Results = new[] { result1.ScriptableObject, result2.ScriptableObject },
                Components = new[] { obj2.ScriptableObject, obj3.ScriptableObject, obj1.ScriptableObject }
            });
            dictionary.Editor_Validate();
            dictionary.Editor_Validate();

            try
            {
                var result = dictionary.Get(obj1, obj2, obj3);
                Assert.IsTrue(result.Contains(result1) && result.Contains(result2));
            }
            finally
            {
                DestroyObjects(dictionary, obj1, obj2, obj3, result1, result2);
            }
        }
        
        [Test]
        public void GetResult_ExistentCombination_SaveRandomOrder_InputRandomOrder_ReturnsResult()
        {
            InitializeDictionaryWithObjectsAndResults(out var dictionary, out var obj1, out var obj2, out var obj3,
                out var result1, out var result2);
            
            dictionary.Editor_ModifyCombination(new CombinationDictionary.SerializedCombinationData()
            {
                Results = new[] { result1.ScriptableObject, result2.ScriptableObject },
                Components = new[] { obj2.ScriptableObject, obj3.ScriptableObject, obj1.ScriptableObject }
            });
            dictionary.Editor_Validate();
            dictionary.Editor_Validate();

            try
            {
                var result = dictionary.Get(obj3, obj1, obj2);
                Assert.IsTrue(result.Contains(result1) && result.Contains(result2));
            }
            finally
            {
                DestroyObjects(dictionary, obj1, obj2, obj3, result1, result2);
            }
        }
    }
}