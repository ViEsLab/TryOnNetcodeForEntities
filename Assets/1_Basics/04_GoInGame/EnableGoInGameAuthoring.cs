using Unity.Entities;
using UnityEngine;

namespace Samples.HelloNetcode {
    public struct EnableGoInGame : IComponentData { }

    [DisallowMultipleComponent]
    public class EnableGoInGameAuthoring : MonoBehaviour {
        class Baker : Baker<EnableGoInGameAuthoring> {
            public override void Bake(EnableGoInGameAuthoring authoring) {
                EnableGoInGame comp = default(EnableGoInGame);
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, comp);
            }
        }
    }
}