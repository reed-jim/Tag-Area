using TMPro;
using UnityEngine;

public class MonsterTimeLeaderboardItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text timeText;

    public void SetPlayerName(string playerName)
    {
        playerNameText.text = playerName;
    }

    public void SetMonsterTime(int monsterTime)
    {
        timeText.text = $"{monsterTime}";
    }
}
