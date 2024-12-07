using UnityEngine;
using static GameEnum;

public class UIScreen : MonoBehaviour
{
    [Header("CUSTOMIZE")]
    [SerializeField] private ScreenRoute route;
    [SerializeField] private bool isRouteActiveFromStart;

    private ISaferioUIAnimation _transitionAnimation;

    private void Awake()
    {
        SwitchRouteButton.switchRouteEvent += OnRouteSwitched;
        WaitingScreen.switchRouteEvent += OnRouteSwitched;

        RegisterMoreEvent();

        _transitionAnimation = GetComponent<ISaferioUIAnimation>();

        if (isRouteActiveFromStart)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

        MoreActionInAwake();
    }

    private void OnDestroy()
    {
        SwitchRouteButton.switchRouteEvent -= OnRouteSwitched;
        WaitingScreen.switchRouteEvent -= OnRouteSwitched;

        UnregisterMoreEvent();
    }

    protected virtual void RegisterMoreEvent()
    {

    }

    protected virtual void UnregisterMoreEvent()
    {

    }

    protected virtual void MoreActionInAwake()
    {

    }

    private void OnRouteSwitched(ScreenRoute route)
    {
        if (route == this.route)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    protected void Show()
    {
        _transitionAnimation.Show();
    }

    protected void Hide()
    {
        _transitionAnimation.Hide();
    }
}
