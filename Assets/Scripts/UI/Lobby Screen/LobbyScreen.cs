using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScreen : UIScreen
{
    [SerializeField] private LobbyManager lobbyManager;

    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button refreshLobbyButton;

    #region PRIVATE FIELD

    #endregion

    #region ACTION
    public static event Action<int, string> updateLobbyRoomEvent;
    public static event Action refreshLobbyListEvent;
    #endregion

    protected override void RegisterMoreEvent()
    {
        createRoomButton.onClick.AddListener(CreateRoom);
        refreshLobbyButton.onClick.AddListener(RefreshLobbyList);
    }

    private void CreateRoom()
    {
        lobbyManager.CreatePublicLobbyAsync();
    }

    private void RefreshLobbyList()
    {
        refreshLobbyListEvent?.Invoke();
    }
}
