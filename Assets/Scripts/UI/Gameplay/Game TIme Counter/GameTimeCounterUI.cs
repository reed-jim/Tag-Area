using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameTimeCounterUI : NetworkBehaviour
{
    [SerializeField] private TMP_Text remainingTimeText;

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

        int remainingSecond = 120;

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
    }

    [Rpc(SendTo.NotServer)]
    private void SyncGameTimeRpc(int remainingSecond)
    {
        remainingTimeText.text = $"{remainingSecond}";
    }
}
