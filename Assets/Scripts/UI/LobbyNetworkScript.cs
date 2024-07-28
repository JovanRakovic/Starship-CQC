using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class LobbyNetworkScript : NetworkBehaviour
{
    int currentPlayer = 0;
    [Rpc(SendTo.ClientsAndHost)]
    public void RefreshLobbyListRpc(bool first, ulong id, FixedString32Bytes username)
    { 
        MainMenuManager m = GameObject.FindObjectOfType<MainMenuManager>();
        Transform playerList = m.playerList;
        Transform playerCardPrefab = m.playerCardPrefab;
        if(first)
        {
            for(int i = playerList.childCount - 1; i >= 0; i--)
                Destroy(playerList.GetChild(i).gameObject);
            currentPlayer = 0;
        }

        Transform card = Instantiate(playerCardPrefab, playerList);
        PlayerCard p = card.GetComponent<PlayerCard>();
        p.clientID = id; 
        p.SetUsernameText(username.ToString());
        card.localPosition = new Vector3(0, (currentPlayer + 1) * card.GetComponent<RectTransform>().rect.height * 1.1f, 0);
        currentPlayer--;
    }
}
