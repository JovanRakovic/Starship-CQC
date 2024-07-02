using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Net;
using TMPro;
using System;
using Unity.Netcode.Transports.UTP;
using Unity.Collections;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    private enum menuState{
        MAIN,HOST,CONNECT,LOBBYHOST,LOBBYCLIENT
    };
    private menuState state = menuState.MAIN;

    [SerializeField] private CanvasGroup mainMenuPanel;
    [SerializeField] private CanvasGroup connectPanel;
    [SerializeField] private CanvasGroup hostPanel;
    [SerializeField] private CanvasGroup lobbyPanel;

    [SerializeField] private TMP_InputField ipField;
    [SerializeField] private TMP_InputField connectPortField;
    [SerializeField] private TMP_InputField hostPortField;
    
    [SerializeField] private TMP_InputField usernameField;

    [Header("Lobby")]
    [SerializeField] public Transform playerList;
    [SerializeField] public Transform playerCardPrefab;
    
    [SerializeField] private Transform lobbyRefresherPrefab;
    
    [SerializeField] private Transform lobbyRefresherInstance;


    private void Start()
    {
        ToggleCanvasGroup(mainMenuPanel, true);
        ToggleCanvasGroup(connectPanel, false);
        ToggleCanvasGroup(hostPanel, false);
        ToggleCanvasGroup(lobbyPanel, false);

        PlayerHandler.getName += (PlayerHandler handler) => {
            handler.SetUsername(usernameField.text);
        };
        
        NetworkManager.Singleton.OnClientConnectedCallback += UpdateList;
        NetworkManager.Singleton.OnClientDisconnectCallback += UpdateList;
        NetworkManager.Singleton.OnClientDisconnectCallback += BackOnServerShutdown;
    }
    private void UpdateList(ulong id)
    {
        StartCoroutine(UpdateListCorutine());
    }

    IEnumerator UpdateListCorutine()
    {
        yield return new WaitForSeconds(.15f);

        bool first = true;
        if(NetworkManager.Singleton.IsServer)
        {
            LobbyNetworkScript script = lobbyRefresherInstance.GetComponent<LobbyNetworkScript>();

            foreach(KeyValuePair<ulong, NetworkClient> entry in NetworkManager.Singleton.ConnectedClients)
            {
                script.RefreshLobbyListRpc(first, entry.Key, entry.Value.PlayerObject.GetComponent<PlayerHandler>().GetUsername());
                first = false;
            }
        }
    }

    
    private void BackOnServerShutdown(ulong t)
    {
        if(t != NetworkManager.Singleton.LocalClientId)
            return;
        
        if(state == menuState.LOBBYHOST || state == menuState.LOBBYCLIENT)
            Back();
    }
    public void Exit()
    {
        NetworkManager.Singleton.Shutdown();
        Application.Quit();
    }
    public void ToConnectMenu()
    {
        SwitchPanels(false, menuState.CONNECT, mainMenuPanel, connectPanel);
    }
    public void ConnectLobby()
    {
        if(lobbyRefresherInstance == null)
            lobbyRefresherInstance = Instantiate(lobbyRefresherPrefab);
        string ip = ipField.text;
        foreach(char c in ip)
            if(Char.IsLetter(c))
                ip = Dns.GetHostAddresses(ipField.text)[0].ToString();
        
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ip,ushort.Parse(connectPortField.text));
        NetworkManager.Singleton.StartClient();

        SwitchPanels(false, menuState.LOBBYCLIENT, connectPanel, lobbyPanel);
    }
    public void ToHostMenu()
    {
        SwitchPanels(false, menuState.HOST, mainMenuPanel, hostPanel);
    }
    public void HostLobby()
    {
        if(lobbyRefresherInstance == null)
            lobbyRefresherInstance = Instantiate(lobbyRefresherPrefab);
        SwitchPanels(false, menuState.LOBBYHOST, hostPanel, lobbyPanel);

        IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipEntry.AddressList[1].ToString(), ushort.Parse(hostPortField.text));
        NetworkManager.Singleton.StartHost();
    }
    public void Back()
    {
        switch(state)
        {
            case menuState.HOST:
                SwitchPanels(false, menuState.MAIN, hostPanel, mainMenuPanel);
                break;
            case menuState.CONNECT:
                SwitchPanels(false, menuState.MAIN, connectPanel, mainMenuPanel);
                break;
            case menuState.LOBBYHOST:
                SwitchPanels(true, menuState.HOST, lobbyPanel, hostPanel);
                break;
             case menuState.LOBBYCLIENT:
                SwitchPanels(true, menuState.CONNECT, lobbyPanel, connectPanel);
                break;
            default:
                NetworkManager.Singleton.Shutdown();
                state = menuState.MAIN;
                ToggleCanvasGroup(mainMenuPanel, true);
                ToggleCanvasGroup(connectPanel, false);
                ToggleCanvasGroup(hostPanel, false);
                ToggleCanvasGroup(lobbyPanel, false);
                break;
        }
    }

    private void SwitchPanels(bool shutdown, menuState newState, CanvasGroup from, CanvasGroup to)
    {
        state = newState;
        if(shutdown) NetworkManager.Singleton.Shutdown();
        ToggleCanvasGroup(to, true);
        ToggleCanvasGroup(from, false);
    }
    private void ToggleCanvasGroup(CanvasGroup group, bool toggle)
    {
        group.alpha = (toggle)? 1 : 0;
        group.blocksRaycasts = toggle;
        group.interactable = toggle;
    }
}
