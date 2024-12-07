using UnityEngine;

public class CharacterMonsterTimeData
{
    public string playerName;
    private int monsterTime;

    public string PlayerName
    {
        get => playerName;
        set => playerName = value;
    }

    public int MonsterTime
    {
        get => monsterTime;
        set => monsterTime = value;
    }

    public CharacterMonsterTimeData(ulong networkObjectId, int monsterTime)
    {
        playerName = $"Player {networkObjectId}";
        this.monsterTime = monsterTime;
    }
}
