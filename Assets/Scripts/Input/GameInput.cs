using Unity.Netcode;
using UnityEngine;

public class GameInput : NetworkBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        if (IsOwner)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 velocity = new Vector3(moveX, 0, moveZ) * moveSpeed;

            if (IsHost)
            {
                rb.linearVelocity = velocity;
            }
            else
            {
                SyncCharacterPositionRpc(velocity);
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void SyncCharacterPositionRpc(Vector3 velocity)
    {
        if (!IsOwner)
        {
            Vector3 predictedPosition = transform.position + velocity * Time.fixedDeltaTime;

            transform.position = predictedPosition;
        }
    }
}
