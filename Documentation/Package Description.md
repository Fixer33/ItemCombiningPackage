# Item Combining - Package Description

Package `com.fixer33.item-combining` 2.0.0 (display name "Item Combining"), Unity 6000.0+, MIT license, author Fixer33.
Git URL: `https://github.com/Fixer33/ItemCombiningPackage.git` (from the README and the package's git remote).

## 1. Purpose and users

A small "crafting recipe" data tool. Designers author a `CombinationDictionary` ScriptableObject asset that maps a set of input ScriptableObjects (components) to one or more output ScriptableObjects (results), using a custom UI Toolkit editor window. Game code then asks the dictionary what a set of components makes, with one call. Intended users: the author's own games (assumed); designers use the editor, programmers use the runtime lookup.

The package does not define items. The consumer makes its own ScriptableObject classes that implement `ICombinableComponent` and/or `ICombinableResult`.

## 2. Architecture and main components

| Assembly | Folder | Content |
|---|---|---|
| `com.fixer33.item-combining` | `Runtime/` | Interfaces and the `CombinationDictionary` asset. Root namespace `ItemCombining`. |
| `com.fixer33.item-combining.Editor` | `Editor/` | Inspector and editor window (Editor platform only). Namespace `ItemCombining.Editor`. |
| `com.fixer33.item-combining.Editor.Tests`, `com.fixer33.item-combining.Tests` | `Tests/Editor`, `Tests/Runtime` | See section 7. |

### Runtime
- `ICombinable`: base interface with `string Id { get; }`, a default property `ScriptableObject` (returns `this as ScriptableObject`) and, in the editor only, a default `Editor_Icon` (`Texture2D`, null by default) used by the window.
- `ICombinableComponent : ICombinable`: marker for an input item.
- `ICombinableResult : ICombinable`: marker for an output item. One class may implement both.
- `CombinationDictionary` (`ScriptableObject`, `partial`, create menu `Data/Combination dictionary`):
  - serialized data: `_requireCorrectSequence`, `_objects` (pool of registered items), `_combinations` (list of `SerializedCombinationData` with `Guid`, `Results[]`, `Components[]`), `_possibleInputTypes` and `_possibleOutputTypes` (`TypeReference<T>` arrays from `com.fixer33.core`; empty means no type filter).
  - nested `SerializedCombinationData` struct and `CombinationBinding` flags enum (`Result = 2`, `Component = 4`, `Any = 255`).
- `CombinationDictionaryEditorPart.cs`: second half of the partial class, wrapped in `#if UNITY_EDITOR`; all mutation methods are prefixed `Editor_`.

### Lookup algorithm (`Get`)
Walks `_combinations` in order and returns the results of the first entry whose component count and elements match. When `RequireCorrectSequence` is false, the input is sorted by `Id` descending (string comparison) before matching, and stored entries are expected to be in the same order (`Editor_ModifyCombination` sorts them when adding a new one). When true, input order must equal stored order. Returns `null` when nothing matches; throws `ArgumentException` for a null or empty argument.

### Editor
- `CombinationDictionaryEditor`: custom inspector with a "Requires correct sequence" toggle (editable only while no combinations exist), the possible input/output type fields, an "Open editor" button (validates, then opens the window) and red "Clear all combinations" / "Clear all data" buttons (each with a confirm dialog).
- `CombinationDictionaryWindow` (`EditorWindow`, titled "Combination dictionary"): static `ShowWindow(CombinationDictionary)`; builds its UI from UXML/USS and switches between four panels: `MainWindowPanel` (lists of components and results, usages and delete, add buttons), `ObjectCaptureWindowPanel` (add an item by selecting it in the Project window; validates interface and type filter), `UsagesWindowPanel` (combinations using an item, create new), `EditedCombinationWindowPanel` (edit one combination).
- `WindowPanelBase`: abstract base with `OnShown`, `OnHidden`, `OnUpdate` hooks and a protected `ElementQuery` attribute that binds fields to UXML elements by name via reflection.
- `WindowElements/`: `CombinableGridButton`, `CombinationPreview`, `GridButton`, `ScrollViewCombinableRecord` (custom VisualElements with `Markup/*.uxml` and `Styles/*.uss`; icons in `Textures/`).
- `EditorUtility` (internal): loads package assets from `Packages/com.fixer33.item-combining/Editor/`; with define `PACKAGES_DEV` it prefixes `Assets/` (for development as an embedded package under Assets).

## 3. Public API surface

Runtime entry points:
- `ICombinableResult[] CombinationDictionary.Get(params ICombinableComponent[] components)`
- `SerializedCombinationData[] CombinationDictionary.GetCombinationsWith(ICombinable element, CombinationBinding binding = CombinationBinding.Any)`
- `T[] CombinationDictionary.GetObjects<T>(T except = null)` (registered objects of type T)
- Properties `RequireCorrectSequence`, `PossibleInputTypes`, `PossibleOutputTypes`.

Extension points: implement `ICombinableComponent` / `ICombinableResult` on your own ScriptableObjects; implement `Editor_Icon` (under `#if UNITY_EDITOR`) for a custom icon; use `PossibleInputTypes` / `PossibleOutputTypes` (set in the inspector) to restrict allowed concrete types. The window can be opened from code with `CombinationDictionaryWindow.ShowWindow`.

Editor-only API on the dictionary: `Editor_CanChangeRequireSequence`, `Editor_SetRequireSequence`, `Editor_ClearAllCombinations`, `Editor_ClearEverything`, `Editor_Validate` (also a "Validate" context menu), `Editor_GetObjects`, `Editor_AddObject`, `Editor_RemoveObject`, `Editor_RemoveCombination`, `Editor_ModifyCombination`.

## 4. Dependencies

- Declared in `package.json`: `com.unity.test-framework` 1.4.6 and `com.fixer33.core` 2.2.4 (was 1.0.1, which the author confirmed was outdated) (Git: `https://github.com/Fixer33/PackageCore.git`).
- Asmdef references: the runtime asmdef references `com.fixer33.core` by GUID (`8cc5332f2857d5844834804085116054`) and uses `Core.Utilities.TypeReference<T>` with `.Get()`. The editor asmdef references the runtime asmdef and `com.fixer33.core` by name. Test asmdefs reference the Test Runner assemblies, the runtime asmdef and NUnit, with constraint `UNITY_INCLUDE_TESTS`.
- Implicit: UnityEngine.UIElements and UnityEditor (built-in), `System.Linq`.
- Define symbols: `UNITY_EDITOR` (gates the partial editor half and `Editor_Icon`), `PACKAGES_DEV` (optional, asset paths). No `versionDefines`.

## 5. Editor tools and menu items

No `[MenuItem]` entries. Create the asset via Create > Data > Combination dictionary, select it, press "Open editor" in the Inspector.

## 6. Samples

None. `package.json` has an empty `samples` array and there is no `Samples~` folder.

## 7. Tests

- `Tests/Editor`: `CombinationDictionaryTestBase` (empty input throws `ArgumentException`; unknown combination returns null), `CombinationDictionaryTests` (unordered mode, four input/storage order permutations), `SequenceOrderCombinationDictionaryTests` (ordered mode, correct and wrong order), helper ScriptableObjects `TestComponent` and `TestResult`.
- `Tests/Runtime`: only an asmdef, no tests.
- Run via Window > General > Test Runner (EditMode). Tests were not executed during this documentation pass.

## 8. How other projects consume it

Package Manager > Add package from git URL with the URL above (README). Installation through the author's `com.fixer33.package-installer` is likely (assumed). It needs `com.fixer33.core` (`https://github.com/Fixer33/PackageCore.git`) to be resolvable; UPM does not fetch Git dependencies from a version number alone (assumed that the consumer adds core). Typical use: create item ScriptableObjects implementing the interfaces, create a dictionary asset, author combinations in the window, reference the asset from game code and call `Get`.

## 9. Current state

- Functionally complete for its README scope. 2.0.0 (2025-07-02) added input/output type filtering. CHANGELOG has 2.0.0 and 1.0.0 only.
- No samples, no runtime tests, no TODO or `NotImplementedException` markers in the code.
- Fixed in source (author confirmed; not yet verified in Unity): `Editor_Validate` now sorts unordered entries by `Id` descending, like `Get`. It previously computed the order and never applied it. The `isModified = isModified || ...` short-circuit remains an open question.
