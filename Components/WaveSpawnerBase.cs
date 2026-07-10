using Systems.SimpleCore.Operations;
using Systems.SimpleSpawn.Abstract;
using Systems.SimpleSpawn.Operations;

namespace Systems.SimpleSpawn.Components
{
    /// <summary>
    ///     Group spawner specialization for wave generation. Timing is owned by derived components.
    /// </summary>
    public abstract class WaveSpawnerBase : GroupSpawnerBase
    {
        /// <summary>
        ///     Checks whether a wave can be generated.
        /// </summary>
        protected virtual OperationResult CanSpawnWave(int waveSize)
        {
            if (waveSize <= 0) return SpawnOperations.InvalidSpawnCount();
            return CanSpawnEntity();
        }

        /// <summary>
        ///     Generates the next entity in a wave.
        /// </summary>
        protected virtual bool TryGenerateNextWaveEntity(out ISpawnableEntity entity)
            => TryGenerateNextEntity(out entity);

        protected sealed override OperationResult CanSpawnGroup(int groupSize)
            => CanSpawnWave(groupSize);

        protected sealed override bool TryGenerateNextGroupEntity(
            out ISpawnableEntity entity)
            => TryGenerateNextWaveEntity(out entity);

        protected sealed override OperationResult GetGroupSpawnedResult()
            => SpawnOperations.WaveSpawned();

        /// <summary>
        ///     Generates and spawns one wave immediately.
        /// </summary>
        public OperationResult TrySpawnWave(int waveSize)
            => TrySpawnGroupInternal(waveSize);

        protected sealed override void OnGroupSpawned(
            int groupSize, in OperationResult result)
            => OnWaveSpawned(groupSize, result);

        protected sealed override void OnGroupSpawnFailed(
            int groupSize, in OperationResult result)
            => OnWaveSpawnFailed(groupSize, result);

        protected virtual void OnWaveSpawned(int waveSize, in OperationResult result) { }

        protected virtual void OnWaveSpawnFailed(int waveSize, in OperationResult result) { }
    }
}
