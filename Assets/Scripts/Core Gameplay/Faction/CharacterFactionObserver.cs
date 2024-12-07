using Unity.Netcode;
using UnityEngine;

public enum CharacterFaction
{
    Human,
    Monster
}

public class CharacterFactionObserver : NetworkBehaviour
{
    private ulong _networkObjectId;

    private CharacterFaction _characterFaction;

    public CharacterFaction CharacterFaction
    {
        get => _characterFaction;
        set => _characterFaction = value;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _networkObjectId = GetComponent<NetworkObject>().NetworkObjectId;
    }

    private void Awake()
    {
        CharacterFactionManager.changeCharacterFactionEvent += ChangeFaction;
        CharacterCollision.changeCharacterFactionEvent += ChangeFaction;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        
        CharacterFactionManager.changeCharacterFactionEvent -= ChangeFaction;
        CharacterCollision.changeCharacterFactionEvent -= ChangeFaction;
    }

    private void ChangeFaction(ulong networkObjectId, CharacterFaction characterFaction)
    {
        if (networkObjectId == _networkObjectId)
        {
            _characterFaction = characterFaction;
        }
    }
}
