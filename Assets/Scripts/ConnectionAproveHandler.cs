using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEditor.Compilation;

public class ConnectionAproveHandler : MonoBehaviour
{
    private const int MaxPlayers = 10;

    private void  Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request , NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("Connection Approval");
        response.Approved = true;
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;
        if(NetworkManager.Singleton.ConnectedClients.Count >= MaxPlayers)
        {
           response.Approved = false;
           response.Reason = "Server is Full";
        }

        response.Pending = false;
    }
}
