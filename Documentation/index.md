# VMGames Save System

VMGames Save System provides a simple API for converting game data to text and storing or retrieving it at a path specified by the application. The package includes a service independent of the data format and storage destination, a local file strategy, and a JSON serializer based on `UnityEngine.JsonUtility`.

## Installation

Install the package through the Unity Package Manager using the source provided by the project (for example, a Git URL or a local package). The manifest declares Unity `6000.5.10f1` as the minimum version and Unity Test Framework `1.6.0` as a dependency.

## Usage

Create the service using the included implementations:

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using VMGames.Core.SaveSystem;
using VMGames.SaveSystem.Unity;

[Serializable]
public class PlayerSaveData
{
    public int Health;
    public Vector3 Position;
}

public class PlayerSave
{
    private readonly SaveService _saveService =
        new SaveService(new FileSystemStorageStrategy(), new UnityJsonSerializer());

    private string SavePath =>
        Path.Combine(Application.persistentDataPath, "player-save.json");

    public Task SaveAsync(PlayerSaveData data)
    {
        return _saveService.SaveAsync(SavePath, data);
    }

    public Task<PlayerSaveData> LoadAsync()
    {
        var defaultData = new PlayerSaveData { Health = 100 };
        return _saveService.LoadAsync(SavePath, defaultData);
    }
}
```

Call the asynchronous methods with `await` from your application flow. `Application.persistentDataPath` provides a platform-specific persistent location. `FileSystemStorageStrategy` does not create directories: if the parent directory does not exist, create it before calling `SaveAsync`.

## Components and API

The types are available in these namespaces:

| Type | Namespace | Description |
|---|---|---|
| `SaveService` | `VMGames.Core.SaveSystem` | Coordinates conversion to and from text and storage operations. |
| `ISerializer` | `VMGames.Core.SaveSystem` | Defines `Serialize<T>` and `Deserialize<T>`; implement it to use another format. |
| `IStorageStrategy` | `VMGames.Core.SaveSystem` | Defines read, write, existence-check, and delete operations; implement it to use another destination. |
| `FileSystemStorageStrategy` | `VMGames.SaveSystem.Unity` | Implements `IStorageStrategy` with local file operations. |
| `UnityJsonSerializer` | `VMGames.SaveSystem.Unity` | Implements `ISerializer` using `JsonUtility.ToJson` and `JsonUtility.FromJson<T>`. |

### `SaveService`

- `SaveAsync<T>(string path, T data)` serializes `data` and writes the text to the specified path.
- `LoadAsync<T>(string path, T defaultValue = default)` returns `defaultValue` if the file does not exist or the content read is empty; otherwise, it deserializes the content as `T`.
- Both methods throw `ArgumentException` if `path` is `null` or empty. Serialization, read, and write exceptions are not caught by the service.

### `IStorageStrategy`

An implementation must provide `WriteTextAsync`, `ReadTextAsync`, `Exists`, and `Delete`. The included strategy reads and writes files on background tasks. `Exists` and `Delete` are synchronous; `Delete` does nothing if the file does not exist.

## Data format and data types

By default, data is serialized with `JsonUtility`. Supplied types must follow Unity serialization rules, including the requirement for `[Serializable]` on ordinary serializable classes and structs. Refer to Unity documentation for supported field types and `JsonUtility` limitations. For other formats, implement `ISerializer` and inject it into `SaveService`.

Paths are used as provided: the service does not choose directories, create folders, or convert relative paths into persistent paths. For player data, consider building the path from `Application.persistentDataPath`.

## Known limitations

- The package does not provide encryption, cloud synchronization, save versioning or migration, backups, or atomic writes.
- The file implementation does not create parent directories.
- The service does not validate or recover invalid content; deserialization and I/O failures follow the behavior of the serializer and platform.
- The default serializer inherits the limitations of `JsonUtility`.

## Package contents

| Location | Contents |
|---|---|
| `Runtime/` | Service, interfaces, and file and JSON implementations for Unity. |
| `Documentation/` | This documentation. |
| `Samples/Example/` | Basic example included with the package; see the README for the save system usage flow. |
| `Tests/` | Package tests, including a disk write/read integration test. |

## Documentation history

| Date | Change |
|---|---|
| 2026-10-28 | Initial documentation for version 0.1.0. |
