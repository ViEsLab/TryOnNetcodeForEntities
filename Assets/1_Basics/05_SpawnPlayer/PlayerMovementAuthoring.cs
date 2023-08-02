using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Samples.HelloNetcode {
    public struct PlayerMovement : IComponentData {
        // 跳跃逻辑将在 Prediction Loop 中计算，因此需要将跳跃速度标记为 GhostField 来保存到 Ghost 历史中
        [GhostField]
        public int JumpVelocity;
    }

    [DisallowMultipleComponent]
    public class PlayerMovementAuthoring : MonoBehaviour {
        // 为了能实时响应修改，将两者绑定
        [RegisterBinding(typeof(PlayerMovement), "JumpVelocity")]
        public int JumpVelocity;

        class Baker : Baker<PlayerMovementAuthoring> {
            public override void Bake(PlayerMovementAuthoring authoring) {
                PlayerMovement comp = default(PlayerMovement);
                comp.JumpVelocity = authoring.JumpVelocity;
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, comp);
            }
        }
    }
}