using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Samples.HelloNetcode {
    public struct PlayerSpawned : IComponentData {}

    public struct ConnectionOwner : IComponentData {
        public Entity Entity;
    }

    public partial class SpawnPlayerSystem : SystemBase {
        private EntityQuery m_NewPlayers;

        protected override void OnCreate() {
            RequireForUpdate(m_NewPlayers);
            RequireForUpdate<Spawner>();
            EntityQuery sceneQuery = GetEntityQuery(new EntityQueryDesc() {
                Any = new[] {
                    ComponentType.ReadOnly<EnableSpawnPlayer>(),
                }
            });
            RequireForUpdate(sceneQuery);
        }

        protected override void OnUpdate() {
            Entity prefab = SystemAPI.GetSingleton<Spawner>().Player;
            EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            Entities.
                WithName("SpawnPlayer").
                WithStoreEntityQueryInField(ref m_NewPlayers).
                WithNone<PlayerSpawned>().
                ForEach((Entity connectionEntity, in NetworkStreamInGame req, in NetworkId networkId) => {
                    Debug.Log($"为连接 {networkId} 创建玩家");
                    Entity player = commandBuffer.Instantiate(prefab);

                    // 为新创建的玩家设置对应连接 id 的 GhostOwner，为建立 CommandTarget 做准备
                    commandBuffer.SetComponent(player, new GhostOwner() { NetworkId = networkId.Value });

                    // 设置 CommandTarget，根据官方介绍，在 AutoCommandTarget 特性开启时不需要手动设置
                    // [ViE]TODO: 在做 ThinClients 示例时回来看一下
                    commandBuffer.SetComponent(connectionEntity, new CommandTarget() { targetEntity = player });

                    // 为当前连接添加完成标志
                    commandBuffer.AddComponent<PlayerSpawned>(connectionEntity);

                    // 把 Player 添加到 LinkedEntityGroup 中使其在断开连接时自动销毁
                    commandBuffer.AppendToBuffer(connectionEntity, new LinkedEntityGroup() { Value = player });

                    commandBuffer.AddComponent(player, new ConnectionOwner() { Entity = connectionEntity});
                }).Run();
            commandBuffer.Playback(EntityManager);
        }
    }
}