using Unity.Entities;
using UnityEngine;

namespace Samples.HelloNetcode {
    public struct Spawner : IComponentData {
        public Entity Player;
    }

    [DisallowMultipleComponent]
    public class SpawnerAuthoring : MonoBehaviour {
        public GameObject Player;

        class Baker : Baker<SpawnerAuthoring> {
            public override void Bake(SpawnerAuthoring authoring) {
                Spawner comp = default(Spawner);
                comp.Player = GetEntity(authoring.Player, TransformUsageFlags.Dynamic);
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, comp);
            }
        }
    }
}