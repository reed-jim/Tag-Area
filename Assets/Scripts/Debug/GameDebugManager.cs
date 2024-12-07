using UnityEngine;
using UnityEngine.UI;

public class GameDebugManager : MonoBehaviour
{
    [SerializeField] private Toggle setIsBotToggle;

    [Header("SCRIPTABLE OBJECT")]
    [SerializeField] private BoolVariable isBotVariable;

    private void Awake()
    {
        setIsBotToggle.onValueChanged.AddListener(EnableNetworkSimulator);
    }

    private void EnableNetworkSimulator(bool isEnable)
    {
        isBotVariable.Value = isEnable;
    }
}
