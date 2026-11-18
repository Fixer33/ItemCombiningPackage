# Item Combining - Open Questions

## Behaviour and correctness
2. **Short-circuit in `Editor_Validate`.** `isModified = isModified || X` stops evaluating X once `isModified` is true, so later null-removals are skipped in that pass. The tests call `Editor_Validate` twice, which may hide it. Intentional?
3. **Sort key mismatch.** `Get` and `Editor_ModifyCombination` sort by `Id` (string, descending), while `Editor_SortObjects` and `Editor_Validate` use `name`. If an item's `Id` changes after authoring, lookups would silently fail. Is `Id` meant to be immutable?
4. **Duplicates.** `Get` returns the first match and no duplicate check was found in the code read. The editor panels were only skimmed; is duplicate prevention expected there?
5. **Null elements in `Get`.** A null inside `components` would raise a `NullReferenceException` on `Id` in unordered mode; only null or empty arrays are documented as errors.
6. **`CombinationBinding` values** (`Result = 2`, `Component = 4`, `Any = 255`, `short`): why these values?

## Dependencies
7. **Core supply.** Is the package installer expected to supply core, since UPM cannot resolve a Git dependency from a version number alone?
8. **GUID vs name references.** The runtime asmdef references core by GUID, the editor asmdef by name. Intentional?
9. **Heavy dependency for a small feature.** Only `TypeReference<T>` is used from core. Intended?

## Documentation and release
10. **README vs code.** The README says the API is a single method; the code also exposes `GetCombinationsWith` and `GetObjects`. The README does not mention the type filter (2.0.0) or the sequence toggle.
11. **Menu path casing.** README says "Create > Data > Combination Dictionary"; code has `Data/Combination dictionary`.
12. **CHANGELOG.** It still contains template placeholder comments. 2.0.0 is a major bump with only "filtering possible input and output component types"; does it break data compatibility with 1.0.0?
13. **No samples, no runtime tests.** `Tests/Runtime` has only an asmdef and `samples` is empty. Intentional or planned?
14. **`PACKAGES_DEV` define.** Nothing in the package defines it; presumably the development project does. Not documented.
15. **Window state.** `CombinationDictionaryWindow` keeps the dictionary in a static field, so after a domain reload or editor restart the window may lose it (assumed, not tested). Intended?

## Answered
- **`Editor_Validate` sorting is a no-op.** Author: yes, fix it. Status: fixed in source (not yet verified in Unity). Intended change: sort Results and Components by `Id` descending, as `Get` does.
- **`com.fixer33.core` 1.0.1 pin is outdated.** Author: yes, update it. Status: fixed in source; `package.json` now pins 2.2.4.
