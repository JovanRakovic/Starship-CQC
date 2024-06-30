using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneManager : NetworkBehaviour
{   
    public void LoadScene(string sceneName)
    {
        if(!IsServer)
            return;
        
        SceneEventProgressStatus status = NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        if(status != SceneEventProgressStatus.Started)
            Debug.LogWarning($"Failed to load {sceneName} " + $"with a {nameof(SceneEventProgressStatus)}: {status}");
    }
}
