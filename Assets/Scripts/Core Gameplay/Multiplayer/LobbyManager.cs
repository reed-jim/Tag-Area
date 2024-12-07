using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saferio.Util.SaferioTween;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using static GameEnum;

public class LobbyManager : MonoBehaviour
{
    #region PRIVATE FIELD
    private Lobby _currentLobby;
    private string _currentLobbyId;
    #endregion

    #region ACTION
    public static event Action<Lobby> updateLobbyRoomEvent;
    public static event Action<string> setLobbyId;
    public static event Action<string, string> setJoinCodeEvent;
    public static event Action<ScreenRoute> switchRouteEvent;
    public static event Action<string> startGameEvent;
    public static event Action<string, int> lobbyCreatedEvent;
    public static event Action<Lobby[]> fetchLobbiesDataEvent;

    public static event Action<string, int> updateNumberPlayerInLobbyEvent;

    public static event Action startClientEvent;
    #endregion

    private void Awake()
    {
        LobbyRoomScrollViewItem.joinLobbyEvent += JoinLobbyByIdAsync;
        LobbyScreen.refreshLobbyListEvent += QueryLobbyAsync;
        LobbyManagerUsingRelay.setJoinCodeEvent += SendJoinCodeAcrossLobby;
        LobbyManagerUsingRelay.toGameplayEvent += SetGameStartedForLobby;

        Init();

        SaferioTween.DelayAsync(1, onCompletedAction: () => QueryLobbyAsync());
    }

    private void OnDestroy()
    {
        LobbyRoomScrollViewItem.joinLobbyEvent -= JoinLobbyByIdAsync;
        LobbyScreen.refreshLobbyListEvent -= QueryLobbyAsync;
        LobbyManagerUsingRelay.setJoinCodeEvent -= SendJoinCodeAcrossLobby;
        LobbyManagerUsingRelay.toGameplayEvent -= SetGameStartedForLobby;
    }

    public string GenerateString(int length = 10, string chars = "abcdefghijklmnopqrstuvwxyz")
    {
        char[] stringChars = new char[length];
        System.Random random = new System.Random();

        for (int i = 0; i < length; i++)
        {
            int index = random.Next(chars.Length);
            stringChars[i] = chars[index];
        }

        return new string(stringChars);
    }

    private async void Init()
    {
        await InitializeLobbyAPI();

        Authenticate();
    }

    private async Task InitializeLobbyAPI()
    {
        await UnityServices.InitializeAsync();
    }

    private async void Authenticate()
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void SubscribeLobbyEvent()
    {
        var callbacks = new LobbyEventCallbacks();

        callbacks.PlayerJoined += OnPlayerJoinedLobby;
        callbacks.DataChanged += HandleOnLobbyDataChanged;

        Lobbies.Instance.SubscribeToLobbyEventsAsync(_currentLobbyId, callbacks);
    }

    public async void CreatePublicLobbyAsync()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized || !AuthenticationService.Instance.IsAuthorized)
        {
            return;
        }

        string lobbyName = GenerateString();
        int maxPlayers = 4;

        CreateLobbyOptions options = new CreateLobbyOptions();

        options.IsPrivate = false;
        options.Data = new Dictionary<string, DataObject>();

        _currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

        setLobbyId?.Invoke(_currentLobby.Id);

        _currentLobbyId = _currentLobby.Id;

        SubscribeLobbyEvent();

        OnLobbyCreated(_currentLobbyId, maxPlayers);
    }

    #region CALLBACK
    private void OnLobbyCreated(string lobbyId, int maxConnections)
    {
        switchRouteEvent?.Invoke(ScreenRoute.LobbyRoom);

        lobbyCreatedEvent?.Invoke(lobbyId, maxConnections);
    }

    private void OnLobbyJoined(string lobbyId, int maxConnections)
    {
        switchRouteEvent?.Invoke(ScreenRoute.LobbyRoom);
    }

    private void OnPlayerJoinedLobby(List<LobbyPlayerJoined> lobbyPlayerJoineds)
    {
        QueryLobbyAsync();

        // updateNumberPlayerInLobbyEvent?.Invoke(_currentLobbyId, lobbyPlayerJoineds.Count);
    }
    #endregion

    private async void QueryLobbyAsync()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            fetchLobbiesDataEvent?.Invoke(lobbies.Results.ToArray());

            for (int i = 0; i < lobbies.Results.Count; i++)
            {
                updateLobbyRoomEvent?.Invoke(lobbies.Results[i]);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void JoinLobbyByIdAsync(string lobbyId)
    {
        _currentLobbyId = lobbyId;

        try
        {
            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            SubscribeLobbyEvent();

            OnLobbyJoined(lobbyId, 4);

            QueryLobbyAsync();

            setLobbyId?.Invoke(lobbyId);

            if (joinedLobby.Data != null)
            {
                string joinCode = joinedLobby.Data["join_code"].Value;

                setJoinCodeEvent?.Invoke(lobbyId, joinCode);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void JoinLobbyByIdAsync(string lobbyId, string joinCode)
    {
        try
        {
            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            OnLobbyJoined(_currentLobbyId, 4);

            QueryLobbyAsync();

            // if (joinedLobby.Data != null)
            // {
            //     joinCode = joinedLobby.Data["join_code"].Value;

            //     startGameEvent?.Invoke(joinCode);
            // }

            var callbacks = new LobbyEventCallbacks();

            callbacks.DataChanged += HandleOnJoinCodeReceived;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async Task JoinLobbyByCodeAysnc(string lobbyCode)
    {
        try
        {
            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async Task UpdateLobbyAsync(Dictionary<string, DataObject> data)
    {
        var updateOptions = new UpdateLobbyOptions
        {
            Data = data
        };

        await LobbyService.Instance.UpdateLobbyAsync(_currentLobbyId, updateOptions);
    }

    private void HandleOnJoinCodeReceived(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> data)
    {
        if (data.ContainsKey("join_code"))
        {
            string joinCode = data["join_code"].Value.Value;

            setJoinCodeEvent?.Invoke(_currentLobbyId, joinCode);
        }
    }

    private void HandleOnLobbyDataChanged(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> data)
    {
        if (data.ContainsKey("is_game_started"))
        {
            if (bool.Parse(data["is_game_started"].Value.Value))
            {
                startClientEvent?.Invoke();
            }
        }
    }

    #region UPDATE
    private void SendJoinCodeAcrossLobby(string lobbyId, string joinCode)
    {
        Dictionary<string, DataObject> data = new Dictionary<string, DataObject>
        {
            {
                "join_code",
                new DataObject(
                visibility: DataObject.VisibilityOptions.Public,
                value: joinCode)
            }
        };

        UpdateLobbyAsync(data);
    }

    private void SetGameStartedForLobby()
    {
        Dictionary<string, DataObject> data = new Dictionary<string, DataObject>
        {
            {
                "is_game_started",
                new DataObject(
                visibility: DataObject.VisibilityOptions.Public,
                value: "true")
            }
        };

        UpdateLobbyAsync(data);
    }
    #endregion
}
