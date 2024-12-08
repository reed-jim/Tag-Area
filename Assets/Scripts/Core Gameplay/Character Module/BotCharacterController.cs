using System;
using Unity.Netcode;
using UnityEngine;

public class BotCharacterController : NetworkBehaviour
{
    [SerializeField] private float fleeMultiplier;
    [SerializeField] private float lerpRatio;

    private ulong _networkObjectId;
    private Vector3 _direction;
    private bool _isTargetFound;

    #region ACTION
    public static event Action<ulong, Vector3> setBotCharacterDirectionEvent;
    #endregion

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _networkObjectId = GetComponent<NetworkObject>().NetworkObjectId;
    }

    private void Awake()
    {
        CharacterVision.chaseTargetEvent += ChaseTarget;
        CharacterVision.fleeTargetEvent += FleeTarget;
        CharacterVision.setIsFoundTargetEvent += SetIsFoundTarget;
    }

    private void Update()
    {
        if (_isTargetFound)
        {
            setBotCharacterDirectionEvent?.Invoke(_networkObjectId, _direction);
        }
    }

    public override void OnDestroy()
    {
        CharacterVision.chaseTargetEvent -= ChaseTarget;
        CharacterVision.fleeTargetEvent -= FleeTarget;
        CharacterVision.setIsFoundTargetEvent -= SetIsFoundTarget;
    }

    private void ChaseTarget(ulong networkObjectId, Vector3 position)
    {
        if (networkObjectId == _networkObjectId && IsSpawned)
        {
            _direction = (position - transform.position).normalized;

            _direction.y = 0;
        }
    }

    private void FleeTarget(ulong networkObjectId, Vector3 position)
    {
        if (networkObjectId == _networkObjectId && IsSpawned)
        {
            _direction = (transform.position - position).normalized;

            _direction.y = 0;
        }
    }

    private void SetIsFoundTarget(ulong networkObjectId, bool isFound)
    {
        if (networkObjectId == _networkObjectId && IsSpawned)
        {
            _isTargetFound = isFound;
        }

        if (!isFound)
        {
            setBotCharacterDirectionEvent?.Invoke(_networkObjectId, Vector3.zero);
        }
    }

    [Rpc(SendTo.Server)]
    private void ChaseTargetRpc(Vector3 position)
    {
        transform.position = Vector3.Lerp(transform.position, position, lerpRatio);
    }

    [Rpc(SendTo.Server)]
    private void FleeTargetRpc(Vector3 position)
    {
        Vector3 destination = fleeMultiplier * (transform.position - position);

        destination.y = transform.position.y;

        transform.position = Vector3.Lerp(transform.position, destination, lerpRatio);
    }
}
