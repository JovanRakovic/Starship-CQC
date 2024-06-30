using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Net;
using TMPro;
using System;
using Unity.Netcode.Transports.UTP;

public class MainMenuManager : MonoBehaviour
{
    private enum menuState{
        MAIN,HOST,CONNECT
    };
    private menuState state = menuState.MAIN;

    [SerializeField] private CanvasGroup mainMenuPanel;
    [SerializeField] private CanvasGroup connectPanel;
    [SerializeField] private CanvasGroup hostPanel;

    [SerializeField] private TMP_InputField ipField;
    [SerializeField] private TMP_InputField connectPortField;
    
    [SerializeField] private TMP_InputField hostPortField;

    private void Start()
    {
        ToggleCanvasGroup(mainMenuPanel, true);
        ToggleCanvasGroup(connectPanel, false);
        ToggleCanvasGroup(hostPanel, false);
    }

    public void Exit()
    {
        Application.Quit();
    }
    public void ToConnectMenu()
    {
        state = menuState.CONNECT;
        ToggleCanvasGroup(mainMenuPanel, false);
        ToggleCanvasGroup(connectPanel, true);
    }
    public void Connect()
    {
        string ip = ipField.text;
        foreach(char c in ip)
            if(Char.IsLetter(c))
                ip = Dns.GetHostAddresses(ipField.text)[0].ToString();
        
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ip,ushort.Parse(connectPortField.text));
        NetworkManager.Singleton.StartClient();
    }
    public void ToHostMenu()
    {
        state = menuState.HOST;
        ToggleCanvasGroup(mainMenuPanel, false);
        ToggleCanvasGroup(hostPanel, true);
    }
    public void BeginHost()
    {
        Debug.Log("Start Hosting");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", ushort.Parse(hostPortField.text));
        NetworkManager.Singleton.StartHost();
    }
    public void Back()
    {
        switch(state)
        {
            case menuState.HOST:
                NetworkManager.Singleton.Shutdown();
                state = menuState.MAIN;
                ToggleCanvasGroup(mainMenuPanel, true);
                ToggleCanvasGroup(hostPanel, false);
                break;
            case menuState.CONNECT:
                NetworkManager.Singleton.Shutdown();
                state = menuState.MAIN;
                ToggleCanvasGroup(mainMenuPanel, true);
                ToggleCanvasGroup(connectPanel, false);
                break;
            default:
                NetworkManager.Singleton.Shutdown();
                state = menuState.MAIN;
                ToggleCanvasGroup(mainMenuPanel, true);
                ToggleCanvasGroup(connectPanel, false);
                ToggleCanvasGroup(hostPanel, false);
                break;
        }
    }

    private void ToggleCanvasGroup(CanvasGroup group, bool toggle)
    {
        group.alpha = (toggle)? 1 : 0;
        group.blocksRaycasts = toggle;
        group.interactable = toggle;
    }
}
