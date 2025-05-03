using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AISpawnConfig
{
    public string AIGameObjectPrefabPath;
    public float FirstSpawnDelay;
    public float SpawnInterval;
    public int NumSpawnEachTime;
}

public class AIManager : MonoBehaviour
{
    List<Coroutine> SpawningCoroutines = new List<Coroutine>();
    public List<AISpawnConfig> Configs = new List<AISpawnConfig>();
    float MinMapBoundX = -15;
    float MaxMapBoundX = 100;
    float MinMapBoundZ = -120;
    float MaxMapBoundZ = 40;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnJoinedRoomEvent += OnJoinedRoom;
        GameManager.OnLeftRoomEvent += OnLeaveRoom;
    }

    void OnJoinedRoom()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            foreach(var Config in Configs)
            {
                SpawningCoroutines.Add(StartCoroutine(SpawnAI(Config)));
            }
        }
    }

    void OnLeaveRoom()
    {
        StopAllCoroutines();
    }

    IEnumerator SpawnAI(AISpawnConfig Config)
    {
        yield return new WaitForSeconds(Config.FirstSpawnDelay);
        while(true)
        {
            if(GameObject.FindGameObjectsWithTag("Creep").Length < 100)
            {
                for (int i = 0; i < Config.NumSpawnEachTime; i++)
                {
                    float RangeX = UnityEngine.Random.Range(MinMapBoundX, MaxMapBoundX);
                    float RangeZ = UnityEngine.Random.Range(MinMapBoundZ, MaxMapBoundZ);
                    Vector3 SpawnPos = new Vector3(RangeX, 0, RangeZ);
                    GameObject AIGO = PhotonNetwork.Instantiate(Config.AIGameObjectPrefabPath, SpawnPos, Quaternion.identity);
                }
            }
            yield return new WaitForSeconds(Config.SpawnInterval);
        }
    }
}
