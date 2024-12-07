using System;
using UnityEngine;
using UnityEngine.UI;
using static GameEnum;

public class WaitingScreen : UIScreen
{
    [SerializeField] private Button continueButton;

    #region ACTION
    public static event Action<ScreenRoute> switchRouteEvent;
    #endregion

    protected override void RegisterMoreEvent()
    {
        continueButton.onClick.AddListener(ToLobbyScreen);
    }

    private void ToLobbyScreen()
    {
        switchRouteEvent?.Invoke(ScreenRoute.Lobby);
    }
}
