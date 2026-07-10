using Systems.SimpleSpawn.Abstract;
using Systems.SimpleSpawn.Components;

namespace Systems.SimpleSpawn.Tests
{
    public sealed class TestSpawnableEntity : SpawnableEntityBase { }

    public sealed class TestSpawner : SpawnerBase { }

    public sealed class TestGroupSpawner : GroupSpawnerBase { }

    public sealed class TestWaveSpawner : WaveSpawnerBase { }

    public sealed class TestDespawnControl : DespawnControlBase
    {
        public bool WasDespawned;

        protected override void OnDespawn()
        {
            WasDespawned = true;
        }
    }
}
