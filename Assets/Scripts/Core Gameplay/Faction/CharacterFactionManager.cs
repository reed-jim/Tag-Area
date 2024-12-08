using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class CharacterFactionManager : NetworkBehaviour
{
    #region PRIVATE FIELD
    private bool _isMonsterExist;
    private List<ulong> _characterNetworkObjectIds;
    #endregion

    #region ACTION
    public static event Action<ulong, CharacterFaction> changeCharacterFactionEvent;
    #endregion

    private void Awake()
    {
        LevelSpawner.spawnCharacterEvent += SelectFactionForCharacter;

        _characterNetworkObjectIds = new List<ulong>();

        StartCoroutine(EnsureThereIsOneMonster());
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        LevelSpawner.spawnCharacterEvent -= SelectFactionForCharacter;
    }

    private IEnumerator EnsureThereIsOneMonster()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.2f);

        while (!_isMonsterExist)
        {
            foreach (var id in _characterNetworkObjectIds)
            {
                int random = UnityEngine.Random.Range(0, 4);

                if (random == 1)
                {
                    AssignMonsterRpc(id, CharacterFaction.Monster);

                    _isMonsterExist = true;

                    break;
                }
            }

            yield return waitForSeconds;
        }
    }

    private async void SelectFactionForCharacter(ulong networkObjectId, int playerCount)
    {
        while (!IsSpawned)
        {
            await Task.Delay(200);
        }

        if (!IsServer)
        {
            return;
        }

        _characterNetworkObjectIds.Add(networkObjectId);
    }

    [Rpc(SendTo.Everyone)]
    private void AssignMonsterRpc(ulong networkObjectId, CharacterFaction characterFaction)
    {
        changeCharacterFactionEvent?.Invoke(networkObjectId, characterFaction);

        _isMonsterExist = true;
    }
}
