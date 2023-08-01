using Unity.Entities;
using UnityEngine;

namespace Samples.HelloNetcode {
    public struct EnableSpawnPlayer : IComponentData {}

    [DisallowMultipleComponent]
    public class EnableSpawnPlayerAuthoring : MonoBehaviour {
        class EnableSpawnPlayerBaker : Baker<EnableSpawnPlayerAuthoring> {
            public override void Bake(EnableSpawnPlayerAuthoring authoring) {
                EnableSpawnPlayer comp = default(EnableSpawnPlayer);
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, comp);
            }
        }
    }
}