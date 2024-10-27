using Unity.Netcode;
using UnityEngine;
using System;
using Sirenix.OdinInspector.Editor;
using JetBrains.Annotations;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
   [SerializeField] private float speed = 3f;

   [CanBeNull] public static event System.Action GameOverEvent;
   private Camera _mainCamera;
   private bool _canCollide  = true;

   private Vector3 _mouseInput;
   private PlayerLength _PlayerLength;
   private readonly ulong[] _targetClientsArray = new ulong[1];

   private void Initialize()
   {
      _mainCamera = Camera.main;
      _PlayerLength = GetComponent<PlayerLength>();
   }

   public override void OnNetworkSpawn()
   {
      base.OnNetworkSpawn();
      Initialize();
   }
   private void Update()
   {
      if (!IsOwner || !Application.isFocused) return;
 
   }

   private void MovePlayerServer()
   {
      _mouseInput.x = Input.mousePosition.x;
      _mouseInput.y = Input.mousePosition.y;
      _mouseInput.z = _mainCamera.nearClipPlane;
      Vector3 mouseWorldCoordinate = _mainCamera.ScreenToWorldPoint(_mouseInput);
      mouseWorldCoordinate.z = 0f;
   }

   [ServerRpc]
   private void MovePlayerServerRpc(Vector3 mouseWorldCoordinates)
   {
      transform.position = Vector3.MoveTowards(transform.position, mouseWorldCoordinates, Time.deltaTime * speed);

      if (mouseWorldCoordinates != transform.position)
      {
         Vector3 targetDirection = mouseWorldCoordinates - transform.position;
         targetDirection.z = 0;
         transform.up = targetDirection;

      }
   }

   private void MovePlayerClient()
   {
      _mouseInput.x = Input.mousePosition.x;
      _mouseInput.y = Input.mousePosition.y;
      _mouseInput.z = _mainCamera.nearClipPlane;
      Vector3 mouseWorldCoordinate = _mainCamera.ScreenToWorldPoint(_mouseInput);
      mouseWorldCoordinate.z = 0f;
      transform.position = Vector3.MoveTowards(transform.position, mouseWorldCoordinate, Time.deltaTime * speed);

      if (mouseWorldCoordinate != transform.position)
      {
         Vector3 targetDirection = mouseWorldCoordinate - transform.position;
         targetDirection.z = 0;
         transform.up = targetDirection;

      }
   }

   [ServerRpc]
   private void DetermineCollisioninnerServerRPC(PlayerData player1, PlayerData player2)
   {
      if (player1.Length > player2.Length)
      {
         WinInformationServerRpc(player1.Id, player2.Id);
      }
      else
      {

      }
   }

   [ServerRpc]
   private void WinInformationServerRpc(ulong winner, ulong loser)
   {
      _targetClientsArray[0] = winner;
      ClientRpcParams clientRpcParams = new ClientRpcParams
      {
         Send = new ClientRpcSendParams
         {
            TargetClientIds = new ulong[] { winner }
         }
      };
      AtePlayerClientRpc(clientRpcParams);

      _targetClientsArray[0] = loser;
      clientRpcParams.Send.TargetClientIds = _targetClientsArray;
      AtePlayerClientRpc(clientRpcParams);
   }
   
   [ClientRpc]
   private void AtePlayerClientRpc(ClientRpcParams clientRpcParams = default)
   {
      if (!IsOwner) return;
      Debug.Log("you  ate player");
   }
   [ClientRpc]
   private void GameOverClientRPC(ClientRpcParams clientRpcParams = default)
   {
      if (!IsOwner) return;
      Debug.Log("you  ate player");
      GameOverEvent?.Invoke();
      NetworkManager.Singleton.Shutdown();
   }

   private IEnumerator CollisionCheckCoroutine()
   {
      _canCollide  = false;
      yield return new WaitForSeconds(0.5f);
      _canCollide  = true;
      StartCoroutine(CollisionCheckCoroutine());
   }
   private void OnCollision2D(Collision2D col)
   {
      Debug.Log("Heead Collision");
      if (col.gameObject.CompareTag("Player")) return;
      if (!IsOwner) return;
      if(!_canCollide)return;

      if (col.gameObject.TryGetComponent(out PlayerLength playerLength0))
      {
         var player1 = new PlayerData()
         {
            Id = OwnerClientId,
            Length = _PlayerLength.length.Value
         };

         var player2 = new PlayerData()
         {
            Id = _PlayerLength.OwnerClientId,
            Length = _PlayerLength.length.Value
         };

         DetermineCollisioninnerServerRPC(player1, player2);
      }
      else if (col.gameObject.TryGetComponent(out Tail tail))
      {
         Debug.Log("Tail Collision");
         WinInformationServerRpc(tail.networkedOwner.GetComponent<PlayerController>().OwnerClientId, OwnerClientId);
      }
   }

   struct PlayerData : INetworkSerializable
   {
      public ulong Id;
      public ushort Length;
      public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
      {
         serializer.SerializeValue(ref Id);
         serializer.SerializeValue(ref Length);
      }
   }
}