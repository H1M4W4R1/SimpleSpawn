using Systems.SimpleCore.Operations;
using Systems.SimpleSpawn.Abstract;
using Systems.SimpleSpawn.Operations;

namespace Systems.SimpleSpawn.Components
{
    /// <summary>
    ///     Base for spawners that generate and spawn multiple entities as one group.
    /// </summary>
    public abstract class GroupSpawnerBase : SpawnerBase
    {
        /// <summary>
        ///     Checks whether a group can be generated.
        /// </summary>
        protected virtual OperationResult CanSpawnGroup(int groupSize)
        {
            if (groupSize <= 0) return SpawnOperations.InvalidSpawnCount();
            return CanSpawnEntity();
        }

        /// <summary>
        ///     Generates the next entity in a group.
        /// </summary>
        protected virtual bool TryGenerateNextGroupEntity(out ISpawnableEntity entity)
            => TryGenerateNextEntity(out entity);

        /// <summary>
        ///     Generates and spawns a group immediately.
        /// </summary>
        public virtual OperationResult TrySpawnGroup(int groupSize)
        {
            return TrySpawnGroupInternal(groupSize);
        }

        /// <summary>
        ///     Executes group generation so wave spawners can reuse the same operation.
        /// </summary>
        protected OperationResult TrySpawnGroupInternal(int groupSize)
        {
            OperationResult canSpawnResult = CanSpawnGroup(groupSize);
            if (!canSpawnResult)
            {
                OnGroupSpawnFailed(groupSize, canSpawnResult);
                return canSpawnResult;
            }

            for (int i = 0; i < groupSize; i++)
            {
                if (!TryGenerateNextGroupEntity(out ISpawnableEntity prefab))
                {
                    OperationResult result = SpawnOperations.EntityNotGenerated();
                    OnGroupSpawnFailed(groupSize, result);
                    return result;
                }

                OperationResult spawnResult = TrySpawnGeneratedEntity(
                    prefab, transform.position, transform.rotation);
                if (!spawnResult)
                {
                    OnGroupSpawnFailed(groupSize, spawnResult);
                    return spawnResult;
                }
            }

            OperationResult groupResult = GetGroupSpawnedResult();
            OnGroupSpawned(groupSize, groupResult);
            return groupResult;
        }

        protected virtual OperationResult GetGroupSpawnedResult()
            => SpawnOperations.GroupSpawned();

        protected virtual void OnGroupSpawned(int groupSize, in OperationResult result) { }

        protected virtual void OnGroupSpawnFailed(int groupSize, in OperationResult result) { }
    }
}
