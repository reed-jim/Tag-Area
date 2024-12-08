using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Saferio/IntVariable")]
public class IntVariable : ScriptableObject
{
    [SerializeField] private int value;

    public int Value
    {
        get => value;
        set => this.value = value;
    }
}
