using Systems.SimpleCore.Operations;
using UnityEngine;

namespace Systems.SimpleSpawn.Examples.Scripts
{
    [DisallowMultipleComponent]
    public sealed class ExampleSpawnScene : MonoBehaviour
    {
        [SerializeField] private ExampleSingleSpawner _spawner;
        [SerializeField] private int _spawnCount = 3;

        private void Start()
        {
            RunExample();
        }

        [ContextMenu("Run Spawn Example")]
        public void RunExample()
        {
            if (!_spawner)
            {
                Debug.LogWarning("[SimpleSpawn] Example spawner is not assigned.");
                return;
            }

            for (int spawnIndex = 0; spawnIndex < _spawnCount; spawnIndex++)
            {
                OperationResult result = _spawner.TrySpawnSingle();
                Debug.Log("[SimpleSpawn] Spawn " + spawnIndex + " result: " + result);
            }
        }
    }
}
