using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRoomScrollViewItem : MonoBehaviour, ISaferioScrollViewItem
{
    [SerializeField] private TMP_Text lobbyIdText;
    [SerializeField] private Button joinButton;

    private Lobby[] _lobbiesData;

    #region ACTION
    public static event Action<string> joinLobbyEvent;
    #endregion

    private void Awake()
    {
        LobbyManager.fetchLobbiesDataEvent += SetLobbiesData;
    }

    private void OnDestroy()
    {
        LobbyManager.fetchLobbiesDataEvent -= SetLobbiesData;
    }

    public void SetLobbiesData(Lobby[] lobbiesData)
    {
        _lobbiesData = lobbiesData;
    }

    private void JoinLobby(string lobbyId)
    {
        joinLobbyEvent?.Invoke(lobbyId);
    }

    public bool IsValidAtIndex(int index)
    {
        if (_lobbiesData == null)
        {
            return false;
        }

        if (index < _lobbiesData.Length)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Refresh(int index)
    {
        if (!IsValidAtIndex(index))
        {
            gameObject.SetActive(false);

            return;
        }
        else
        {
            gameObject.SetActive(true);
        }

        string lobbyId = _lobbiesData[index].Id;

        lobbyIdText.text = lobbyId;

        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(() => JoinLobby(lobbyId));
    }

    public void Setup(int index, RectTransform parent)
    {
        if (!IsValidAtIndex(index))
        {
            gameObject.SetActive(false);
        }
    }
}
