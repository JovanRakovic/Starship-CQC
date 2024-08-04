using Unity.Netcode;
using UnityEngine;

public class NetworkManagerSingular : NetworkManager
{
    private void Awake()
    {
        if(GameObject.FindObjectsByType<NetworkManagerSingular>(FindObjectsSortMode.None).Length > 1)
            Destroy(gameObject);
    }
}
