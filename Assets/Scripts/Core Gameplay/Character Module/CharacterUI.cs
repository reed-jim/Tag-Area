using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CharacterUI : NetworkBehaviour
{
    [SerializeField] private TMP_Text nameText;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        nameText.text = $"Player {GetComponent<NetworkObject>().NetworkObjectId}";

        AssignPlayerNameAsync();
    }

    private async void AssignPlayerNameAsync()
    {
        await Task.Delay(3000);

        nameText.text = $"Player {OwnerClientId}";
    }
}
