using PrimeTween;
using Saferio.Util.SaferioTween;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private Image fadeBackground;

    private void Awake()
    {
        GameTimeCounterUI.endGameEvent += OnGameEnded;

        fadeBackground.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameTimeCounterUI.endGameEvent -= OnGameEnded;
    }

    private void OnGameEnded()
    {
        fadeBackground.gameObject.SetActive(true);

        Tween.Alpha(fadeBackground, 0.85f, duration: 0.5f);
    }
}
