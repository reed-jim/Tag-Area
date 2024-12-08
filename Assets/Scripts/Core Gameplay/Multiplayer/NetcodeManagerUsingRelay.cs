using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class NetcodeManagerUsingRelay : NetworkBehaviour
{
    [SerializeField] private NetworkManager networkManager;

    #region PRIVATE FIELD
    private Dictionary<string, string> lobbyIdWithJoinCodeDictionary;
    private string _currentJoinCode;
    #endregion

    #region ACTION
    public static event Action hostStartedEvent;
    public static event Action clientStartedEvent;
    public static event Action<ulong> setClientIdEvent;
    public static event Action<string, string> setJoinCodeEvent;
    public static event Action toGameplayEvent;
    #endregion

    #region LIFE CYCLE
    private void Awake()
    {
        LobbyManager.lobbyCreatedEvent += StartHostWithRelay;
        LobbyManager.setJoinCodeEvent += SetJoinCode;
        LobbyManager.startClientEvent += StartClientWithRelay;

        networkManager.OnServerStarted += OnHostStarted;
        networkManager.OnClientStarted += OnClientStarted;
        networkManager.OnClientConnectedCallback += OnClientConnected;

        lobbyIdWithJoinCodeDictionary = new Dictionary<string, string>();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        LobbyManager.lobbyCreatedEvent -= StartHostWithRelay;
        LobbyManager.setJoinCodeEvent -= SetJoinCode;
        LobbyManager.startClientEvent -= StartClientWithRelay;

        networkManager.OnServerStarted -= OnHostStarted;
        networkManager.OnClientStarted -= OnClientStarted;
        networkManager.OnClientConnectedCallback -= OnClientConnected;
    }
    #endregion

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

    // SHOULD USE THIS, FINISH LATER
    public async void StartClientWithRelay(string lobbyId)
    {
        string joinCode = lobbyIdWithJoinCodeDictionary[lobbyId];
    }

    private void SetJoinCode(string lobbyId, string joinCode)
    {
        _currentJoinCode = joinCode;
    }

    #region CALLBACK
    private void OnHostStarted()
    {
        if (IsHost)
        {
            hostStartedEvent?.Invoke();
        }
    }

    private void OnClientStarted()
    {
        if (!IsHost)
        {
            clientStartedEvent?.Invoke();
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId == networkManager.LocalClientId)
        {
            setClientIdEvent?.Invoke(clientId);
        }
    }
    #endregion
}
