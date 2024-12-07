using UnityEngine;

public class MonsterTimeLeaderboardUI : MonoBehaviour
{
    [SerializeField] private MonsterTimeLeaderboardItemUI[] monsterTimeLeaderboardItems;

    private void Awake()
    {
        MonsterTimeLeaderboardManager.updateMonsterTimeUIEvent += UpdateUI;
    }

    private void OnDestroy()
    {
        MonsterTimeLeaderboardManager.updateMonsterTimeUIEvent -= UpdateUI;
    }

    private void UpdateUI(CharacterMonsterTimeData[] monsterTimes)
    {
        for (int i = 0; i < monsterTimes.Length; i++)
        {
            monsterTimeLeaderboardItems[i].SetPlayerName(monsterTimes[i].PlayerName);
            monsterTimeLeaderboardItems[i].SetMonsterTime(monsterTimes[i].MonsterTime);
        }
    }
}
