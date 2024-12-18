using System;
using PrimeTween;
using Unity.Netcode;
using UnityEngine;

public class SpeedBooster : NetworkBehaviour, IBooster
{
    [SerializeField] private float speedMultiplier;
    [SerializeField] private float duration;

    public static event Action<ulong, float, float> boostSpeedEvent;

    #region PRIVATE FIELD
    private ulong _networkObjectId;
    #endregion

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _networkObjectId = GetComponent<NetworkObject>().NetworkObjectId;
    }

    public void Boost(ulong networkObjectId)
    {
        SyncSpeedBoostRpc(networkObjectId);
    }

    [Rpc(SendTo.Everyone)]
    private void SyncSpeedBoostRpc(ulong networkObjectId)
    {
        boostSpeedEvent?.Invoke(networkObjectId, speedMultiplier, duration);

        Tween.Scale(transform, 0, duration: 0.5f).OnComplete(() =>
        {
            transform.localScale = Vector3.one;

            gameObject.SetActive(false);
        });
    }
}
