using PrimeTween;
using Saferio.Util.SaferioTween;
using TMPro;
using UnityEngine;

public class MonsterTimeLeaderboardUI : MonoBehaviour
{
    [SerializeField] private RectTransform container;
    [SerializeField] private TMP_Text gameResultText;

    [SerializeField] private MonsterTimeLeaderboardItemUI[] monsterTimeLeaderboardItems;

    private void Awake()
    {
        MonsterTimeLeaderboardManager.updateMonsterTimeUIEvent += UpdateUI;
        MonsterTimeLeaderboardManager.setGameResultEvent += SetGameResult;
        GameTimeCounterUI.endGameEvent += OnGameEnded;

        gameResultText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        MonsterTimeLeaderboardManager.updateMonsterTimeUIEvent -= UpdateUI;
        MonsterTimeLeaderboardManager.setGameResultEvent -= SetGameResult;
        GameTimeCounterUI.endGameEvent -= OnGameEnded;
    }

    private void UpdateUI(CharacterMonsterTimeData[] monsterTimes)
    {
        for (int i = 0; i < monsterTimes.Length; i++)
        {
            monsterTimeLeaderboardItems[i].SetPlayerName(monsterTimes[i].PlayerName);
            monsterTimeLeaderboardItems[i].SetMonsterTime(monsterTimes[i].MonsterTime);
        }
    }

    private void OnGameEnded()
    {
        // container.anchorMin = new Vector2(0.5f, 0.5f);
        // container.anchorMax = new Vector2(0.5f, 0.5f);

        Tween.LocalPosition(container, Vector3.zero, duration: 0.5f).OnComplete(() =>
        {
            gameResultText.gameObject.SetActive(true);

            gameResultText.rectTransform.localScale = Vector3.zero;

            Tween.Scale(gameResultText.rectTransform, 1, duration: 0.3f);
        });
    }

    private void SetGameResult(CharacterMonsterTimeData winnerData)
    {
        gameResultText.text = $"{winnerData.PlayerName} won the game!";
    }
}
