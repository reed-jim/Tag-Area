using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class CharacterVision : NetworkBehaviour
{
    [SerializeField] private CharacterFactionObserver characterFactionObserver;

    [Header("SCRIPTABLE OBJECT")]
    [SerializeField] private BoolVariable isBotVariable;

    [Header("CUSTOMIZE")]
    [SerializeField] private float radiusCheck;
    [SerializeField] private LayerMask layerMaskCheck;

    #region ACTION
    public static event Action<ulong, Vector3> chaseTargetEvent;
    public static event Action<ulong, Vector3> fleeTargetEvent;
    public static event Action<ulong, bool> setIsFoundTargetEvent;
    #endregion

    private ulong _networkObjectId;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _networkObjectId = GetComponent<NetworkObject>().NetworkObjectId;

        DelayStartSeekingTargetAsync();
    }

    [Rpc(SendTo.NotMe)]
    private void SeekTargetRPC(ulong networkObjectId)
    {
        StartSeekingTarget(networkObjectId);
    }

    private async void DelayStartSeekingTargetAsync()
    {
        while (!IsSpawned)
        {
            await Task.Delay(200);
        }

        await Task.Delay(1000);

        if (IsOwner && isBotVariable.Value)
        {
            StartCoroutine(SeekingTarget());

            SeekTargetRPC(_networkObjectId);
        }
    }

    private void StartSeekingTarget(ulong networkObjectId)
    {
        if (networkObjectId == _networkObjectId)
        {
            StartCoroutine(SeekingTarget());
        }
    }

    private IEnumerator SeekingTarget()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.2f);

        yield return new WaitUntil(() => IsSpawned);

        while (true)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radiusCheck, layerMaskCheck);

            bool isTargetFound = false;

            if (colliders != null)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    CharacterFactionObserver targetCharacterFactionObserver = colliders[i].GetComponent<CharacterFactionObserver>();

                    if (targetCharacterFactionObserver != null)
                    {
                        if (characterFactionObserver.CharacterFaction == CharacterFaction.Monster &&
                            targetCharacterFactionObserver.CharacterFaction == CharacterFaction.Human)
                        {
                            chaseTargetEvent?.Invoke(_networkObjectId, colliders[i].transform.position);

                            isTargetFound = true;

                            // ChaseTargetRpc(colliders[i].transform.position);
                        }

                        if (characterFactionObserver.CharacterFaction == CharacterFaction.Human &&
                            targetCharacterFactionObserver.CharacterFaction == CharacterFaction.Monster)
                        {
                            fleeTargetEvent?.Invoke(_networkObjectId, colliders[i].transform.position);

                            isTargetFound = true;

                            // FleeTargetRpc(colliders[i].transform.position);
                        }
                    }
                }
            }

            setIsFoundTargetEvent?.Invoke(_networkObjectId, isTargetFound);

            yield return waitForSeconds;
        }
    }

    // [Rpc(SendTo.Everyone)]
    // private void ChaseTargetRpc(Vector3 position)
    // {
    //     chaseTargetEvent?.Invoke(_networkObjectId, position);
    // }

    // [Rpc(SendTo.Everyone)]
    // private void FleeTargetRpc(Vector3 position)
    // {
    //     fleeTargetEvent?.Invoke(_networkObjectId, position);
    // }
}
