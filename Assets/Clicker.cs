using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
public class Clicker : NetworkBehaviour
{
    public TextMeshPro counter;
    int count = 0;

    private void Awake()
    {
        //count.Value = 0;
    }
    private void OnMouseDown()
    {
        CountUpServerRPC();
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void CountUpServerRPC()
    {
        count++;
        counter.text = count + "";
    }
}
