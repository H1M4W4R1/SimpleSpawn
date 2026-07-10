using Systems.SimpleSpawn.Abstract;
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

}
