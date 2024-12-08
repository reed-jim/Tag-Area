using System;
using Saferio.Util;
using Saferio.Util.SaferioTween;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScreen : UIScreen
{
    [SerializeField] private LobbyManager lobbyManager;

    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button refreshLobbyButton;
    [SerializeField] private ObjectPoolingScrollView lobbyRoomScrollView;

    #region ACTION
    public static event Action refreshLobbyListEvent;
    #endregion

    protected override void MoreActionInAwake()
    {
        base.MoreActionInAwake();
    }

    protected override void RegisterMoreEvent()
    {
        LobbyManager.fetchLobbiesDataEvent += OnLobbiesDataFetched;

        createRoomButton.onClick.AddListener(CreateRoom);
        refreshLobbyButton.onClick.AddListener(RefreshLobbyList);
    }

    protected override void UnregisterMoreEvent()
    {
        LobbyManager.fetchLobbiesDataEvent -= OnLobbiesDataFetched;
    }

    private void CreateRoom()
    {
        lobbyManager.CreatePublicLobbyAsync();
    }

    private void RefreshLobbyList()
    {
        refreshLobbyListEvent?.Invoke();
    }

    private void OnLobbiesDataFetched(Lobby[] lobbies)
    {
        SaferioTween.DelayAsync(0.2f, onCompletedAction: (() =>
        {
            lobbyRoomScrollView.Refresh();
        }));
    }
}
