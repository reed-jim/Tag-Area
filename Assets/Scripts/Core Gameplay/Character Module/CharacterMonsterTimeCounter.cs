using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CharacterMonsterTimeCounter : NetworkBehaviour
{
    [SerializeField] private CharacterFactionObserver characterFactionObserver;

    [SerializeField] private int maxTime;

    private int _monsterTime;
    private ulong _networkObjectId;

    #region ACTION
    public static event Action<ulong, int> updateMonsterTimeEvent;
    #endregion  

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _networkObjectId = GetComponent<NetworkObject>().NetworkObjectId;
    }

    private void Awake()
    {
        StartCoroutine(Counting());
    }

    private IEnumerator Counting()
    {
        WaitForSeconds waitOneSecond = new WaitForSeconds(1);

        int timePassed = 0;

        yield return new WaitUntil(() => IsSpawned);

        while (timePassed < maxTime)
        {
            yield return waitOneSecond;

            if (characterFactionObserver.CharacterFaction == CharacterFaction.Monster)
            {
                _monsterTime++;

                updateMonsterTimeEvent?.Invoke(_networkObjectId, _monsterTime);
            }

            timePassed++;
        }
    }
}
