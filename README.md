# SimpleSpawn

Prefab spawning primitives for Unity. SimpleSpawn provides a low-level `SpawnAPI`, tracked single/group/wave spawner components, typed ScriptableObject spawn lists, and an optional despawn-control component.

## Core concepts

| Type | Role |
|---|---|
| `ISpawnableEntity` | Marker contract placed directly on a prefab component that can be instantiated. |
| `SpawnableEntityBase` | Minimal `MonoBehaviour` base that implements `ISpawnableEntity`. |
| `SpawnAPI` | Low-level external `TrySpawn` and `TryDespawn` operations. It accepts a direct `ISpawnableEntity` reference. |
| `SpawnerBase` | Tracks entities spawned by one component and exposes `SpawnedEntities` and despawn control. |
| `SingleSpawnerBase` | Single-entity spawner base. Each `TrySpawn` call generates one entity; timing is owned by the caller or derived component. |
| `GroupSpawnerBase` | Group generation base with overridable `CanSpawnGroup` and `TryGenerateNextGroupEntity` hooks plus `TrySpawnGroup`. |
| `WaveSpawnerBase` | Group-spawner overload with overridable `CanSpawnWave` and `TryGenerateNextWaveEntity` hooks plus `TrySpawnWave`; it has no timing policy. |
| `SpawnListBase<TSpawner>` | Typed ScriptableObject extension point whose callbacks receive the current concrete spawner. |
| `DespawnControlBase` | Optional component for custom despawn validation and cleanup. |

## Quick start

Create a prefab with a component derived from `SpawnableEntityBase` (or implement `ISpawnableEntity` directly on a `Component`):

```csharp
public sealed class EnemyEntity : SpawnableEntityBase { }
```

Create a spawn list asset using **Create > Simple Spawn > Spawn List**, then add the prefab component to its list. Add a custom spawner component and assign the list:

```csharp
public sealed class EnemySpawner : SingleSpawnerBase { }
```

For typed list callbacks, define a concrete list for the concrete spawner:

```csharp
[CreateAssetMenu(menuName = "Game/Enemy Spawn List")]
public sealed class EnemySpawnList : SpawnListBase<EnemySpawner>
{
    [SerializeField] private EnemyEntity _prefab;

    protected override bool CanSpawn(EnemySpawner currentSpawner)
    {
        return _prefab;
    }

    protected override bool TryGenerateNextEntity(
        EnemySpawner currentSpawner, out ISpawnableEntity entity)
    {
        entity = _prefab;
        return _prefab;
    }
}
```

`SpawnerBase` calls `CanSpawn` and `TryGenerateNextEntity` with the current spawner. Every successful result is added to `SpawnedEntities`; destroyed entries are removed the next time the list is accessed. The default spawn list skips missing or destroyed prefab references. The API validates that the interface reference is also a Unity `Component` before instantiating it.

`GroupSpawnerBase.TrySpawnGroup` and `WaveSpawnerBase.TrySpawnWave` generate entities immediately. They do not start coroutines or wait between operations, so a derived component can choose a timer, tick system, animation callback, or another scheduling policy. If generation or spawning fails, entities created during that group or wave are despawned before the failure callback runs. A despawn-control component that rejects rollback can leave an entity tracked and active.

## Calling the low-level API

Use `SpawnAPI` when the caller already owns the prefab reference and does not need spawner policy:

```csharp
OperationResult result = SpawnAPI.TrySpawn(
    enemyPrefab, position, Quaternion.identity, parent, out ISpawnableEntity entity);

if (result)
    SpawnAPI.TryDespawn(entity);
```

An entity must be represented by a prefab component. Runtime code cannot reliably determine whether an arbitrary scene component is a prefab asset, so project setup should keep `ISpawnableEntity` references assigned to prefab assets.

## Despawn control

Add a `DespawnControlBase` implementation to the prefab when despawn needs validation or cleanup:

```csharp
public sealed class EnemyDespawnControl : DespawnControlBase
{
    protected override OperationResult CanDespawn()
    {
        return IsReadyToLeave ? SpawnOperations.Permitted() : SpawnOperations.SpawnNotPermitted();
    }

    protected override void OnDespawn()
    {
        // Release gameplay-owned resources before Destroy runs.
    }
}
```

Without a control component, `SpawnAPI.TryDespawn` destroys the entity's GameObject directly.

## Operation results

`SpawnOperations` uses system code `0x0012` and provides `Permitted`, `Spawned`, `Despawned`, `AllDespawned`, configuration errors, invalid-prefab errors, and invalid-count errors.
