using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameTimeCounterUI : NetworkBehaviour
{
    [SerializeField] private TMP_Text remainingTimeText;

    [Header("SCRIPTABLE OBJECT")]
    [SerializeField] private GameConfiguration gameConfiguration;

    #region ACTION
    public static event Action endGameEvent;
    #endregion

    private void Awake()
    {
        StartCoroutine(Counting());
    }

    private IEnumerator Counting()
    {
        WaitForSeconds waitOneSecond = new WaitForSeconds(1);

        int remainingSecond = gameConfiguration.MaxGameTime;

        while (remainingSecond > 0)
        {
            yield return waitOneSecond;

            remainingSecond--;

            if (IsServer)
            {
                remainingTimeText.text = $"{remainingSecond}";

                SyncGameTimeRpc(remainingSecond);
            }
        }

        endGameEvent?.Invoke();
    }

    [Rpc(SendTo.NotServer)]
    private void SyncGameTimeRpc(int remainingSecond)
    {
        remainingTimeText.text = $"{remainingSecond}";
    }
}
