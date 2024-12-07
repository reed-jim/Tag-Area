using System;
using Unity.Netcode;
using UnityEngine;

public class LevelSpawner : NetworkBehaviour
{
    public GameObject playerPrefab;

    #region ACTION
    public static Action<Transform> cameraFollowCharacterEvent;
    #endregion

    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += HandleOnClientConnectedCallback;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        SpawnPlayer();
    }

    private void HandleOnClientConnectedCallback(ulong clientId)
    {
        SpawnPlayer(clientId);
    }

    private void SpawnPlayer()
    {
        if (IsServer)
        {
            if (playerPrefab != null)
            {
                GameObject playerObject = Instantiate(playerPrefab, GetSpawnPosition(), transform.rotation);

                NetworkObject networkPlayerObject = playerObject.GetComponent<NetworkObject>();

                networkPlayerObject.Spawn();

                cameraFollowCharacterEvent?.Invoke(playerObject.transform);
            }
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (IsServer)
        {
            if (playerPrefab != null)
            {
                GameObject playerObject = Instantiate(playerPrefab, GetSpawnPosition(), transform.rotation);

                NetworkObject networkPlayerObject = playerObject.GetComponent<NetworkObject>();

                networkPlayerObject.Spawn();
                networkPlayerObject.ChangeOwnership(clientId);

                cameraFollowCharacterEvent?.Invoke(playerObject.transform);
            }
        }
    }

    private Vector3 GetSpawnPosition()
    {
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;

        if (playerCount == 1)
        {
            return new Vector3(-8, 0, 8);
        }
        else if (playerCount == 2)
        {
            return new Vector3(8, 0, 8);
        }
        else if (playerCount == 3)
        {
            return new Vector3(8, 0, -8);
        }
        else
        {
            return new Vector3(-8, 0, -8);
        }
    }
}
