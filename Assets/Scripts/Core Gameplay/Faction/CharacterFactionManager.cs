using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class CharacterFactionManager : NetworkBehaviour
{
    #region PRIVATE FIELD
    private bool _isMonsterExist;
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

    private async void SelectFactionForCharacter(ulong networkObjectId, int characterIndex)
    {
        while (!IsSpawned)
        {
            await Task.Delay(200);
        }

        if (!_isMonsterExist)
        {
            int random = UnityEngine.Random.Range(0, 4);

            if (characterIndex == 1)
            {
                AssignMonster(networkObjectId);

                AssignMonsterRpc(networkObjectId, CharacterFaction.Monster);
            }
            else
            {
                if (random == 1)
                {
                    AssignMonster(networkObjectId);

                    AssignMonsterRpc(networkObjectId, CharacterFaction.Monster);

                    // changeCharacterFactionEvent?.Invoke(networkObjectId, CharacterFaction.Monster);

                    // _isMonsterExist = true;
                }
            }
        }
    }

    private void AssignMonster(ulong networkObjectId)
    {
        changeCharacterFactionEvent?.Invoke(networkObjectId, CharacterFaction.Monster);

        _isMonsterExist = true;
    }

    [Rpc(SendTo.NotMe)]
    private void AssignMonsterRpc(ulong networkObjectId, CharacterFaction characterFaction)
    {
        Debug.Log("SPAWNEEDD RRPPPC");
        changeCharacterFactionEvent?.Invoke(networkObjectId, characterFaction);

        _isMonsterExist = true;
    }
}
