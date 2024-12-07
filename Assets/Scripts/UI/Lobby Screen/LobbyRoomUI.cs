using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRoomUI : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyIdText;
    [SerializeField] private TMP_Text joinCodeText;

    [SerializeField] private Button joinButton;

    private int _lobbyIndex;
    private string _lobbyId;
    private string _joinCode;

    #region ACTION
    public static event Action<string, string> joinLobbyEvent;
    #endregion

    private void Awake()
    {
        // LobbyScreen.updateLobbyRoomEvent += OnRoomCreated;
        // LobbyNetworkManager.updateLobbyRoomItemEvent += OnLobbyCreated;

        joinButton.onClick.AddListener(JoinLobby);
    }

    private void OnDestroy()
    {
        // LobbyScreen.updateLobbyRoomEvent -= OnRoomCreated;
        // LobbyNetworkManager.updateLobbyRoomItemEvent -= OnLobbyCreated;
    }

    public void Setup(int slotIndex)
    {
        _lobbyIndex = slotIndex;

        gameObject.SetActive(false);
    }

    private void OnLobbyCreated(int roomIndex, string lobbyId)
    {
        if (roomIndex == _lobbyIndex)
        {
            lobbyIdText.text = lobbyId;

            _lobbyId = lobbyId;

            gameObject.SetActive(true);
        }
    }

    private void JoinLobby()
    {
        joinLobbyEvent?.Invoke(_lobbyId, _joinCode);
    }
}
