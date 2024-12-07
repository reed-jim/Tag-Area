using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private Button joinGameButton;

    #region ACTION
    public static event Action<string> toSceneEvent;
    #endregion

    private void Awake()
    {
        NetworkManager.Singleton.OnClientStarted += ToGameplayScene;

        joinGameButton.onClick.AddListener(JoinGame);
    }

    private void JoinGame()
    {
        Debug.Log(NetworkManager.Singleton.ConnectedHostname);
        Debug.Log(NetworkManager.Singleton.ConnectedClientsList.Count);
        Debug.Log(NetworkManager.Singleton.IsClient);

        if (NetworkManager.Singleton.ConnectedClients.Count == 0)
        {
            JoinHost();
        }
        else
        {
            JoinClient();
        }
    }

    private void JoinHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void JoinClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void ToGameplayScene()
    {
        toSceneEvent?.Invoke(GameConstants.GAMEPLAY_SCENE);
    }
}
