using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CharacterMonsterTimeCounter : NetworkBehaviour
{
    [SerializeField] private CharacterFactionObserver characterFactionObserver;

    [Header("SCRIPTABLE OBJECT")]
    [SerializeField] private GameConfiguration gameConfiguration;

    #region PRIVATE FIELD
    private int _monsterTime;
    private ulong _networkObjectId;
    #endregion

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

        int maxTime = gameConfiguration.MaxGameTime;

        updateMonsterTimeEvent?.Invoke(_networkObjectId, _monsterTime);

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
