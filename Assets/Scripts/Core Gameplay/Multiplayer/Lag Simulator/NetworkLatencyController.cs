using Unity.Multiplayer.Tools.NetworkSimulator.Runtime;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkLatencyController : MonoBehaviour
{
    [SerializeField] private Toggle enableLagSimulatorToggle;
    [SerializeField] private NetworkSimulator networkSimulator;

    // #if DEVELOPMENT_BUILD && !UNITY_EDITOR
    //         NetworkManager.Singleton.GetComponent<UnityTransport>().SetDebugSimulatorParameters(
    //             packetDelay: 120,
    //             packetJitter: 5,
    //             dropRate: 3);
    // #endif

    private void Awake()
    {
        enableLagSimulatorToggle.onValueChanged.AddListener(EnableNetworkSimulator);

        // NetworkManager.Singleton.GetComponent<UnityTransport>().SetDebugSimulatorParameters(
        //     packetDelay: 120,
        //     packetJitter: 5,
        //     dropRate: 3);

        networkSimulator.Disconnect();
    }

    private void EnableNetworkSimulator(bool isEnable)
    {
        Debug.Log(isEnable);
        // networkSimulator.enabled = isEnable;
    }
}
