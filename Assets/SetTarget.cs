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

    private Vector2 _oldPosition;
    public bool findablePath;

    public IEnumerator testfloor()
    {
        target =  GameObject.FindGameObjectWithTag("EndPoint");
        _agent.isStopped = false;
        findablePath = false;
        transform.position = _oldPosition;
        
        yield return new WaitForSeconds(0.1f);
        _agent.isStopped = true;
    }

    void Start()
    {
        target =  GameObject.FindGameObjectWithTag("EndPoint");
        _oldPosition = new Vector2(transform.position.x, transform.position.y);
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    void Update()
    {

        _agent.SetDestination(target.transform.position);
        if (Vector2.Distance(transform.position, target.transform.position) <= 0.1f)
        {
            findablePath = true;
            print("Exit has been found");
        }
        else
        {
            findablePath = false;
        }
    }
}


