using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Unity.Netcode;
using UnityEngine;

public class CharacterMaterialPropertyBlock : NetworkBehaviour
{
    [Header("CUSTOMIZE")]
    [SerializeField] private string colorReference;
    [SerializeField] private float transitionDuration;

    #region PRIVATE FIELD
    private ulong _networkObjectId;
    [SerializeField] private Renderer _renderer;
    private MaterialPropertyBlock _propertyBlock;
    private bool _isColorEffectActive;
    #endregion

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _networkObjectId = GetComponent<NetworkObject>().NetworkObjectId;
    }

    private void Awake()
    {
        CharacterFactionManager.changeCharacterFactionEvent += ColorEffectOnTag;
        CharacterCollision.changeCharacterFactionEvent += ColorEffectOnTag;

        Init();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        CharacterFactionManager.changeCharacterFactionEvent += ColorEffectOnTag;
        CharacterCollision.changeCharacterFactionEvent += ColorEffectOnTag;
    }

    private void Init()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();
        }

        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
        }
    }

    public void SetColor(Color color)
    {
        Init();

        _propertyBlock.SetColor(colorReference, color);

        _renderer.SetPropertyBlock(_propertyBlock);
    }

    private void ColorEffectOnTag(ulong networkObjectId, CharacterFaction characterFaction)
    {
        if (networkObjectId != _networkObjectId)
        {
            return;
        }

        if (_isColorEffectActive)
        {
            return;
        }

        Color startColor;
        Color endColor;

        if (characterFaction == CharacterFaction.Monster)
        {
            startColor = Color.white;
            endColor = Color.red;
        }
        else
        {
            startColor = Color.red;
            endColor = Color.white;
        }

        if (networkObjectId == _networkObjectId)
        {
            float hdrIntensity = 1;

            Tween.Custom(startColor, endColor, duration: transitionDuration, onValueChange: newVal => SetColor(hdrIntensity * newVal))
            .OnComplete(() =>
            {
                _isColorEffectActive = false;
            });

            _isColorEffectActive = true;
        }
    }
}
