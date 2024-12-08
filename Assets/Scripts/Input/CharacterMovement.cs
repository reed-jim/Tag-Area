using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class CharacterMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    [SerializeField] private float lerpRatio;
    private Rigidbody rb;

    #region PRIVATE FIELD
    private ulong _networkObjectId;
    private Vector3 _predictedPosition;
    #endregion

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _networkObjectId = GetComponent<NetworkObject>().NetworkObjectId;
    }

    void Awake()
    {
        SpeedBooster.boostSpeedEvent += BoostSpeed;

        rb = GetComponent<Rigidbody>();

        _predictedPosition = transform.position;
    }

    public override void OnDestroy()
    {
        SpeedBooster.boostSpeedEvent -= BoostSpeed;
    }

    void Update()
    {
        MoveCharacter();

        if (IsServer && !IsOwner)
        {
            transform.position = Vector3.Lerp(transform.position, _predictedPosition, lerpRatio);
        }
    }

    private void MoveCharacter()
    {
        if (IsOwner)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 velocity = new Vector3(moveX, 0, moveZ) * moveSpeed;

            transform.position += velocity * Time.deltaTime;

            if (IsHost)
            {
                // rb.linearVelocity = velocity;
            }
            else
            {
                SyncCharacterPositionRpc(transform.position + velocity * Time.deltaTime);
            }
        }
    }

    private async void BoostSpeed(ulong networkObjectId)
    {
        if (networkObjectId != _networkObjectId)
        {
            return;
        }

        moveSpeed *= 1.5f;

        await Task.Delay(5000);

        moveSpeed /= 1.5f;
    }

    [Rpc(SendTo.Server)]
    private void SyncCharacterPositionRpc(Vector3 predictedPosition)
    {
        // Vector3 predictedPosition = transform.position + velocity * Time.deltaTime;

        _predictedPosition = predictedPosition;

        // transform.position = predictedPosition;
    }
}
