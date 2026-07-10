using System.Collections.Generic;
using Systems.SimpleSpawn.Components;
using UnityEngine;

namespace Systems.SimpleSpawn.Abstract
{
    /// <summary>
    ///     Non-generic bridge used by spawner components to store typed spawn lists.
    /// </summary>
    public abstract class SpawnListBase : ScriptableObject
    {
        internal abstract bool CanSpawn(SpawnerBase currentSpawner);

        internal abstract bool TryGenerateNextEntity(
            SpawnerBase currentSpawner, out ISpawnableEntity entity);
    }

    /// <summary>
    ///     Spawn list whose callbacks receive the concrete current spawner type.
    /// </summary>
    /// <typeparam name="TSpawner">Concrete spawner type that owns the list.</typeparam>
    public abstract class SpawnListBase<TSpawner> : SpawnListBase
        where TSpawner : SpawnerBase
    {
        /// <summary>
        ///     Checks whether this list can generate an entity for the current spawner.
        /// </summary>
        protected abstract bool CanSpawn(TSpawner currentSpawner);

        /// <summary>
        ///     Generates the next prefab for the current spawner.
        /// </summary>
        protected abstract bool TryGenerateNextEntity(
            TSpawner currentSpawner, out ISpawnableEntity entity);

        internal sealed override bool CanSpawn(SpawnerBase currentSpawner)
        {
            if (currentSpawner is not TSpawner typedSpawner) return false;
            return CanSpawn(typedSpawner);
        }

        internal sealed override bool TryGenerateNextEntity(
            SpawnerBase currentSpawner, out ISpawnableEntity entity)
        {
            if (currentSpawner is not TSpawner typedSpawner)
            {
                entity = null;
                return false;
            }

            return TryGenerateNextEntity(typedSpawner, out entity);
        }
    }

    /// <summary>
    ///     Reusable random list implementation for component prefab references.
    /// </summary>
    /// <typeparam name="TSpawner">Concrete spawner type that owns the list.</typeparam>
    /// <typeparam name="TSpawnableEntity">Entity component type stored in the list.</typeparam>
    public abstract class SpawnListBase<TSpawner, TSpawnableEntity> : SpawnListBase<TSpawner>
        where TSpawner : SpawnerBase
        where TSpawnableEntity : Component, ISpawnableEntity
    {
        [SerializeField] private List<TSpawnableEntity> _entities = new();

        /// <summary>
        ///     Prefabs available to the default generator and custom list implementations.
        /// </summary>
        protected IReadOnlyList<TSpawnableEntity> Entities => _entities;

        protected override bool CanSpawn(TSpawner currentSpawner) => _entities.Count > 0;

        protected override bool TryGenerateNextEntity(
            TSpawner currentSpawner, out ISpawnableEntity entity)
        {
            if (_entities.Count == 0)
            {
                entity = null;
                return false;
            }

            entity = _entities[Random.Range(0, _entities.Count)];
            Component component = entity as Component;
            return !ReferenceEquals(component, null) && component;
        }
    }
}
