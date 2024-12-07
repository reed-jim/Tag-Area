using UnityEngine;

public class FollowCharacterCamera : MonoBehaviour
{
    [SerializeField] private Transform _character;

    [Header("CUSTOMIZE")]
    [SerializeField] private Vector3 offset;
    [SerializeField, Range(0, 1)] private float lerpRatio;

    #region PRIVATE FIELD

    #endregion

    private void Awake()
    {
        CharacterCameraBinder.bindCameraEvent += AssignCharacterToFollow;

        offset = transform.position;
    }

    private void OnDestroy()
    {
        CharacterCameraBinder.bindCameraEvent -= AssignCharacterToFollow;
    }

    private void LateUpdate()
    {
        FollowCharacter();
    }

    private void FollowCharacter()
    {
        if (_character != null)
        {
            transform.position = Vector3.Lerp(transform.position, _character.position + offset, lerpRatio);
        }
    }

    private void AssignCharacterToFollow(Transform character)
    {
        _character = character;
    }
}
