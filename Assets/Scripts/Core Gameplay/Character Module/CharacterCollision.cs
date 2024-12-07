using System;
using Unity.Netcode;
using UnityEngine;

public class CharacterCollision : NetworkBehaviour
{
    [SerializeField] private CharacterFactionObserver characterFactionObserver;

    #region PRIVATE FIELD
    private ulong _networkObjectId;
    #endregion

    #region ACTION
    public static event Action<ulong, CharacterFaction> changeCharacterFactionEvent;
    #endregion

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _networkObjectId = GetComponent<NetworkObject>().NetworkObjectId;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (characterFactionObserver.CharacterFaction == CharacterFaction.Human)
        {
            return;
        }

        CharacterFactionObserver otherCharacterFactionObserver = other.GetComponent<CharacterFactionObserver>();

        if (otherCharacterFactionObserver != null)
        {
            if (otherCharacterFactionObserver.CharacterFaction == CharacterFaction.Human)
            {
                SyncCollisionRpc(
                    _networkObjectId,
                    otherCharacterFactionObserver.GetComponent<NetworkObject>().NetworkObjectId,
                    CharacterFaction.Human,
                    CharacterFaction.Monster
                );

                // otherCharacterFactionObserver.CharacterFaction = CharacterFaction.Monster;
                // characterFactionObserver.CharacterFaction = CharacterFaction.Human;

                // changeCharacterFactionEvent?.Invoke(otherCharacterFactionObserver.GetComponent<NetworkObject>().NetworkObjectId, otherCharacterFactionObserver.CharacterFaction);
                // changeCharacterFactionEvent?.Invoke(_networkObjectId, characterFactionObserver.CharacterFaction);
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    private void SyncCollisionRpc(ulong networkObjectId, ulong otherNetworkObjectId, CharacterFaction characterFaction, CharacterFaction otherCharacterFaction)
    {
        changeCharacterFactionEvent?.Invoke(otherNetworkObjectId, otherCharacterFaction);
        changeCharacterFactionEvent?.Invoke(networkObjectId, characterFaction);
    }
}
