using System.Collections;
using System.Collections.Generic;
using Unity.NetCode;
using UnityEngine;

[UnityEngine.Scripting.Preserve]
public class Game : ClientServerBootstrap {
    public override bool Initialize(string defaultWorldName) {
        // 设置自动连接接口
        AutoConnectPort = 7979;
        return base.Initialize(defaultWorldName);
    }
}