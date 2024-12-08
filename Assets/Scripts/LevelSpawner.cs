using System;
using Unity.Netcode;
using UnityEngine;

public class LevelSpawner : NetworkBehaviour
{
    public GameObject playerPrefab;

    #region PRIVATE FIELD
    private int _numPlayerSpawned;
    #endregion

    #region ACTION
    public static Action<ulong, int> spawnCharacterEvent;
    public static Action<Transform> cameraFollowCharacterEvent;
    #endregion

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            SpawnPlayer();
        }
        else
        {
            RequestSpawnCharacterRpc(NetworkManager.Singleton.LocalClientId);
        }
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

                int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
                int currentPlayerIndex = playerCount - 1;

                spawnCharacterEvent?.Invoke(networkPlayerObject.NetworkObjectId, playerCount);
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

                SyncSpawnCharacterEventRpc(networkPlayerObject.NetworkObjectId);
            }
        }
    }

    private Vector3 GetSpawnPosition()
    {
        _numPlayerSpawned++;

        if (_numPlayerSpawned == 1)
        {
            return new Vector3(-8, 1, 8);
        }
        else if (_numPlayerSpawned == 2)
        {
            return new Vector3(8, 1, 8);
        }
        else if (_numPlayerSpawned == 3)
        {
            return new Vector3(8, 1, -8);
        }
        else
        {
            return new Vector3(-8, 1, -8);
        }
    }

    [Rpc(SendTo.Server)]
    private void RequestSpawnCharacterRpc(ulong clientId)
    {
        SpawnPlayer(clientId);
    }

    [Rpc(SendTo.Everyone)]
    private void SyncSpawnCharacterEventRpc(ulong networkObjectId)
    {
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        int currentPlayerIndex = playerCount - 1;

        spawnCharacterEvent?.Invoke(networkObjectId, playerCount);
    }
}
