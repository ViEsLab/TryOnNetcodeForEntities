using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct TestCube : IComponentData {
}

[DisallowMultipleComponent]
public class CubeAuthoring : MonoBehaviour {
    class Baker : Baker<CubeAuthoring> {
        public override void Bake(CubeAuthoring authoring) {
            TestCube component = default(TestCube);
            AddComponent(component);
        }
    }
}