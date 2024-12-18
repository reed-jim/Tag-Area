using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class MovementData
{
    private int _tick;
    private Vector3 _speed;

    public MovementData()
    {

    }

    public MovementData(int tick, Vector3 speed)
    {
        _tick = tick;
        _speed = speed;
    }

    public int Tick
    {
        get => _tick;
        set => _tick = value;
    }

    public Vector3 Speed
    {
        get => _speed;
        set => _speed = value;
    }
}

public class CharacterMovementWithClientPrediction : NetworkBehaviour
{
    [Header("TRAIL")]
    [SerializeField] private TrailRenderer speedBoostTrail;
    [SerializeField] private TrailRenderer monsterTrail;

    [Header("SCRIPTABLE OBJECT")]
    [SerializeField] private BoolVariable isBotVariable;

    [Header("CUSTOMIZE")]
    [SerializeField] private float moveSpeed = 5f;

    #region PRIVATE FIELD
    private ulong _networkObjectId;
    private Vector3 inputDirection;

    private float _currentMoveSpeed;
    private bool _isMovable;

    private bool _isSpeedBoosting;
    [SerializeField] private bool _isBot;

    #region CLIENT
    private Vector3 _compensatedPosition;
    private Vector3 _speed;
    // private Vector3 _lastSpeed;
    // private int _serverTickPassed;
    // private int _lastServerTickPassed;
    private float _lastServerSyncTime;



    private const int BUFFER_SIZE = 1024;
    private MovementData[] movementDatum = new MovementData[BUFFER_SIZE];
    private int _clientTickTime;
    private int _lastClientTickTime;
    #endregion
    #endregion

    #region LIFE CYCLE
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
        BotCharacterController.setBotCharacterDirectionEvent += SetBotMovementDirection;

        _currentMoveSpeed = moveSpeed;

        for (int i = 0; i < movementDatum.Length; i++)
        {
            movementDatum[i] = new MovementData();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        LevelSpawner.spawnCharacterEvent -= EnableCharacterMovement;
        CharacterFactionManager.changeCharacterFactionEvent -= EnableMonsterTrail;
        CharacterCollision.changeCharacterFactionEvent -= EnableMonsterTrail;
        SpeedBooster.boostSpeedEvent -= BoostSpeed;
        BotCharacterController.setBotCharacterDirectionEvent -= SetBotMovementDirection;
    }

    private void Update()
    {
        if (!_isMovable)
        {
            return;
        }

        if (IsServer)
        {
            if (IsOwner)
            {
                if (!_isBot)
                {
                    HandleMovementInput();
                }

                transform.position = transform.position + inputDirection * _currentMoveSpeed;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, _compensatedPosition, 0.3333f);
            }
        }
        else
        {
            if (IsOwner)
            {
                if (!_isBot)
                {
                    HandleMovementInput();

                    _speed = inputDirection * _currentMoveSpeed;

                    transform.position = transform.position + inputDirection * _currentMoveSpeed;

                    _clientTickTime++;

                    movementDatum[_clientTickTime % BUFFER_SIZE] = new MovementData(_clientTickTime, _speed);
                }

                if (Time.time - _lastServerSyncTime > 0.03f)
                {
                    Vector3 predictedPosition = Vector3.zero;

                    for (int i = _lastClientTickTime; i < _clientTickTime; i++)
                    {
                        int index = i % BUFFER_SIZE;

                        predictedPosition += movementDatum[index].Speed;
                    }

                    SyncClientSpeedRpc(_networkObjectId, predictedPosition);

                    _lastServerSyncTime = Time.time;
                    _lastClientTickTime = _clientTickTime;
                }
            }
        }
    }
    #endregion

    #region CLIENT
    [Rpc(SendTo.Server)]
    private void SyncClientSpeedRpc(ulong networkObjectId, Vector3 predictedPosition)
    {
        if (networkObjectId == _networkObjectId && !IsOwner)
        {
            CompensatePosition(predictedPosition);
        }
    }

    private void CompensatePosition(Vector3 predictedPosition)
    {
        _compensatedPosition += predictedPosition;
    }
    #endregion

    #region BOT
    [Rpc(SendTo.NotMe)]
    private void SyncIsBotRpc(ulong networkObjectId)
    {
        if (networkObjectId == _networkObjectId)
        {
            _isBot = true;
        }
    }

    private void SetBotMovementDirection(ulong networkObjectId, Vector3 direction)
    {
        if (networkObjectId == _networkObjectId)
        {
            _speed = direction * _currentMoveSpeed;
        }
    }
    #endregion

    // This will be moved to another module later
    #region INPUT
    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
    }
    //
    #endregion

    private void EnableCharacterMovement(ulong networkObjectId, int currentPlayerIndex)
    {
        if (networkObjectId == _networkObjectId)
        {
            _compensatedPosition = transform.position;

            _isBot = isBotVariable.Value;

            if (_isBot)
            {
                SyncIsBotRpc(networkObjectId);
            }

            _isMovable = true;
        }
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

            monsterTrail.emitting = false;

            await Task.Delay((int)(monsterTrail.time * 1000));

            monsterTrail.gameObject.SetActive(false);

            monsterTrail.emitting = true;
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

        speedBoostTrail.emitting = true;

        _isSpeedBoosting = false;
    }
}
