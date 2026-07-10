using NUnit.Framework;
using Systems.SimpleCore.Operations;
using Systems.SimpleSpawn.Operations;
using UnityEditor;
using UnityEngine;

namespace Systems.SimpleSpawn.Tests
{
    public sealed class SpawnerTests
    {
        [Test]
        public void TrySpawn_UsesTypedListAndTracksEntity()
        {
            GameObject prefabObject = new("SpawnPrefab");
            TestSpawnableEntity prefab = prefabObject.AddComponent<TestSpawnableEntity>();
            TestSpawnList list = ScriptableObject.CreateInstance<TestSpawnList>();
            list.Prefab = prefab;

            GameObject spawnerObject = new("Spawner");
            TestSpawner spawner = spawnerObject.AddComponent<TestSpawner>();
            SerializedObject serializedSpawner = new(spawner);
            serializedSpawner.FindProperty("_spawnList").objectReferenceValue = list;
            serializedSpawner.ApplyModifiedPropertiesWithoutUndo();

            OperationResult result = spawner.TrySpawn();

            Assert.IsTrue(result);
            Assert.AreEqual(1, spawner.SpawnedEntities.Count);
            Assert.AreEqual(1, list.CanSpawnCallCount);
            Assert.AreEqual(1, list.GenerateCallCount);
            Assert.AreSame(spawner, list.CurrentSpawner);

            OperationResult despawnResult = spawner.DespawnAll();
            Assert.IsTrue(despawnResult);
            Assert.AreEqual(SpawnOperations.SUCCESS_ALL_DESPAWNED, despawnResult.resultCode);
            Assert.AreEqual(0, spawner.SpawnedEntities.Count);

            Object.DestroyImmediate(spawnerObject);
            Object.DestroyImmediate(prefabObject);
            Object.DestroyImmediate(list);
        }

        [Test]
        public void TrySpawn_WithoutList_ReturnsConfigurationError()
        {
            GameObject spawnerObject = new("Spawner");
            TestSpawner spawner = spawnerObject.AddComponent<TestSpawner>();

            OperationResult result = spawner.TrySpawn();

            Assert.IsFalse(result);
            Assert.AreEqual(SpawnOperations.ERROR_SPAWN_LIST_NOT_ASSIGNED, result.resultCode);

            Object.DestroyImmediate(spawnerObject);
        }

        [Test]
        public void TrySpawnGroup_GeneratesAndTracksWholeGroup()
        {
            GameObject prefabObject = new("SpawnPrefab");
            TestSpawnableEntity prefab = prefabObject.AddComponent<TestSpawnableEntity>();
            TestGroupSpawnList list = ScriptableObject.CreateInstance<TestGroupSpawnList>();
            list.Prefab = prefab;

            GameObject spawnerObject = new("GroupSpawner");
            TestGroupSpawner spawner = spawnerObject.AddComponent<TestGroupSpawner>();
            SerializedObject serializedSpawner = new(spawner);
            serializedSpawner.FindProperty("_spawnList").objectReferenceValue = list;
            serializedSpawner.ApplyModifiedPropertiesWithoutUndo();

            OperationResult result = spawner.TrySpawnGroup(2);

            Assert.IsTrue(result);
            Assert.AreEqual(SpawnOperations.SUCCESS_GROUP_SPAWNED, result.resultCode);
            Assert.AreEqual(2, spawner.SpawnedEntities.Count);

            spawner.DespawnAll();
            Object.DestroyImmediate(spawnerObject);
            Object.DestroyImmediate(prefabObject);
            Object.DestroyImmediate(list);
        }

        [Test]
        public void TrySpawnWave_UsesGroupGenerationWithoutScheduling()
        {
            GameObject prefabObject = new("SpawnPrefab");
            TestSpawnableEntity prefab = prefabObject.AddComponent<TestSpawnableEntity>();
            TestWaveSpawnList list = ScriptableObject.CreateInstance<TestWaveSpawnList>();
            list.Prefab = prefab;

            GameObject spawnerObject = new("WaveSpawner");
            TestWaveSpawner spawner = spawnerObject.AddComponent<TestWaveSpawner>();
            SerializedObject serializedSpawner = new(spawner);
            serializedSpawner.FindProperty("_spawnList").objectReferenceValue = list;
            serializedSpawner.ApplyModifiedPropertiesWithoutUndo();

            OperationResult result = spawner.TrySpawnWave(3);

            Assert.IsTrue(result);
            Assert.AreEqual(SpawnOperations.SUCCESS_WAVE_SPAWNED, result.resultCode);
            Assert.AreEqual(3, spawner.SpawnedEntities.Count);

            spawner.DespawnAll();
            Object.DestroyImmediate(spawnerObject);
            Object.DestroyImmediate(prefabObject);
            Object.DestroyImmediate(list);
        }
    }
}
