using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class NetworkSceneManager
{
    public static void LoadSceneNetwork(string sceneName)
    {
        SceneEventProgressStatus status = NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        if(status != SceneEventProgressStatus.Started)
            Debug.LogWarning($"Failed to load {sceneName} " + $"with a {nameof(SceneEventProgressStatus)}: {status}");
    }

    public static void LoadScene(string sceneName)
    {
        LoadScene(sceneName);
    }
}
