using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public enum CharacterFaction
{
    Human,
    Monster
}

public class CharacterFactionObserver : NetworkBehaviour
{
    [Header("CUSTOMIZE")]
    [SerializeField] private float invincibleDuration;

    private ulong _networkObjectId;

    [SerializeField] private CharacterFaction _characterFaction;
    private bool _isInvincible;

    public CharacterFaction CharacterFaction
    {
        get => _characterFaction;
        set => _characterFaction = value;
    }

    public bool IsInvincible
    {
        get => _isInvincible;
        set => _isInvincible = value;
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

            IsInvincible = true;

            DelayDisableInvincibleStateAsync();
        }
    }

    private async void DelayDisableInvincibleStateAsync()
    {
        await Task.Delay((int)(invincibleDuration * 1000));

        _isInvincible = false;
    }
}
