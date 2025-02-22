using System.Linq;
using NUnit.Framework;

namespace ItemCombining.Editor.Tests
{
    public class SequenceOrderCombinationDictionaryTests : CombinationDictionaryTestBase
    {
        [Test]
        public void GetResult_ExistentCombination_CorrectOrder_ReturnsResult()
        {
            InitializeDictionaryWithObjectsAndResults(out var dictionary, out var obj1, out var obj2, out var obj3,
                out var result1, out var result2);
            dictionary.Editor_SetRequireSequence(true);
            
            dictionary.Editor_ModifyCombination(new CombinationDictionary.SerializedCombinationData()
            {
                Results = new[] { result1.ScriptableObject, result2.ScriptableObject },
                Components = new[] { obj3.ScriptableObject, obj1.ScriptableObject, obj2.ScriptableObject }
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
        public void GetResult_ExistentCombination_WrongOrder_ReturnsNull()
        {
            InitializeDictionaryWithObjectsAndResults(out var dictionary, out var obj1, out var obj2, out var obj3,
                out var result1, out var result2);
            dictionary.Editor_SetRequireSequence(true);
            
            dictionary.Editor_ModifyCombination(new CombinationDictionary.SerializedCombinationData()
            {
                Results = new[] { result1.ScriptableObject, result2.ScriptableObject },
                Components = new[] { obj3.ScriptableObject, obj1.ScriptableObject, obj2.ScriptableObject }
            });
            dictionary.Editor_Validate();
            dictionary.Editor_Validate();

            try
            {
                Assert.IsNull(dictionary.Get(obj1, obj3, obj2));
            }
            finally
            {
                DestroyObjects(dictionary, obj1, obj2, obj3, result1, result2);
            }
        }
    }
}