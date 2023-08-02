using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Samples.HelloNetcode {
    // 收集指令
    [UpdateInGroup(typeof(HelloNetcodeInputSystemGroup))]
    [AlwaysSynchronizeSystem]
    public partial class GatherAutoCommandsSystem : SystemBase {
        protected override void OnCreate() {
            RequireForUpdate<NetworkStreamInGame>();
            RequireForUpdate<EnableSpawnPlayer>();
            RequireForUpdate<PlayerInput>();
        }

        protected override void OnUpdate() {
            bool left = Input.GetKey(KeyCode.LeftArrow);
            bool right = Input.GetKey(KeyCode.RightArrow);
            bool down = Input.GetKey(KeyCode.DownArrow);
            bool up = Input.GetKey(KeyCode.UpArrow);
            bool jump = Input.GetKeyDown(KeyCode.Space);

            // 只有本地玩家需要收集指令，因此利用 GhostOwnerIsLocal 来排除，该组件会在所有包含 GhostOwner 的实体上自动添加
            Entities
                .WithName("GatherInput")
                .WithAll<GhostOwnerIsLocal>()
                .ForEach((ref PlayerInput inputData) => {
                    inputData = default;

                    if (jump) {
                        inputData.Jump.Set();
                    }
                    if (left) {
                        inputData.Horizontal -= 1;
                    }
                    if (right) {
                        inputData.Horizontal += 1;
                    }
                    if (down) {
                        inputData.Vertical -= 1;
                    }
                    if (up) {
                        inputData.Vertical += 1;
                    }
                }).ScheduleParallel();
        }
    }

    // [ViE]TODO: 在这里 Update 的用处还要看一下
    [UpdateInGroup(typeof(HelloNetcodePredictedSystemGroup))]
    public partial class ProcessAutoCommandsSystem : SystemBase {
        protected override void OnCreate() {
            RequireForUpdate<EnableSpawnPlayer>();
            RequireForUpdate<PlayerInput>();
        }

        protected override void OnUpdate() {
            float movementSpeed = SystemAPI.Time.DeltaTime * 3;
            SystemAPI.TryGetSingleton<ClientServerTickRate>(out var tickRate);
            // 保持跳跃表现在不同的模拟帧率下结果相同
            int velocityDecStep = 60 / tickRate.SimulationTickRate;
            // 排除不需要模拟的实体
            Entities.WithName("ProcessInputForTick").WithAll<Simulate>().ForEach(
                (ref PlayerInput input, ref LocalTransform trans, ref PlayerMovement movement) => {
                    if (input.Jump.IsSet) {
                        movement.JumpVelocity = 10;
                    }

                    float verticalMovement = 0;
                    if (movement.JumpVelocity > 0) {
                        movement.JumpVelocity -= velocityDecStep;
                        verticalMovement = 1;
                    } else {
                        if (trans.Position.y > 0) {
                            verticalMovement = -1;
                        }
                    }

                    float3 moveInput = new float3(input.Horizontal, verticalMovement, input.Vertical);
                    moveInput = math.normalizesafe(moveInput) * movementSpeed;

                    if (movement.JumpVelocity <= 0 &&
                        (trans.Position.y + moveInput.y < 0 || trans.Position.y + moveInput.y < 0.05f)) {
                        moveInput.y = trans.Position.y = 0;
                    }
                    trans.Position += new float3(moveInput.x, moveInput.y, moveInput.z);
                }).ScheduleParallel();
        }
    }
}