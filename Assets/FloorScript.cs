using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.Serialization;

public class FloorScript : MonoBehaviour
{
    [Header("Settings")]
    private GameObject[] _dungeonWalls;
    [SerializeField] private GameObject floorChecker, spawnPoint;
    [SerializeField] private SetTarget target;
    
    
    [Space(10), Header("Walls")]
    [SerializeField]private NavMeshSurface navMeshSurface;
    [SerializeField]private GameObject horizontalWallPrefab, horizontalWallWithDoorPrefab;
    [SerializeField]private GameObject verticalWallPrefab, verticalWallWithDoorPrefab;
    
    private GameObject _floorcheck;
    private float _canFindLocation;
    
    private List<GameObject> _allWalls = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        _floorcheck = Instantiate(floorChecker, spawnPoint.transform.position, spawnPoint.transform.rotation);
        target =  _floorcheck.GetComponent<SetTarget>();
        _dungeonWalls = GameObject.FindGameObjectsWithTag("TurnOffWalls");
        foreach (GameObject d in _dungeonWalls)
        {
            int randomNumber = Random.Range(0, 3);
            GameObject wall = null;
            
            
            if (randomNumber == 0)
                wall = Instantiate(d.layer == LayerMask.NameToLayer("VeriticalWall") ? verticalWallWithDoorPrefab : horizontalWallWithDoorPrefab
                    , d.transform.position, d.transform.rotation, GameObject.Find("WallParent").transform);
            else
                wall = Instantiate(d.layer == LayerMask.NameToLayer("VeriticalWall") ? verticalWallPrefab : horizontalWallPrefab
                    , d.transform.position, d.transform.rotation, GameObject.Find("WallParent").transform);
                
            _allWalls.Add(wall);
        }
        
        navMeshSurface.BuildNavMesh();
    }
    
    
    IEnumerator GetWalls()
    {
        Destroy(_floorcheck);


        
        for (int i = 0; i < _allWalls.Count; i++)
        {
            Destroy(_allWalls[i]);
        }
        _allWalls.Clear();
        
        foreach (GameObject d in _dungeonWalls)
        {
            int randomNumber = Random.Range(0, 3);
            
            GameObject wall = null;
            
            if (randomNumber == 0)
                wall = Instantiate(d.layer == LayerMask.NameToLayer("VeriticalWall") ? verticalWallWithDoorPrefab : horizontalWallWithDoorPrefab
                    , d.transform.position, d.transform.rotation, GameObject.Find("WallParent").transform);
            else
                wall = Instantiate(d.layer == LayerMask.NameToLayer("VeriticalWall") ? verticalWallPrefab : horizontalWallPrefab,
                    d.transform.position, d.transform.rotation, GameObject.Find("WallParent").transform);
                
            _allWalls.Add(wall);
            
        }
        
        yield return new WaitForSeconds(0.1f);
        _floorcheck = Instantiate(floorChecker, spawnPoint.transform.position, spawnPoint.transform.rotation);
        target =  _floorcheck.GetComponent<SetTarget>();
        navMeshSurface.BuildNavMesh();
        StartCoroutine(target.testfloor());
    }


    void Update()
    {
        _canFindLocation += Time.deltaTime;
        if (_canFindLocation > 9 && target.findablePath != true) // no path found remaking
        {
            _canFindLocation = 0;
            StartCoroutine(GetWalls());
            print("Exit not found remaking room");
            
        }
    }
    
}
