# VMGames Save System

Save and load game data as text using a generic service and replaceable storage and serialization components. The included implementation writes local files and uses Unity's `JsonUtility`.

## Requirements

- Unity `6000.5.10f1` or later.
- Declared package dependency: Unity Test Framework `1.6.0`.

## Installation

Install the package through the Unity Package Manager using the source provided by the project (for example, a Git URL or a local package). No additional setup is required.

## Quick start

Use a full path under `Application.persistentDataPath` to store save data in a persistent, platform-appropriate location:

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

public class SaveExample
{
    private readonly SaveService _saveService =
        new SaveService(new FileSystemStorageStrategy(), new UnityJsonSerializer());

    private string SavePath =>
        Path.Combine(Application.persistentDataPath, "player-save.json");

    public async Task SavePlayerAsync(PlayerSaveData data)
    {
        await _saveService.SaveAsync(SavePath, data);
    }

    public async Task<PlayerSaveData> LoadPlayerAsync()
    {
        return await _saveService.LoadAsync(
            SavePath,
            new PlayerSaveData { Health = 100 });
    }
}
```

Create the file's parent directory before saving if it does not already exist. The file strategy reads and writes on background tasks but does not create directories.

## Included API

- `SaveService`: coordinates serialization and storage through `ISerializer` and `IStorageStrategy`.
- `ISerializer` and `IStorageStrategy`: contracts for replacing the data format or storage destination.
- `FileSystemStorageStrategy`: reads, writes, checks, and deletes local files.
- `UnityJsonSerializer`: serializes and deserializes using `UnityEngine.JsonUtility`.

See the [documentation](Documentation/index.md) for API details, behavior, and limitations.

## Changelog

See [CHANGELOG.md](CHANGELOG.md).
