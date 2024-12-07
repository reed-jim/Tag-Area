using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CharacterUI : NetworkBehaviour
{
    [SerializeField] private TMP_Text nameText;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        nameText.text = $"{GetComponent<NetworkObject>().NetworkObjectId}";
    }
}
