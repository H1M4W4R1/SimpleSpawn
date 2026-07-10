using Systems.SimpleCore.Operations;
using Systems.SimpleSpawn.Abstract;
using Systems.SimpleSpawn.Components;
using Systems.SimpleSpawn.Operations;
using UnityEngine;

namespace Systems.SimpleSpawn.Tests
{
    public sealed class TestSpawnList : SpawnListBase<TestSpawner>
    {
        public TestSpawnableEntity Prefab;
        public int CanSpawnCallCount;
        public int GenerateCallCount;
        public TestSpawner CurrentSpawner;

        protected override bool CanSpawn(TestSpawner currentSpawner)
        {
            CanSpawnCallCount++;
            CurrentSpawner = currentSpawner;
            return Prefab;
        }

        protected override bool TryGenerateNextEntity(
            TestSpawner currentSpawner, out ISpawnableEntity entity)
        {
            GenerateCallCount++;
            CurrentSpawner = currentSpawner;
            entity = Prefab;
            return Prefab;
        }
    }

    public sealed class TestGroupSpawnList : SpawnListBase<TestGroupSpawner>
    {
        public TestSpawnableEntity Prefab;

        protected override bool CanSpawn(TestGroupSpawner currentSpawner)
            => Prefab;

        protected override bool TryGenerateNextEntity(
            TestGroupSpawner currentSpawner, out ISpawnableEntity entity)
        {
            entity = Prefab;
            return Prefab;
        }
    }

    public sealed class TestWaveSpawnList : SpawnListBase<TestWaveSpawner>
    {
        public TestSpawnableEntity Prefab;

        protected override bool CanSpawn(TestWaveSpawner currentSpawner)
            => Prefab;

        protected override bool TryGenerateNextEntity(
            TestWaveSpawner currentSpawner, out ISpawnableEntity entity)
        {
            entity = Prefab;
            return Prefab;
        }
    }

    public sealed class FailingGroupSpawnList : SpawnListBase<TestGroupSpawner>
    {
        public TestSpawnableEntity Prefab;
        public int FailAfterCount;
        private int _generateCallCount;

        protected override bool CanSpawn(TestGroupSpawner currentSpawner)
            => Prefab;

        protected override bool TryGenerateNextEntity(
            TestGroupSpawner currentSpawner, out ISpawnableEntity entity)
        {
            _generateCallCount++;
            if (_generateCallCount > FailAfterCount)
            {
                entity = null;
                return false;
            }

            entity = Prefab;
            return Prefab;
        }
    }

    public sealed class TestDespawnControl : DespawnControlBase
    {
        public bool WasDespawned;
        public bool AllowDespawn = true;

        protected override OperationResult CanDespawn()
            => AllowDespawn
                ? SpawnOperations.Permitted()
                : SpawnOperations.SpawnNotPermitted();

        protected override void OnDespawn()
        {
            WasDespawned = true;
        }
    }

    public sealed class CallbackTestSpawner : SpawnerBase
    {
        public bool SpawnFailed;

        protected override void OnSpawnFailed(in OperationResult result)
        {
            SpawnFailed = true;
        }

        public OperationResult TrySpawnInvalidEntity()
            => TrySpawnGeneratedEntity(
                new TestNonComponentEntity(), Vector3.zero, Quaternion.identity);
    }

    public sealed class TestNonComponentEntity : ISpawnableEntity { }

}
