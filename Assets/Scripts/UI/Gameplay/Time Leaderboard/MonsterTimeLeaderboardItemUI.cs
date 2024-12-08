using TMPro;
using UnityEngine;

public class MonsterTimeLeaderboardItemUI : MonoBehaviour
{
    [SerializeField] private RectTransform container;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text timeText;

    private void Awake()
    {
        container.gameObject.SetActive(false);
    }

    public void SetPlayerName(string playerName)
    {
        if (!container.gameObject.activeSelf)
        {
            container.gameObject.SetActive(true);
        }

        playerNameText.text = playerName;
    }

    public void SetMonsterTime(int monsterTime)
    {
        timeText.text = $"{monsterTime}";
        timeText.color = new Color((255 - monsterTime) / 255f, (130 - monsterTime) / 255f, (130 - monsterTime) / 255f, 1);
    }
}
