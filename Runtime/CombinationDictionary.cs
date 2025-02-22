using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ItemCombining
{
    /// <summary>
    /// Dictionary that stores all valid combinations
    /// </summary>
    [CreateAssetMenu(fileName = "Combination dictionary", menuName = "Data/Combination dictionary", order = 0)]
    public partial class CombinationDictionary : ScriptableObject
    {
        /// <summary>
        /// Indicates whether the dictionary requires exact order of component in it's combinations
        /// </summary>
        public bool RequireCorrectSequence => _requireCorrectSequence;

        [SerializeField] private bool _requireCorrectSequence;
        [SerializeField] private List<ScriptableObject> _objects = new();
        [SerializeField] private List<SerializedCombinationData> _combinations = new();
        
        public T[] GetObjects<T>(T except = null) where T : ScriptableObject
        {
            return _objects.Except(new[] { except }).OfType<T>().ToArray();
        }
        
        /// <summary>
        /// Get result for combination of the elements
        /// </summary>
        /// <param name="components">Components</param>
        /// <returns>NULL if no combination found. Array of all results for the combination</returns>
        /// <exception cref="ArgumentException">Thrown if array is empty or null</exception>
        public ICombinableResult[] Get(params ICombinableComponent[] components)
        {
            if (components is not { Length: > 0 })
                throw new ArgumentException("No components supplied");
            
            ICombinableComponent[] sortedCombination = _requireCorrectSequence ? components : 
                components.OrderByDescending(i => i.Id).ToArray();

            foreach (var serializedCombinationData in _combinations)
            {
                if (sortedCombination.Length != serializedCombinationData.Components.Length)
                    continue;

                bool isCombinationCorrect = true;
                for (int i = 0; i < sortedCombination.Length; i++)
                {
                    if (sortedCombination[i] == serializedCombinationData.Components[i] as ICombinableComponent)
                        continue;

                    isCombinationCorrect = false;
                    break;
                }
                
                if (isCombinationCorrect == false)
                    continue;

                return serializedCombinationData.Results.OfType<ICombinableResult>().ToArray();
            }
            
            return null;
        }

        public SerializedCombinationData[] GetCombinationsWith(ICombinable element, CombinationBinding binding = CombinationBinding.Any)
        {
            List<SerializedCombinationData> result = new();
            foreach (var combinationData in _combinations)
            {
                if (binding.HasFlag(CombinationBinding.Result) && 
                    combinationData.Results.Contains(element.ScriptableObject))
                {
                    result.Add(combinationData);
                    continue;
                }

                if (binding.HasFlag(CombinationBinding.Component) &&
                    combinationData.Components.Contains(element.ScriptableObject))
                {
                    result.Add(combinationData);
                    continue;
                }
            }

            return result.ToArray();
        }
        
        [Serializable]
        public struct SerializedCombinationData
        {
            public string Guid;
            public ScriptableObject[] Results;
            public ScriptableObject[] Components;

            public SerializedCombinationData(ScriptableObject[] resultInstanceIds, ScriptableObject[] componentsInstanceIds)
            {
                Guid = System.Guid.NewGuid().ToString();
                
                Results = resultInstanceIds;
                Components = componentsInstanceIds;
            }
        }

        [Flags]
        public enum CombinationBinding : short
        {
            Result = 2,
            Component = 4,
            
            Any = 255
        }
    }
}