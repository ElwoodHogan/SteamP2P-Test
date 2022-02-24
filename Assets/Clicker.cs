using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Sirenix.OdinInspector;
public class Clicker : NetworkBehaviour
{
    public TextMeshPro counter;
    public NetworkVariable<int> count = new NetworkVariable<int>(0);
    public bool printVal = false;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (x)=> counter.text = count.Value + "";
    }

    private void OnMouseDown()
    {
        CountUpServerRPC();
        ChangeCounterClientRPC();
    }

    private void Update()
    {
        if (printVal)
        {
            print(count.Value);
            printVal = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CountUpServerRPC()
    {
        count.Value++;
        //ChangeCounterClientRPC();
    }

    [ClientRpc]
    public void ChangeCounterClientRPC()
    {
        counter.text = count.Value + "";
    }
}
