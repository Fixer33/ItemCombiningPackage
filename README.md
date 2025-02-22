# Item Combining

![Unity](https://img.shields.io/badge/Unity-UPM%20Package-blue)
![GitHub](https://img.shields.io/github/license/Fixer33/ItemCombiningPackage)

Item Combining is a Unity Editor tool that provides a structured way to manage combinations of `ScriptableObject` assets. It introduces the **CombinationDictionary**, a custom data structure used to store and retrieve valid combinations. The main feature of this package is its **Editor Window UI**, which allows for an intuitive way to edit and manage combinations.

## Features

- Custom **CombinationDictionary** to store valid ScriptableObject combinations.
- Simple API with a single method for retrieving valid combinations.
- User-friendly **Editor Window UI** to edit combinations.
- Used with **UPM (Unity Package Manager)**.

## Installation

### Using UPM (Unity Package Manager)

1. Open Unity and go to **Window > Package Manager**.
2. Click the **+** button and select **Add package from git URL**.
3. Enter the repository URL:
   ```
   https://github.com/Fixer33/ItemCombiningPackage.git
   ```
4. Click **Add** and wait for Unity to install the package.

## Usage

### Creating a Combination Dictionary
To create a new **CombinationDictionary**, right-click in the Project window and select:

**Create > Data > Combination Dictionary**

### Accessing the Editor Window
Once you have a **CombinationDictionary** selected, open the editor UI by clicking the **Open Editor** button in the **Inspector**.

### Retrieving a Combination
To get a valid combination from a `CombinationDictionary`, call the `Get` method on an instance of the dictionary:

```csharp
ICombinableResult[] results = combinationDictionary.Get(component1, component2);
```

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

Manage and edit ScriptableObject combinations easily with Item Combining!

