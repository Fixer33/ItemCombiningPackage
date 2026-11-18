# Item Combining - Usage Guide

Package `com.fixer33.item-combining` 2.0.0, Unity 6000.0+. Items marked (assumed) were inferred rather than read directly.

The package gives you a `CombinationDictionary` ScriptableObject that maps a set of input ScriptableObjects (components) to one or more output ScriptableObjects (results), a UI Toolkit editor window to author those mappings, and a one-call runtime lookup. The package does not ship any item types; you write your own.

## 1. Installation

### Via com.fixer33.package-installer (recommended)
1. Install `com.fixer33.package-installer` (`https://github.com/Fixer33/PackageInstaller.git` (assumed from that package's git remote `git@github.com:Fixer33/PackageInstaller.git`; the installer is not listed in its own packages-list.json)).
2. Open **Tools > Fixer33 > Package Installer**.
3. Pick "Item Combining" (package id `com.fixer33.item-combining`). Its installer entry declares one dependency, `com.fixer33.core`, so the installer installs core as well.

Installer entry (from `packages-list.json`, group "Tools"): `PackageUrl` = `https://github.com/Fixer33/ItemCombiningPackage.git`, `Dependencies` = `com.fixer33.core`.

### Manually
Window > Package Manager > + > Add package from git URL:

```
https://github.com/Fixer33/ItemCombiningPackage.git
```

`package.json` declares `com.fixer33.core` 2.2.4 and `com.unity.test-framework` 1.4.6. Unity cannot resolve a Git package from a version number alone, so if core is missing add it yourself: `https://github.com/Fixer33/PackageCore.git` (assumed; this is the usual UPM behaviour).

## 2. Minimal setup

1. Create your item classes. Every item must be a `ScriptableObject` that implements `ICombinableComponent` (input), `ICombinableResult` (output), or both. The only member you must implement is `string Id { get; }`:

```csharp
using System;
using ItemCombining;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject, ICombinableComponent, ICombinableResult
{
    public string Id => _id;
    [SerializeField] private string _id;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_id))
            _id = Guid.NewGuid().ToString();   // stable, unique id
    }
}
```

(This follows the package's own test helpers `TestComponent` / `TestResult`.)

2. Create item assets from your own create menu.
3. Create the dictionary: Project window right-click > **Create > Data > Combination dictionary**.
4. Select the dictionary asset and click **Open editor** in the Inspector (see section 4).
5. In the window add your items and author combinations.
6. Reference the dictionary asset from a MonoBehaviour or another asset and call `Get` (section 3).

## 3. Using the public API

Namespace `ItemCombining`.

### Look up a combination
```csharp
using ItemCombining;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    [SerializeField] private CombinationDictionary _dictionary;
    [SerializeField] private Item _a, _b;

    public void Craft()
    {
        ICombinableResult[] results = _dictionary.Get(_a, _b);
        if (results == null)
        {
            Debug.Log("No recipe");
            return;
        }
        foreach (var r in results)
            Debug.Log(r.Id);
    }
}
```
`ICombinableResult[] Get(params ICombinableComponent[] components)`:
- Throws `ArgumentException` for a null or empty argument.
- Returns `null` when no combination matches. The first matching entry wins.
- If `RequireCorrectSequence` is false, the input is sorted by `Id` descending before matching (so argument order does not matter). If true, the argument order must equal the stored order.
- The array contains all results of the matching combination.

### Find combinations that use an item
```csharp
var all    = _dictionary.GetCombinationsWith(item);                                   // CombinationBinding.Any
var asPart = _dictionary.GetCombinationsWith(item, CombinationDictionary.CombinationBinding.Component);
var asOut  = _dictionary.GetCombinationsWith(item, CombinationDictionary.CombinationBinding.Result);
```
Returns `CombinationDictionary.SerializedCombinationData[]` (fields `Guid`, `Results`, `Components`).

### Other members
- `T[] GetObjects<T>(T except = null) where T : ScriptableObject`: the items registered in the dictionary that are of type `T`.
- Properties `RequireCorrectSequence` (bool), `PossibleInputTypes`, `PossibleOutputTypes` (arrays of `Core.Utilities.TypeReference<...>`).

### Editor-only members (inside `#if UNITY_EDITOR`, prefixed `Editor_`)
`Editor_CanChangeRequireSequence`, `Editor_SetRequireSequence`, `Editor_ClearAllCombinations`, `Editor_ClearEverything`, `Editor_Validate` (also the "Validate" context menu on the asset), `Editor_GetObjects`, `Editor_AddObject`, `Editor_RemoveObject`, `Editor_RemoveCombination`, `Editor_ModifyCombination`. You normally use the window instead; they exist for editor tooling and tests. Do not call them from runtime code (they are stripped from builds).

## 4. Editor windows and menu paths

- **Create > Data > Combination dictionary** creates the asset (the README writes "Combination Dictionary"; the code says "Combination dictionary").
- There are no `[MenuItem]` entries in this package.
- **Inspector of a CombinationDictionary** (`CombinationDictionaryEditor`):
  - "Requires correct sequence" toggle. It is editable only while the dictionary has no combinations; otherwise a label says the dictionary must be empty to change it.
  - "Possible Input Types" and "Possible Output Types" lists.
  - **Open editor** button: runs `Editor_Validate`, then opens the window.
  - Red **Clear all combinations** and **Clear all data** buttons, each with a confirm dialog ("Clear all data" also removes the registered item pool).
- **Combination dictionary window** (`CombinationDictionaryWindow`, opened with the static `CombinationDictionaryWindow.ShowWindow(CombinationDictionary)`, namespace `ItemCombining.Editor`). It switches between four panels:
  - Main panel: lists of registered components and results, with usages, delete and add buttons.
  - Object capture panel: add an item by selecting it in the Project window. It checks the interface and the possible-type filter.
  - Usages panel: combinations that use a chosen item, and creating a new one.
  - Edited combination panel: edit one combination.

## 5. Configuration and scripting defines

Per-asset configuration (Inspector):
- Requires correct sequence: off by default. Decide before authoring; it can only be changed on an empty dictionary.
- Possible Input Types / Possible Output Types: restrict which concrete types the capture panel accepts. Empty means no filter. The check compares the exact type (`!=` against `TypeReference.Get()`), so a base class or interface entry does not match derived classes (read from `ObjectCaptureWindowPanel`; the matching is on the item's concrete type).

Defines:
- `UNITY_EDITOR`: gates the `Editor_*` half of the dictionary and the `Editor_Icon` member.
- `PACKAGES_DEV` (optional): makes the internal asset loader prefix `Assets/` to the package path, for developing the package as an embedded package under `Assets/`. Nothing in the package defines it; do not set it in normal consumer projects.
- No `versionDefines` are used.

## 6. Extension points and a worked example

- Implement `ICombinableComponent` / `ICombinableResult` on your own ScriptableObjects (any number of types).
- Override `Editor_Icon` (default interface member, `#if UNITY_EDITOR`) to give an item an icon in the window.
- Restrict allowed types through the Inspector type lists.
- Open the window from your own code with `CombinationDictionaryWindow.ShowWindow`.

Worked example: an ingredient that can also be a result, with a custom icon in the window.

```csharp
using System;
using ItemCombining;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Ingredient")]
public class Ingredient : ScriptableObject, ICombinableComponent, ICombinableResult
{
    [SerializeField] private string _id;
    [SerializeField] private Texture2D _icon;

    public string Id => _id;

#if UNITY_EDITOR
    public Texture2D Editor_Icon => _icon;
#endif

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_id)) _id = Guid.NewGuid().ToString();
    }
}
```
Then in the dictionary Inspector add `Ingredient` to Possible Input Types and Possible Output Types (optional), open the window, add ingredient assets and combine them. (That `Editor_Icon` is honoured on classes implementing the default interface member is (assumed) from the interface definition; the window code was not traced for it.)

## 7. Common pitfalls (current behaviour)

- `Id` must be stable and unique. `Get` and `Editor_ModifyCombination` order by `Id` (string, descending), so changing an item's `Id` after authoring can make lookups fail silently in unordered mode; re-author the combinations.
- Empty or null `components` throws `ArgumentException`. A null element inside the array is not checked and would throw a `NullReferenceException` in unordered mode (read from code).
- Duplicates: `Get` returns the first match; no duplicate-combination check was found in the code read, so two identical recipes give the first one's result.
- `Editor_Validate` now sorts entries by `Id` in unordered mode (fixed in source, not yet verified in Unity). Previously its sort was computed and never applied. Also, `isModified = isModified || ...` short-circuits, so later cleanups in one pass may be skipped; running validate again (it runs every time you click Open editor) catches them.
- Switching "Requires correct sequence" is only possible on an empty dictionary. Use "Clear all combinations" first.
- Item sorting in the pool uses `name`, combinations use `Id`; do not rely on the two being in the same order.
- The window keeps its dictionary in a static field; after a domain reload or editor restart it may lose the reference. Re-open through the Inspector button (assumed, not tested).
- `package.json` pins `com.fixer33.core` 2.2.4 (updated from 1.0.1, which the author confirmed was outdated). Use a core version that contains `Core.Utilities.TypeReference<T>` (assumed any current version).
- `GetCombinationsWith` does not null-check `element`.
- The package has no samples and no runtime tests. Editor tests live in `Tests/Editor` (Window > General > Test Runner, EditMode).

## 8. Working with other com.fixer33.* packages

- `com.fixer33.core` - `https://github.com/Fixer33/PackageCore.git`. Required: provides `TypeReference<T>` used for the type filters.
- `com.fixer33.package-installer` - installs this package and its core dependency (see section 1).

No other `com.fixer33.*` package is referenced by this package's code.

## 9. Remaining open questions

From the package's Open Questions.md (still open and affecting usage):
- Q2: short-circuit in `Editor_Validate` (`isModified = isModified || ...`); intentional?
- Q3: sort key mismatch (`Id` vs `name`); is `Id` meant to be immutable?
- Q4: duplicate combinations: is prevention expected in the editor panels?
- Q5: null elements passed to `Get`: intended to throw `NullReferenceException`?
- Q7: who supplies `com.fixer33.core` (installer vs consumer), since UPM cannot resolve it from a version number.
- Q11: create menu casing ("Combination Dictionary" in README vs "Combination dictionary" in code).
- Q12: does 2.0.0 break data compatibility with 1.0.0?
- Q14: `PACKAGES_DEV` define is undocumented/undefined in the package.
- Q15: window state lost after domain reload (static dictionary field).

Applied in source (not yet verified in Unity): `Editor_Validate` sorting, and the `com.fixer33.core` pin (2.2.4).
