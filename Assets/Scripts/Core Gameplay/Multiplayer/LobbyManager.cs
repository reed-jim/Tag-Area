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
    public static event Action<int, string> updateLobbyRoomItemEvent;
    public static event Action<string> setLobbyId;
    public static event Action<ScreenRoute> switchRouteEvent;
    public static event Action<string> startGameEvent;
    public static event Action<string, int> lobbyCreatedEvent;
    public static event Action<Lobby[]> fetchLobbiesDataEvent;
    #endregion

    private void Awake()
    {
        LobbyRoomUI.joinLobbyEvent += JoinLobbyByIdAsync;
        LobbyScreen.refreshLobbyListEvent += QueryLobbyAsync;
        LobbyManagerUsingRelay.setJoinCodeEvent += SendJoinCodeAcrossLobby;

        Init();

        SaferioTween.DelayAsync(1, onCompletedAction: () => QueryLobbyAsync());
    }

    private void OnDestroy()
    {
        LobbyRoomUI.joinLobbyEvent -= JoinLobbyByIdAsync;
        LobbyScreen.refreshLobbyListEvent -= QueryLobbyAsync;
        LobbyManagerUsingRelay.setJoinCodeEvent -= SendJoinCodeAcrossLobby;
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

        OnLobbyCreated(_currentLobbyId, maxPlayers);
    }

    private void OnLobbyCreated(string lobbyId, int maxConnections)
    {
        switchRouteEvent?.Invoke(ScreenRoute.LobbyRoom);

        lobbyCreatedEvent?.Invoke(lobbyId, maxConnections);

        // // wait for Lobby Room Screen to be active
        // SaferioTween.Delay(0.2f, onCompletedAction: () =>
        // {
        //     lobbyCreatedEvent?.Invoke(lobbyId, maxConnections);
        // });
    }

    private async void QueryLobbyAsync()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);
            Debug.Log(lobbies.Results.Count);
            fetchLobbiesDataEvent?.Invoke(lobbies.Results.ToArray());

            for (int i = 0; i < lobbies.Results.Count; i++)
            {
                updateLobbyRoomItemEvent?.Invoke(i, lobbies.Results[i].Id);
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

            if (joinedLobby.Data != null)
            {
                joinCode = joinedLobby.Data["join_code"].Value;

                startGameEvent?.Invoke(joinCode);
            }

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

            Debug.Log(data["join_code"].Value);
            Debug.Log(joinCode);

            startGameEvent?.Invoke(joinCode);
        }
    }

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
}
