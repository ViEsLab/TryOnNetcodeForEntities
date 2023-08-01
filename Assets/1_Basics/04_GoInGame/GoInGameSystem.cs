using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Samples.HelloNetcode {
    [UpdateInGroup(typeof(HelloNetcodeSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ServerSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial class GoInGameSystem : SystemBase {
        private EntityQuery m_NewConnections;

        protected override void OnCreate() {
            RequireForUpdate<EnableGoInGame>();
            RequireForUpdate(m_NewConnections);
        }

        protected override void OnUpdate() {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            FixedString32Bytes worldName = World.Name;

            // 连接建立后立刻标记 in-game
            // 为什么要缓存这个EntityQuery？
            Entities.
                WithName("NewConnectionGoInGame").
                WithStoreEntityQueryInField(ref m_NewConnections).
                WithNone<NetworkStreamInGame>().
                ForEach((Entity ent, in NetworkId id) => {
                    Debug.Log($"[{worldName}] Go in game connection {id.Value}");
                    commandBuffer.AddComponent<NetworkStreamInGame>(ent);
                }).Run();
            commandBuffer.Playback(EntityManager);
        }
    }
}