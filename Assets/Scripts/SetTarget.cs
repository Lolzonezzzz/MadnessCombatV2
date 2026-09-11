using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class SetTarget : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private NavMeshAgent _agent;
    public FloorScript floorScript;

    public Vector2 oldPosition;
    public bool findablePath;
    
    private Collider2D _collider;
    private Coroutine _testCoroutine;



    void Start()
    {
        target =  GameObject.FindGameObjectWithTag("EndPoint");
        oldPosition = new Vector2(transform.position.x, transform.position.y);
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        Vector3 pos = transform.position;
        floorScript = GameObject.FindGameObjectWithTag("FloorChecker").GetComponent<FloorScript>();
        pos.z = 0;
        transform.position = pos;
        _agent.SetDestination(target.transform.position);
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, target.transform.position) <= 0.1f)
        {
            findablePath = true;
            print("Exit has been found");
            floorScript.CreatePlayer();
            
            
            Destroy(gameObject); // Destroy game object once everything has been done.

        }
        
    }
    
    
}


