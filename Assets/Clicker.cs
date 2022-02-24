using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
public class Clicker : NetworkBehaviour
{
    public TextMeshPro counter;
    NetworkVariable<int> count;

    private void Awake()
    {
        count.Value = 0;
    }
    private void OnMouseDown()
    {
        CountUpServerRPC();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) print(count.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void CountUpServerRPC()
    {
        count.Value++;
        ChangeCounterClientRPC();
    }

    [ClientRpc]
    public void ChangeCounterClientRPC()
    {
        counter.text = count.Value + "";
    }
}
