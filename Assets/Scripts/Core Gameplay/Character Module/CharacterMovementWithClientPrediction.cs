using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class CharacterMovementWithClientPrediction : NetworkBehaviour
{
    [SerializeField] private TrailRenderer speedBoostTrail;
    [SerializeField] private TrailRenderer monsterTrail;

    [Header("SCRIPTABLE OBJECT")]
    [SerializeField] private BoolVariable isBotVariable;

    [Header("CUSTOMIZE")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interpolationFactor = 10f;

    #region PRIVATE FIELD
    private ulong _networkObjectId;

    private Vector3 predictedPosition;
    private Vector3 serverPosition;

    private float _currentMoveSpeed;

    private float lastInputTime = 0f;
    private Vector3 inputDirection;

    private bool _isMovable;
    private bool _isSpeedBoosting;

    [SerializeField] private bool _isBot;
    #endregion

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _networkObjectId = GetComponent<NetworkObject>().NetworkObjectId;

        speedBoostTrail.gameObject.SetActive(false);
        monsterTrail.gameObject.SetActive(false);
    }

    private void Awake()
    {
        LevelSpawner.spawnCharacterEvent += EnableCharacterMovement;
        CharacterFactionManager.changeCharacterFactionEvent += EnableMonsterTrail;
        CharacterCollision.changeCharacterFactionEvent += EnableMonsterTrail;
        SpeedBooster.boostSpeedEvent += BoostSpeed;
        BotCharacterController.setBotCharacterDirectionEvent += SetBotDirection;

        _currentMoveSpeed = moveSpeed;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        LevelSpawner.spawnCharacterEvent -= EnableCharacterMovement;
        CharacterFactionManager.changeCharacterFactionEvent -= EnableMonsterTrail;
        CharacterCollision.changeCharacterFactionEvent -= EnableMonsterTrail;
        SpeedBooster.boostSpeedEvent -= BoostSpeed;
        BotCharacterController.setBotCharacterDirectionEvent -= SetBotDirection;
    }

    private void Update()
    {
        if (!_isMovable)
        {
            return;
        }

        if (IsOwner)
        {
            if (!isBotVariable.Value)
            {
                HandleMovementInput();
                PredictMovement();
            }

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

    private void EnableCharacterMovement(ulong networkObjectId, int currentPlayerIndex)
    {
        if (networkObjectId == _networkObjectId)
        {
            predictedPosition = transform.position;
            serverPosition = transform.position;

            _isMovable = true;
        }
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
            predictedPosition = transform.position + inputDirection * _currentMoveSpeed * Time.deltaTime;
        }
    }

    private void SetBotDirection(ulong networkObjectId, Vector3 direction)
    {
        if (networkObjectId == _networkObjectId)
        {
            predictedPosition = transform.position + direction * _currentMoveSpeed * Time.deltaTime;
        }
    }

    private void InterpolatePosition()
    {
        transform.position = Vector3.Lerp(transform.position, serverPosition, Time.deltaTime * interpolationFactor);
    }

    private async void EnableMonsterTrail(ulong networkObjectId, CharacterFaction characterFaction)
    {
        if (networkObjectId != _networkObjectId)
        {
            return;
        }

        if (characterFaction == CharacterFaction.Monster)
        {
            _currentMoveSpeed = 1.2f * moveSpeed;

            monsterTrail.gameObject.SetActive(true);
        }
        else
        {
            _currentMoveSpeed = moveSpeed;

            await Task.Delay((int)(monsterTrail.time * 1000));

            monsterTrail.gameObject.SetActive(false);
        }
    }

    private async void BoostSpeed(ulong networkObjectId, float speedMultiplier, float duration)
    {
        if (networkObjectId != _networkObjectId)
        {
            return;
        }

        if (_isSpeedBoosting)
        {
            return;
        }

        _currentMoveSpeed = speedMultiplier * moveSpeed;

        speedBoostTrail.gameObject.SetActive(true);

        await Task.Delay((int)(duration * 1000));

        _currentMoveSpeed = moveSpeed;

        speedBoostTrail.emitting = false;

        await Task.Delay((int)(speedBoostTrail.time * 1000));

        speedBoostTrail.gameObject.SetActive(false);

        _isSpeedBoosting = false;
    }

    [Rpc(SendTo.NotServer)]
    private void SubmitMovementInputServerRpc(Vector3 inputDirection)
    {
        if (IsOwner)
        {
            serverPosition = transform.position + inputDirection * _currentMoveSpeed * Time.deltaTime;

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
