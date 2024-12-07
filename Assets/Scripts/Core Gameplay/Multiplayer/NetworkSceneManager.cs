using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneManager : NetworkBehaviour
{
    private void Awake()
    {
        LobbyManager.toSceneEvent += ToScene;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        LobbyManager.toSceneEvent -= ToScene;
    }

    private void ToScene(string sceneName)
    {
        if (IsServer && !string.IsNullOrEmpty(sceneName))
        {
            var status = NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

            if (status != SceneEventProgressStatus.Started)
            {

            }
        }
    }
}
