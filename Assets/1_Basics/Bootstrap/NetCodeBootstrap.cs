using Unity.NetCode;

namespace Samples.HelloNetcode {
    [UnityEngine.Scripting.Preserve]
    public class NetCodeBootstrap : ClientServerBootstrap {
        public override bool Initialize(string defaultWorldName) {
            AutoConnectPort = 7979;
            return base.Initialize(defaultWorldName);
        }
    }
}