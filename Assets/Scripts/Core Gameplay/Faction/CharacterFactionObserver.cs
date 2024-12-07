using UnityEngine;

public enum CharacterFaction
{
    Human,
    Monster
}

public class CharacterFactionObserver : MonoBehaviour
{
    private CharacterFaction _characterFaction;

    public CharacterFaction CharacterFaction
    {
        get => _characterFaction;
        set => _characterFaction = value;
    }
}
