using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class CharacterFactionManager : NetworkBehaviour
{
    #region PRIVATE FIELD
    private bool _isMonsterExist;
    private int _numberPlayerGotFaction;
    #endregion

    #region ACTION
    public static event Action<ulong, CharacterFaction> changeCharacterFactionEvent;
    #endregion

    private void Awake()
    {
        LevelSpawner.spawnCharacterEvent += SelectFactionForCharacter;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        LevelSpawner.spawnCharacterEvent -= SelectFactionForCharacter;
    }

    private async void SelectFactionForCharacter(ulong networkObjectId, int playerCount)
    {
        if (!IsServer)
        {
            return;
        }

        while (!IsSpawned)
        {
            await Task.Delay(200);
        }

        Debug.Log("FACTIONA " + playerCount + "/" + _numberPlayerGotFaction + "/" + _isMonsterExist);

        if (!_isMonsterExist)
        {
            int random = UnityEngine.Random.Range(0, 999);

            if (_numberPlayerGotFaction == playerCount - 1)
            {
                // AssignMonster(networkObjectId);

                AssignMonsterRpc(networkObjectId, CharacterFaction.Monster);
            }
            else
            {
                if (random == 1)
                {
                    // AssignMonster(networkObjectId);

                    AssignMonsterRpc(networkObjectId, CharacterFaction.Monster);
                }
            }
        }

        _numberPlayerGotFaction++;
    }

    private void AssignMonster(ulong networkObjectId)
    {
        changeCharacterFactionEvent?.Invoke(networkObjectId, CharacterFaction.Monster);

        _isMonsterExist = true;
    }

    [Rpc(SendTo.Everyone)]
    private void AssignMonsterRpc(ulong networkObjectId, CharacterFaction characterFaction)
    {
        changeCharacterFactionEvent?.Invoke(networkObjectId, characterFaction);

        _isMonsterExist = true;
    }
}
