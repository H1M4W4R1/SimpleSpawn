using Systems.SimpleSpawn.Abstract;
using Systems.SimpleSpawn.Components;
using UnityEngine;

namespace Systems.SimpleSpawn.Data
{
    /// <summary>
    ///     Default random spawn list usable by every spawner type.
    /// </summary>
    [CreateAssetMenu(
        fileName = "SpawnList",
        menuName = "Simple Spawn/Spawn List")]
    public sealed class SpawnList : SpawnListBase<SpawnerBase, SpawnableEntityBase> { }
}
