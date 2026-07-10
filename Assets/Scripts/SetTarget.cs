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

    private bool _findpath = false;

    public IEnumerator Testfloor()
    {
        target =  GameObject.FindGameObjectWithTag("EndPoint");
        _findpath = false;
        findablePath = true;
        transform.position = oldPosition;
        
        yield return new WaitForSeconds(0.1f);
        _findpath = false;
    }

    void Start()
    {
        target =  GameObject.FindGameObjectWithTag("EndPoint");
        oldPosition = new Vector2(transform.position.x, transform.position.y);
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;
    }

    void Update()
    {
        _agent.isStopped = _findpath;
        target =  GameObject.FindGameObjectWithTag("EndPoint");
        Vector2 newDest = target.transform.position;
        if ((Vector2)_agent.destination != newDest)
        {
            _agent.SetDestination(newDest);
        }
        if (Vector2.Distance(transform.position, target.transform.position) <= 0.1f)
        {
            findablePath = true;
            print("Exit has been found");
            Destroy(gameObject);
        }
        
    }
    
    
}


