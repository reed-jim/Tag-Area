using Unity.Netcode;
using UnityEngine;

public class BotCharacterController : NetworkBehaviour
{
    [SerializeField] private float fleeMultiplier;
    [SerializeField] private float lerpRatio;

    private ulong _networkObjectId;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _networkObjectId = GetComponent<NetworkObject>().NetworkObjectId;
    }

    private void Awake()
    {
        CharacterVision.chaseTargetEvent += ChaseTarget;
        CharacterVision.fleeTargetEvent += FleeTarget;
    }

    public override void OnDestroy()
    {
        CharacterVision.chaseTargetEvent -= ChaseTarget;
        CharacterVision.fleeTargetEvent -= FleeTarget;
    }

    private void ChaseTarget(ulong networkObjectId, Vector3 position)
    {
        if (networkObjectId == _networkObjectId && IsSpawned)
        {
            ChaseTargetRpc(position);
        }
    }

    private void FleeTarget(ulong networkObjectId, Vector3 position)
    {
        if (networkObjectId == _networkObjectId && IsSpawned)
        {
            FleeTargetRpc(position);
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
