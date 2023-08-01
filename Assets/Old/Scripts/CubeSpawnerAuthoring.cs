using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct CubeSpawner : IComponentData {
    public Entity Cube;
}

[DisallowMultipleComponent]
public class CubeSpawnerAuthoring : MonoBehaviour {
    public GameObject Cube;

    class Baker : Baker<CubeSpawnerAuthoring> {
        public override void Bake(CubeSpawnerAuthoring authoring) {
            CubeSpawner component = default(CubeSpawner);
            component.Cube = GetEntity(authoring.Cube);
            AddComponent(component);
        }
    }
}