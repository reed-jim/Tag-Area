using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class CharacterCameraBinder : NetworkBehaviour
{
    public static event Action<Transform> bindCameraEvent;

    private void Awake()
    {
        BindCameraAsync();
    }

    private async void BindCameraAsync()
    {
        while (!IsOwner)
        {
            await Task.Delay(1000);
        }

        if (IsOwner)
        {
            bindCameraEvent?.Invoke(transform);
        }
    }
}
