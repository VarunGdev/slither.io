using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.BossRoom.Infrastructure;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField]private GameObject prefab;
    private const int MaxPrefabCount = 50;
    private WaitForSeconds _waitForSeconds = new WaitForSeconds(2f);

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SpawnFoodStart;
    }

    private void SpawnFoodStart()
    {
        NetworkManager.Singleton.OnServerStarted -= SpawnFoodStart;
        NetworkObjectPool.Singleton.InitializePool();
        for(int i = 0;i<30; ++i)
        {
            SpawnFood();
        }
        
        StartCoroutine(SpawnOverTime());
    }

    private void SpawnFood()
    {
        NetworkObject obj = NetworkObjectPool.Singleton.GetNetworkObject(prefab,GetRandomPositionOnMap(),Quaternion.identity);
        obj.GetComponent<FoodScript>().prefab = prefab;
        if(!obj.IsSpawned)obj.Spawn(true);
    }

    private Vector3 GetRandomPositionOnMap()
    {
        return new Vector3(Random.Range(-9f,9f),Random.Range(-5f,5f),0f);
    }

    private IEnumerator SpawnOverTime()
    {
        while(NetworkManager.Singleton.ConnectedClients.Count > 0)
        {
            yield return _waitForSeconds;
            if(NetworkObjectPool.Singleton.GetCurrentPrefabCount(prefab) < MaxPrefabCount)
            SpawnFood();
        }
    }
}
