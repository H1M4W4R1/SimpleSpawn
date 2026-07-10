using NUnit.Framework;
using Systems.SimpleCore.Operations;
using Systems.SimpleSpawn.Abstract;
using Systems.SimpleSpawn.Operations;
using Systems.SimpleSpawn.Utility;
using UnityEngine;

namespace Systems.SimpleSpawn.Tests
{
    public sealed class SpawnAPITests
    {
        [Test]
        public void TrySpawn_WithDirectPrefabReference_CreatesSpawnedEntity()
        {
            GameObject prefabObject = new("SpawnPrefab");
            TestSpawnableEntity prefab = prefabObject.AddComponent<TestSpawnableEntity>();
            GameObject parentObject = new("SpawnParent");

            OperationResult result = SpawnAPI.TrySpawn(
                prefab, Vector3.one, Quaternion.identity, parentObject.transform,
                out ISpawnableEntity spawnedEntity);

            Assert.IsTrue(result);
            Assert.AreEqual(SpawnOperations.SUCCESS_SPAWNED, result.resultCode);
            Assert.IsNotNull(spawnedEntity);
            Component spawnedComponent = spawnedEntity as Component;
            Assert.IsNotNull(spawnedComponent);
            Assert.AreEqual(parentObject.transform, spawnedComponent.transform.parent);

            Object.DestroyImmediate(parentObject);
            Object.DestroyImmediate(prefabObject);
        }

        [Test]
        public void TrySpawn_WithNullPrefab_ReturnsInvalidPrefab()
        {
            OperationResult result = SpawnAPI.TrySpawn(
                null, Vector3.zero, Quaternion.identity, null,
                out ISpawnableEntity spawnedEntity);

            Assert.IsFalse(result);
            Assert.AreEqual(SpawnOperations.ERROR_INVALID_PREFAB, result.resultCode);
            Assert.IsNull(spawnedEntity);
        }

        [Test]
        public void TryDespawn_WithControl_InvokesControl()
        {
            GameObject entityObject = new("SpawnedEntity");
            TestSpawnableEntity entity = entityObject.AddComponent<TestSpawnableEntity>();
            TestDespawnControl control = entityObject.AddComponent<TestDespawnControl>();

            OperationResult result = SpawnAPI.TryDespawn(entity);

            Assert.IsTrue(result);
            Assert.IsTrue(control.WasDespawned);
            Assert.AreEqual(SpawnOperations.SUCCESS_DESPAWNED, result.resultCode);

            Object.DestroyImmediate(entityObject);
        }
    }
}
