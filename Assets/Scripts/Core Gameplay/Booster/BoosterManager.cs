using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BoosterManager : NetworkBehaviour
{
    [SerializeField] private GameObject booster;
    [SerializeField] private int secondToSpawnBooster;

    public static event Action<ulong, Vector3> syncPositionEvent;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            StartCoroutine(SpawnBooster());
        }
    }

    private IEnumerator SpawnBooster()
    {

        WaitForSeconds waitOneSecond = new WaitForSeconds(1);

        int remainingTimeToSpawnBooster = secondToSpawnBooster;

        while (true)
        {
            if (remainingTimeToSpawnBooster == 0)
            {
                // GameObject boosterObject = ObjectPoolingEverything.GetFromPool(GameConstants.BOOSTER);

                GameObject boosterObject = Instantiate(booster);

                NetworkObject networkBoosterObject = boosterObject.GetComponent<NetworkObject>();

                boosterObject.transform.position = GetRandomPosition();

                networkBoosterObject.Spawn();

                boosterObject.SetActive(true);

                remainingTimeToSpawnBooster = secondToSpawnBooster;
            }
            else
            {
                remainingTimeToSpawnBooster--;
            }

            yield return waitOneSecond;
        }
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 position = new Vector3();

        position.x = 2 * UnityEngine.Random.Range(-4, 4);
        position.y = 8;
        position.z = 2 * UnityEngine.Random.Range(-4, 4);

        return position;
    }
}
