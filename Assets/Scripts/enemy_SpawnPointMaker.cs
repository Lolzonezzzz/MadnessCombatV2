using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;


public class enemy_SpawnPointMaker : MonoBehaviour
{
    [SerializeField] private float room_size_Length = 3;
    [SerializeField] private float room_size_Height = 3;
    
    [Space(30)]
    [SerializeField] private float LengthOffset, HeightOffset;
    [SerializeField] private int amountofspawns;
    [Space(30)]
    [SerializeField] private GameObject spawnpoint;
    [SerializeField] private GameObject LockerChest;
    private List<GameObject>  spawnpoints = new List<GameObject>();
    private BoxCollider2D _boxCollider2D;
    private RoomType _roomType;
    

    private bool hasactivatedspawnpoint = false;
    void OnValidate()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        
        _boxCollider2D.size = new Vector2(room_size_Length, room_size_Height);
        _boxCollider2D.offset = new Vector2(LengthOffset, HeightOffset);
    }

    void OnDrawGizmos()
    {
        switch (_roomType)
        {
            case RoomType.Room_With_Enemies:
                Gizmos.color = Color.red;
                break;
            case RoomType.Room_Without_Enemies:
                Gizmos.color = Color.grey;
                break;
            case RoomType.Treasure_Room:
                Gizmos.color = Color.yellow;
                break;
        }
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(_boxCollider2D.offset, _boxCollider2D.size);
    }

    [Button]
    public void CreateSpawnPoint()
    {
        RemoveSpawnPoint();
        float spawnpointlimitheight = transform.position.y - HeightOffset;
        float spawnpointlimitlength = transform.position.x - LengthOffset;

        amountofspawns = 0;
        _roomType = (RoomType)Random.Range(0, 4);

        if (_roomType == RoomType.Room_With_Enemies)
            amountofspawns = Random.Range(1, 4);
        
        for (int i = 0; i < amountofspawns; i++)
        {
            float StartPointX = spawnpointlimitlength - room_size_Length / 2f;
            float EndPointX = spawnpointlimitlength + room_size_Length / 2f;
            
            float StartPointY = spawnpointlimitheight - room_size_Height / 2f;
            float EndPointY = spawnpointlimitheight + room_size_Height / 2f;
            
            GameObject SpawnPoint = Instantiate(spawnpoint,  new Vector2(Random.Range(StartPointX, EndPointX), Random.Range(StartPointY, EndPointY)), Quaternion.identity);
            SpawnPoint.transform.parent = transform;
            SpawnPoint.tag = "EnemySpawnPoint";
            SpawnPoint.name = "EnemySpawnPoint";
            spawnpoints.Add(SpawnPoint);
        }
        
        
        print("Spawnpoints created");
    }


    void RemoveSpawnPoint()
    {
        for (int i = 0; i < spawnpoints.Count; i++)
        {
            DestroyImmediate(spawnpoints[i]);
        }
        
        spawnpoints.Clear();
    }

    void RemoveLocker()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hasactivatedspawnpoint == false)
        {
            hasactivatedspawnpoint = true;
            ActivateSpawnPoint();
        }
    }

    void ActivateSpawnPoint()
    {
        FloorScript floorScript = GetComponentInParent<FloorScript>();
        for (int i = 0; i < amountofspawns; i++)
        {
            GameObject spawnedenemy = Instantiate(floorScript.enemies[Random.Range(0, floorScript.enemies.Length)], spawnpoints[i].transform.position, Quaternion.identity);
            
        }
    }
}

enum RoomType
{
    Room_With_Enemies,
    Room_Without_Enemies,
    Treasure_Room
}
