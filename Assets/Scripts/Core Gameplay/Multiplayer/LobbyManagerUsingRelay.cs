using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class LobbyManagerUsingRelay : NetworkBehaviour
{
    [SerializeField] private NetworkManager networkManager;

    #region PRIVATE FIELD
    private Dictionary<string, string> lobbyIdWithJoinCodeDictionary;
    private string _currentLobbyId;
    private string _currentJoinCode;
    #endregion

    #region ACTION
    public static event Action<string, string> setJoinCodeEvent;
    public static event Action toGameplayEvent;
    public static event Action<string> toSceneEvent;
    #endregion

    private void Awake()
    {
        LobbyDetailScreen.startGameEvent += StartGame;
        LobbyManager.lobbyCreatedEvent += StartHostWithRelay;
        LobbyManager.setJoinCodeEvent += SetJoinCode;
        LobbyManager.startClientEvent += StartClientWithRelay;

        // networkManager.OnServerStarted += OnHostStarted;
        // networkManager.OnClientStarted += OnClientStarted;

        lobbyIdWithJoinCodeDictionary = new Dictionary<string, string>();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        LobbyDetailScreen.startGameEvent -= StartGame;
        LobbyManager.lobbyCreatedEvent -= StartHostWithRelay;
        LobbyManager.setJoinCodeEvent -= SetJoinCode;
        LobbyManager.startClientEvent -= StartClientWithRelay;

        // networkManager.OnServerStarted -= OnHostStarted;
        // networkManager.OnClientStarted -= OnClientStarted;
    }

    private void SetJoinCode(string lobbyId, string joinCode)
    {
        _currentJoinCode = joinCode;
    }

    public async void GenerateJoinCode(string lobbyId, int maxConnections = 5)
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

        networkManager.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        lobbyIdWithJoinCodeDictionary.Add(lobbyId, joinCode);

        setJoinCodeEvent?.Invoke(lobbyId, joinCode);
    }

    public async void StartHostWithRelay(string lobbyId, int maxConnections = 5)
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

        networkManager.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        lobbyIdWithJoinCodeDictionary.Add(lobbyId, joinCode);

        setJoinCodeEvent?.Invoke(lobbyId, joinCode);

        networkManager.StartHost();
    }

    public async void StartClientWithRelay()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: _currentJoinCode);

        networkManager.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

        networkManager.StartClient();
    }

    private void StartGame()
    {
        toSceneEvent?.Invoke(GameConstants.GAMEPLAY_SCENE);

        toGameplayEvent?.Invoke();
    }

    // private void OnHostStarted()
    // {
    //     toSceneEvent?.Invoke(GameConstants.GAMEPLAY_SCENE);

    //     toGameplayEvent?.Invoke();
    // }

    // private void OnClientStarted()
    // {
    //     toSceneEvent?.Invoke(GameConstants.GAMEPLAY_SCENE);

    //     // toGameplayEvent?.Invoke();
    // }
}
