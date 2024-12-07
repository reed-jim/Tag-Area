using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneManager : NetworkBehaviour
{
    private void Awake()
    {
        GameNetcodeManager.toSceneEvent += ToScene;
        LobbyManagerUsingRelay.toSceneEvent += ToScene;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        GameNetcodeManager.toSceneEvent -= ToScene;
        LobbyManagerUsingRelay.toSceneEvent -= ToScene;
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
