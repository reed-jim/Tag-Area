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

public class LobbyManagerUsingRelay : NetworkBehaviour
{
    [SerializeField] private NetworkManager networkManager;

    #region PRIVATE FIELD
    private Dictionary<string, string> lobbyIdWithJoinCodeDictionary;
    #endregion

    #region ACTION
    public static event Action<string, string> setJoinCodeEvent;
    public static event Action toGameplayEvent;
    #endregion

    private void Awake()
    {
        // LobbyDetailScreen.startGameForLobbyEvent += StartHostWithRelay;
        // LobbyNetworkManager.startGameEvent += StartClientWithRelay;

        LobbyManager.lobbyCreatedEvent += GenerateJoinCode;

        networkManager.OnServerStarted += OnHostStarted;
        networkManager.OnClientStarted += OnClientStarted;

        lobbyIdWithJoinCodeDictionary = new Dictionary<string, string>();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        LobbyManager.lobbyCreatedEvent -= GenerateJoinCode;

        // LobbyDetailScreen.startGameForLobbyEvent -= StartHostWithRelay;
        // LobbyNetworkManager.startGameEvent -= StartClientWithRelay;

        networkManager.OnServerStarted -= OnHostStarted;
        networkManager.OnClientStarted -= OnClientStarted;
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
        // await UnityServices.InitializeAsync();

        // if (!AuthenticationService.Instance.IsSignedIn)
        // {
        //     await AuthenticationService.Instance.SignInAnonymouslyAsync();
        // }

        // Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

        // networkManager.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

        // string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        // lobbyIdWithJoinCodeDictionary.Add(lobbyId, joinCode);

        // setJoinCodeEvent?.Invoke(lobbyId, joinCode);

        networkManager.StartHost();
    }

    public async void StartClientWithRelay(string joinCode)
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);

        networkManager.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

        networkManager.StartClient();
    }

    private void OnHostStarted()
    {
        toGameplayEvent?.Invoke();
    }

    private void OnClientStarted()
    {
        toGameplayEvent?.Invoke();
    }
}
