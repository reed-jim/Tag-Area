using Unity.Multiplayer.Tools.NetworkSimulator.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class GameDebugManager : MonoBehaviour
{
    [SerializeField] private Toggle enableLagSimulatorToggle;
    [SerializeField] private Toggle setIsBotToggle;

    [SerializeField] private NetworkSimulator networkSimulator;


    [Header("SCRIPTABLE OBJECT")]
    [SerializeField] private BoolVariable isBotVariable;

    private void Awake()
    {
        enableLagSimulatorToggle.onValueChanged.AddListener(EnableNetworkSimulator);
        setIsBotToggle.onValueChanged.AddListener(SetIsBot);

        networkSimulator.gameObject.SetActive(enableLagSimulatorToggle.isOn);
    }

    private void EnableNetworkSimulator(bool isEnable)
    {
        networkSimulator.gameObject.SetActive(isEnable);
    }

    private void SetIsBot(bool isEnable)
    {
        isBotVariable.Value = isEnable;
    }
}
