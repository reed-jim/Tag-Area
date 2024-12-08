using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyDetailScreen : UIScreen
{
    [SerializeField] private TMP_Text lobbyIdText;
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private TMP_Text numberPlayerText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMP_Text startGameButtonText;

    private string _lobbyId;
    private string _joinCode;
    private string _numberPlayerInLobby;

    #region ACTION
    public static event Action startGameEvent;
    #endregion

    protected override void MoreActionInAwake()
    {
        base.MoreActionInAwake();

        startGameButton.image.color = GameConstants.ERROR_BACKGROUND;
        startGameButtonText.color = GameConstants.ERROR_TEXT;

        startGameButtonText.text = GameConstants.DISCONNECTED;
    }

    protected override void RegisterMoreEvent()
    {
        base.RegisterMoreEvent();

        LobbyManager.setLobbyId += SetLobbyId;
        LobbyManager.setJoinCodeEvent += SetJoinCode;
        LobbyManager.updateNumberPlayerInLobbyEvent += UpdateNumberPlayerLobby;
        LobbyManager.updateLobbyRoomEvent += UpdateLobby;
        LobbyManagerUsingRelay.setJoinCodeEvent += SetJoinCode;
        LobbyManagerUsingRelay.hostStartedEvent += OnHostStarted;
        LobbyManagerUsingRelay.clientStartedEvent += OnClientStarted;

        startGameButton.onClick.AddListener(StartGame);
    }

    protected override void UnregisterMoreEvent()
    {
        base.UnregisterMoreEvent();

        LobbyManager.setLobbyId -= SetLobbyId;
        LobbyManager.setJoinCodeEvent -= SetJoinCode;
        LobbyManager.updateNumberPlayerInLobbyEvent -= UpdateNumberPlayerLobby;
        LobbyManager.updateLobbyRoomEvent -= UpdateLobby;
        LobbyManagerUsingRelay.setJoinCodeEvent -= SetJoinCode;
        LobbyManagerUsingRelay.hostStartedEvent -= OnHostStarted;
        LobbyManagerUsingRelay.clientStartedEvent -= OnClientStarted;
    }

    private void UpdateLobby(Lobby lobby)
    {
        UpdateNumberPlayerLobby(lobby.Id, lobby.Players.Count);
    }

    private void SetLobbyId(string lobbyId)
    {
        _lobbyId = lobbyId;

        lobbyIdText.text = $"Lobby Id: {_lobbyId}";
    }

    private void SetJoinCode(string lobbyId, string joinCode)
    {
        if (lobbyId == _lobbyId)
        {
            _joinCode = joinCode;

            joinCodeText.text = $"Join Code: {_joinCode}";
        }
    }

    private void UpdateNumberPlayerLobby(string lobbyId, int numPlayer)
    {
        if (lobbyId == _lobbyId)
        {
            numberPlayerText.text = $"<color=#78FF78>{numPlayer}/4 <color=#fff>Players Joined";
        }
    }

    private void StartGame()
    {
        startGameEvent?.Invoke();
    }

    #region CALLBACK
    private void OnHostStarted()
    {
        startGameButton.gameObject.SetActive(true);

        startGameButton.image.color = GameConstants.PRIMARY_BACKGROUND;
        startGameButtonText.color = GameConstants.PRIMARY_TEXT;

        startGameButtonText.text = GameConstants.START_GAME;
    }

    private void OnClientStarted()
    {
        startGameButton.gameObject.SetActive(true);

        startGameButton.interactable = false;

        startGameButton.image.color = GameConstants.PRIMARY_BACKGROUND;
        startGameButtonText.color = GameConstants.PRIMARY_TEXT;

        startGameButtonText.text = GameConstants.CONNECTED;
    }
    #endregion
}
