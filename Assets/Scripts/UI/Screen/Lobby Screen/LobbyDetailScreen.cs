using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyDetailScreen : UIScreen
{
    [SerializeField] private TMP_Text clientIdText;
    [SerializeField] private TMP_Text lobbyIdText;
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private TMP_Text numberPlayerText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMP_Text startGameButtonText;

    #region PRIVATE FIELD
    private string _lobbyId;
    private string _joinCode;
    #endregion

    #region ACTION
    public static event Action<string> toSceneEvent;
    #endregion

    #region LIFE CYCLE
    protected override void MoreActionInAwake()
    {
        base.MoreActionInAwake();

        startGameButton.image.color = GameConstants.ERROR_BACKGROUND;
        startGameButtonText.color = GameConstants.ERROR_TEXT;

        startGameButtonText.text = GameConstants.DISCONNECTED;

        clientIdText.gameObject.SetActive(false);
        lobbyIdText.gameObject.SetActive(false);
    }

    protected override void RegisterMoreEvent()
    {
        base.RegisterMoreEvent();

        LobbyManager.setLobbyId += SetLobbyId;
        LobbyManager.setJoinCodeEvent += SetJoinCode;
        LobbyManager.updateLobbyRoomEvent += UpdateLobby;
        NetcodeManagerUsingRelay.setJoinCodeEvent += SetJoinCode;
        NetcodeManagerUsingRelay.hostStartedEvent += OnHostStarted;
        NetcodeManagerUsingRelay.clientStartedEvent += OnClientStarted;
        NetcodeManagerUsingRelay.setClientIdEvent += SetClientId;

        startGameButton.onClick.AddListener(StartGame);
    }

    protected override void UnregisterMoreEvent()
    {
        base.UnregisterMoreEvent();

        LobbyManager.setLobbyId -= SetLobbyId;
        LobbyManager.setJoinCodeEvent -= SetJoinCode;
        LobbyManager.updateLobbyRoomEvent -= UpdateLobby;
        NetcodeManagerUsingRelay.setJoinCodeEvent -= SetJoinCode;
        NetcodeManagerUsingRelay.hostStartedEvent -= OnHostStarted;
        NetcodeManagerUsingRelay.clientStartedEvent -= OnClientStarted;
        NetcodeManagerUsingRelay.setClientIdEvent -= SetClientId;
    }
    #endregion

    private void UpdateLobby(Lobby lobby)
    {
        UpdateNumberPlayerLobby(lobby.Id, lobby.Players.Count);
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
        toSceneEvent?.Invoke(GameConstants.GAMEPLAY_SCENE);
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

    private void SetClientId(ulong clientId)
    {
        clientIdText.gameObject.SetActive(true);

        clientIdText.text = $"Player {clientId}";
    }

    private void SetLobbyId(string lobbyId)
    {
        lobbyIdText.gameObject.SetActive(true);

        lobbyIdText.text = $"Lobby Id: {lobbyId}";

        _lobbyId = lobbyId;
    }

    private void SetJoinCode(string lobbyId, string joinCode)
    {
        if (lobbyId == _lobbyId)
        {
            _joinCode = joinCode;

            joinCodeText.text = $"Join Code: {_joinCode}";
        }
    }
    #endregion
}
