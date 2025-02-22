#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ItemCombining
{
    public partial class CombinationDictionary
    {
        public bool Editor_CanChangeRequireSequence()
        {
            return _combinations.Count == 0;
        }

        public void Editor_SetRequireSequence(bool require)
        {
            if (Editor_CanChangeRequireSequence() == false)
                return;

            _requireCorrectSequence = require;
            Editor_Validate();
            EditorUtility.SetDirty(this);
        }

        public void Editor_ClearAllCombinations()
        {
            if (EditorUtility.DisplayDialog("Confirm", "Are you sure you want to clear all combinations?",
                    "Delete combinations!", "Cancel"))
            {
                _combinations.Clear();
                EditorUtility.SetDirty(this);
            }
        }

        public void Editor_ClearEverything()
        {
            if (EditorUtility.DisplayDialog("Confirm", "Are you sure you want to clear all data from dictionary?",
                    "Delete everything!", "Cancel"))
            {
                _objects.Clear();
                _combinations.Clear();
                EditorUtility.SetDirty(this);
            }
        }

        [ContextMenu("Validate")]
        public void Editor_Validate()
        {
            bool isModified = false;
            
            isModified = isModified || _objects.RemoveAll(i => i == null) > 0;
            
            for (var i = 0; i < _combinations.Count; i++)
            {
                var combination = _combinations[i];
                var buf = combination.Results.ToList();
                isModified = isModified || buf.RemoveAll(ii => ii == null) > 0;
                combination.Results = buf.ToArray();

                buf = combination.Components.ToList();
                isModified = isModified || buf.RemoveAll(ii => ii == null) > 0;
                combination.Components = buf.ToArray();
                _combinations[i] = combination;
            }
            
            isModified = isModified || _combinations.RemoveAll(i => i.Components.Length <= 0 && i.Results.Length <= 0) > 0;

            if (_requireCorrectSequence == false)
            {
                for (var i = 0; i < _combinations.Count; i++)
                {
                    var combination = _combinations[i];
                
                    var buf = combination.Results.ToList();
                    isModified = isModified || buf.OrderByDescending(ii => ii.name).Count() != combination.Results.Length;
                    combination.Results = buf.ToArray();

                    buf = combination.Components.ToList();
                    isModified = isModified || buf.OrderByDescending(ii => ii.name).Count() != combination.Components.Length;
                    combination.Components = buf.ToArray();
                
                    _combinations[i] = combination;
                }
            }
            
            if (isModified)
                EditorUtility.SetDirty(this);
        }
        
        private void Editor_SortObjects()
        {
            _objects = _objects.OrderByDescending(i => i.name).ToList();
            EditorUtility.SetDirty(this);
        }
        
        public ScriptableObject[] Editor_GetObjects() => _objects.ToArray();
        
        public void Editor_AddObject(ICombinable so)
        {
            if (_objects.Contains(so.ScriptableObject)) 
                return;
            
            _objects.Add(so.ScriptableObject);
            Editor_SortObjects();
            Editor_Validate();
            EditorUtility.SetDirty(this);
        }

        public void Editor_RemoveObject(ScriptableObject so)
        {
            if (_objects.Contains(so) == false)
                return;

            _objects.Remove(so);
            Editor_SortObjects();
            Editor_Validate();
            EditorUtility.SetDirty(this);
        }

        public void Editor_RemoveCombination(SerializedCombinationData combination)
        {
            int combinationIndex = -1;
            for (var i = 0; i < _combinations.Count; i++)
            {
                if (_combinations[i].Guid.Equals(combination.Guid) == false)
                    continue;

                combinationIndex = i;
            }
            
            if (combinationIndex < 0)
                return;

            _combinations.RemoveAt(combinationIndex);
            Editor_Validate();
            EditorUtility.SetDirty(this);
        }
        
        public void Editor_ModifyCombination(SerializedCombinationData editedCombination)
        {
            for (int i = 0; i < _combinations.Count; i++)
            {
                if (_combinations[i].Guid.Equals(editedCombination.Guid) == false)
                    continue;

                _combinations[i] = editedCombination;
                Editor_Validate();
                EditorUtility.SetDirty(this);
                return;
            }

            if (_requireCorrectSequence)
            {
                _combinations.Add(new SerializedCombinationData(
                    editedCombination.Results.Where(i => i != null).ToArray(),
                    editedCombination.Components.Where(i => i != null).ToArray()));
            }
            else
            {
                _combinations.Add(new SerializedCombinationData(
                    editedCombination.Results.Where(i => i is ICombinable).OrderByDescending(i => ((ICombinable)i).Id).ToArray(),
                    editedCombination.Components.Where(i => i is ICombinable).OrderByDescending(i => ((ICombinable)i).Id).ToArray()));
            }
            Editor_Validate();
            EditorUtility.SetDirty(this);
        }
    }
}
#endif