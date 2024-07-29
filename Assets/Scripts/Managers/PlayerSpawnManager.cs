using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawnManager : NetworkBehaviour
{
    [SerializeField] private Transform[] shipPrefabs;
    [SerializeField] private Transform spawnsHolder;

    private void Start() 
    {
        if(!IsServer)
        {
            Destroy(gameObject);
            return;
        }

        SpawnOrdered();
    }

    private void SpawnOrdered()
    {
        for(int i = 0; i < NetworkManager.ConnectedClientsIds.Count; i++)
        {
            Transform spawn = spawnsHolder.GetChild((int)NetworkManager.ConnectedClientsIds[i] % spawnsHolder.childCount);

            Transform g = Instantiate(shipPrefabs[0], spawn.position, spawn.rotation);
            g.GetComponent<NetworkObject>().SpawnWithOwnership(NetworkManager.ConnectedClientsIds[i]);
        }
    }
}
