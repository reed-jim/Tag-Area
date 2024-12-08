using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class CharacterMovementWithClientPrediction : NetworkBehaviour
{
    [SerializeField] private TrailRenderer trail;

    [Header("CUSTOMIZE")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interpolationFactor = 10f;

    #region PRIVATE FIELD
    private ulong _networkObjectId;

    private Vector3 predictedPosition;
    private Vector3 serverPosition;

    private float lastInputTime = 0f;
    private Vector3 inputDirection;

    private bool _isMovable;
    private bool _isSpeedBoosting;
    #endregion

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _networkObjectId = GetComponent<NetworkObject>().NetworkObjectId;

        trail.gameObject.SetActive(false);

        DelayEnableMovementAsync();
    }

    private void Awake()
    {
        SpeedBooster.boostSpeedEvent += BoostSpeed;

        predictedPosition = transform.position;
        serverPosition = transform.position;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        SpeedBooster.boostSpeedEvent -= BoostSpeed;
    }

    private void Update()
    {
        if (!_isMovable)
        {
            return;
        }

        if (IsOwner)
        {
            HandleMovementInput();
            PredictMovement();
            transform.position = predictedPosition;

            if (Time.time - lastInputTime > 0.1f)
            {
                lastInputTime = Time.time;
                SubmitMovementInputServerRpc(inputDirection);
            }
        }
        else
        {
            InterpolatePosition();
        }
    }

    private async void DelayEnableMovementAsync()
    {
        await Task.Delay(3000);

        _isMovable = true;
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
    }

    private void PredictMovement()
    {
        if (inputDirection != Vector3.zero)
        {
            predictedPosition = transform.position + inputDirection * moveSpeed * Time.deltaTime;
        }
    }

    private void InterpolatePosition()
    {
        transform.position = Vector3.Lerp(transform.position, serverPosition, Time.deltaTime * interpolationFactor);
    }

    private async void BoostSpeed(ulong networkObjectId)
    {
        if (networkObjectId != _networkObjectId)
        {
            return;
        }

        if (_isSpeedBoosting)
        {
            return;
        }

        moveSpeed *= 1.5f;

        trail.gameObject.SetActive(true);

        await Task.Delay(5000);

        moveSpeed /= 1.5f;

        trail.emitting = false;

        await Task.Delay((int)(trail.time * 1000));

        trail.gameObject.SetActive(false);

        _isSpeedBoosting = false;
    }

    [Rpc(SendTo.NotServer)]
    private void SubmitMovementInputServerRpc(Vector3 inputDirection)
    {
        if (IsOwner)
        {
            serverPosition = transform.position + inputDirection * moveSpeed * Time.deltaTime;

            UpdateServerPositionClientRpc(serverPosition);
        }
    }

    [Rpc(SendTo.Server)]
    private void UpdateServerPositionClientRpc(Vector3 newServerPosition)
    {
        if (!IsOwner)
        {
            serverPosition = newServerPosition;
        }
    }
}
