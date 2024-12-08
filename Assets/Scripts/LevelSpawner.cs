using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void Awake()
    {
        // NetworkManager.Singleton.OnClientConnectedCallback += HandleOnClientConnectedCallback;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            SpawnPlayer();
        }
        else
        {
            Debug.Log(NetworkManager.Singleton.LocalClientId);
            RequestSpawnCharacterRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void HandleOnClientConnectedCallback(ulong clientId)
    {
        SpawnPlayer(clientId);
    }

    private void HandleSceneChanged(ulong clientId, string sceneName, SceneManager transitionType)
    {
        if (IsOwner)
        {
            Debug.Log($"Client {clientId} switched to scene {sceneName} using {transitionType}");
            // Perform actions based on the scene change
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

    [Rpc(SendTo.NotServer)]
    private void SyncSpawnCharacterEventRpc(ulong networkObjectId)
    {
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        int currentPlayerIndex = playerCount - 1;

        spawnCharacterEvent?.Invoke(networkObjectId, currentPlayerIndex);
    }
}
