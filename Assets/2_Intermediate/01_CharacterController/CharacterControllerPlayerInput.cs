using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.NetCode.Samples.Common;
using UnityEngine;

namespace Samples.HelloNetcode
{
    [GhostComponent(PrefabType=GhostPrefabType.AllPredicted, OwnerSendType = SendToOwnerType.SendToNonOwner)]
    public struct CharacterControllerPlayerInput : IInputComponentData {
        [GhostField] public float2 Movement;
        [GhostField] public InputEvent Jump;
        [GhostField] public InputEvent PrimaryFire;
        [GhostField] public InputEvent SecondaryFire;
        [GhostField] public float Pitch;
        [GhostField] public float Yaw;
    }

    [UpdateInGroup(typeof(HelloNetcodeInputSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct SampleCharacterControllerPlayerInputSystem : ISystem {
        bool m_WasJumpTouch;
        bool m_WasFireTouch;
        bool m_WasSecondaryFireTouch;
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<CharacterControllerPlayerInput>();
            state.RequireForUpdate<NetworkStreamInGame>();
        }
        public void OnUpdate(ref SystemState state) {
            foreach (var input in SystemAPI.Query<RefRW<CharacterControllerPlayerInput>>().WithAll<GhostOwnerIsLocal>()) {
                input.ValueRW.Movement = default;
                input.ValueRW.Jump = default;
                input.ValueRW.PrimaryFire = default;
                input.ValueRW.SecondaryFire = default;
                if (Input.GetKey("left") || Input.GetKey("a")) {
                    input.ValueRW.Movement.x -= 1;
                }
                if (Input.GetKey("right") || Input.GetKey("d")) {
                    input.ValueRW.Movement.x += 1;
                }
                if (Input.GetKey("down") || Input.GetKey("s")) {
                    input.ValueRW.Movement.y -= 1;
                }
                if (Input.GetKey("up") || Input.GetKey("w")) {
                    input.ValueRW.Movement.y += 1;
                }
                if (Input.GetKeyDown("space")) {
                    input.ValueRW.Jump.Set();
                }

                float2 lookDelta = new float2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                input.ValueRW.Pitch = math.clamp(input.ValueRW.Pitch+lookDelta.y, -math.PI/2, math.PI/2);
                input.ValueRW.Yaw = math.fmod(input.ValueRW.Yaw + lookDelta.x, 2*math.PI);

                if(Input.GetKeyDown(KeyCode.Mouse0)) {
                    input.ValueRW.PrimaryFire.Set();
                }
                if(Input.GetKeyDown(KeyCode.Mouse1)) {
                    input.ValueRW.SecondaryFire.Set();
                }
            }
        }
    }
}