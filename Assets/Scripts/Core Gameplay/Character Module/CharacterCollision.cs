using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class CharacterCollision : NetworkBehaviour
{
    [SerializeField] private CharacterFactionObserver characterFactionObserver;

    [Header("CUSTOMIZE")]
    [SerializeField] private float durationBetweenTwoCollisionWithCharacter;

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
        IBooster booster = other.GetComponent<IBooster>();

        if (booster != null)
        {
            booster.Boost(_networkObjectId);
        }

        if (characterFactionObserver.CharacterFaction == CharacterFaction.Human)
        {
            return;
        }

        CharacterFactionObserver otherCharacterFactionObserver = other.GetComponent<CharacterFactionObserver>();

        if (otherCharacterFactionObserver != null)
        {
            if (otherCharacterFactionObserver.CharacterFaction == CharacterFaction.Human && !otherCharacterFactionObserver.IsInvincible)
            {
                GameObject tapSound = ObjectPoolingEverything.GetFromPool(GameConstants.TAG_SOUND);

                tapSound.SetActive(true);

                tapSound.GetComponent<AudioSource>().Play();

                SyncCollisionRpc(
                    _networkObjectId,
                    otherCharacterFactionObserver.GetComponent<NetworkObject>().NetworkObjectId,
                    CharacterFaction.Human,
                    CharacterFaction.Monster
                );
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
