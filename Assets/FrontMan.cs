using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;
using Netcode.Transports;

public class FrontMan : MonoBehaviour
{
    public static FrontMan FM;
    public Camera MainCam;
    public CanvasAI Canvas;
    public SteamNetworkingTransport Transport;
    public NetworkManager NetworkManager;

    private void Awake()
    {
        //NetworkManager = GetComponent<NetworkManager>();
        //Transport = (SteamNetworkingTransport)NetworkManager.NetworkConfig.NetworkTransport;
        FM = this;
    }
}
