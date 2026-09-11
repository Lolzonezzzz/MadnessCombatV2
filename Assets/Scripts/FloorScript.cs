using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class FloorScript : MonoBehaviour
{
    [Header("Settings")] private GameObject[] _dungeonWalls;
    [SerializeField] private GameObject floorChecker, spawnPoint;
    [SerializeField] private SetTarget target;


    [Space(10), Header("Walls")] [SerializeField]
    private NavMeshSurface navMeshSurface;

    [SerializeField] private GameObject horizontalWallPrefab, horizontalWallWithDoorPrefab;
    [SerializeField] private GameObject verticalWallPrefab, verticalWallWithDoorPrefab;

    private GameObject _floorcheck;
    private float _canFindLocation;
    private NavMeshAgent _agent;

    private List<GameObject> _allWalls = new List<GameObject>();


    [Space(10), Header("Player and enemies")] [SerializeField]
    private GameObject player;
    
    [Space(4)]
    [SerializeField]private GameObject[] enemies;
    // Start is called before the first frame update
    void Start()
    {
        _dungeonWalls = GameObject.FindGameObjectsWithTag("TurnOffWalls");
        foreach (GameObject d in _dungeonWalls)
        {
            int randomNumber = Random.Range(0, 3);
            GameObject wall = null;


            if (randomNumber == 0)
                wall = Instantiate(d.layer == LayerMask.NameToLayer("VeriticalWall")
                        ? verticalWallWithDoorPrefab
                        : horizontalWallWithDoorPrefab
                    , d.transform.position, d.transform.rotation, GameObject.Find("WallParent").transform);
            else
                wall = Instantiate(d.layer == LayerMask.NameToLayer("VeriticalWall")
                        ? verticalWallPrefab
                        : horizontalWallPrefab
                    , d.transform.position, d.transform.rotation, GameObject.Find("WallParent").transform);

            _allWalls.Add(wall);
        }

        navMeshSurface.BuildNavMesh();
        _floorcheck = Instantiate(floorChecker, spawnPoint.transform.position, spawnPoint.transform.rotation);
        _agent = _floorcheck.GetComponent<NavMeshAgent>();
        CheckforwallNaviagator checkforwallNaviagator = _floorcheck.GetComponentInChildren<CheckforwallNaviagator>();
        target = _floorcheck.GetComponent<SetTarget>();
        checkforwallNaviagator.floorcheck = this;

    }


    private bool _isRebuilding;



    public void CreatePlayer()
    {
        GameObject createthePlayer = Instantiate(player, spawnPoint.transform.position,  Quaternion.identity);
        MouseWeightedCameraScript mouseWeightedCameraScript = Camera.main.GetComponent<MouseWeightedCameraScript>();
        mouseWeightedCameraScript.enabled = true;
        print("player has been spawned");
    }

    public IEnumerator GetWalls()
    {
        if (_isRebuilding) yield break;
        _isRebuilding = true;

        Destroy(_floorcheck);
        yield return null;

        _canFindLocation = 0;
        for (int i = 0; i < _allWalls.Count; i++)
        {
            Destroy(_allWalls[i]);
        }

        _allWalls.Clear();
        foreach (GameObject d in _dungeonWalls)
        {
            int randomNumber = Random.Range(0, 2);
            GameObject wall = null;


            if (randomNumber == 0)
                wall = Instantiate(d.layer == LayerMask.NameToLayer("VeriticalWall")
                        ? verticalWallWithDoorPrefab
                        : horizontalWallWithDoorPrefab
                    , d.transform.position, d.transform.rotation, GameObject.Find("WallParent").transform);
            else
                wall = Instantiate(d.layer == LayerMask.NameToLayer("VeriticalWall")
                        ? verticalWallPrefab
                        : horizontalWallPrefab
                    , d.transform.position, d.transform.rotation, GameObject.Find("WallParent").transform);

            _allWalls.Add(wall);
        }

        yield return null;
        navMeshSurface.BuildNavMesh();
        yield return null;
        Destroy(_floorcheck);
        _floorcheck = Instantiate(floorChecker, spawnPoint.transform.position, spawnPoint.transform.rotation);
        _agent = _floorcheck.GetComponent<NavMeshAgent>();
        _agent.Warp(spawnPoint.transform.position);
        CheckforwallNaviagator checkforwallNaviagator =
            _floorcheck.GetComponentInChildren<CheckforwallNaviagator>();
        target = _floorcheck.GetComponent<SetTarget>();
        checkforwallNaviagator.floorcheck = this;
        _isRebuilding = false;
    }
}
