using UnityEngine;
using UnityEngine.AI;

public class EnemyActivator : MonoBehaviour
{
    private EnemyAI _enemyAI;
    private ClothesScript _clothesScript;
    private NavMeshAgent _navMeshAgent;
    
    GameObject _player;
    [SerializeField]float detectionRange = 15;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _enemyAI = GetComponent<EnemyAI>();
        _clothesScript = GetComponent<ClothesScript>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        
        _navMeshAgent.isStopped = true;
        _enemyAI.enabled = false;
        _clothesScript.enabled = false;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(_player.transform.position, transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            Activate();
        }


    }

    void Activate()
    {
        if (_navMeshAgent.enabled)
        {
            _navMeshAgent.isStopped = false;
            _enemyAI.enabled = true;
            _clothesScript.enabled = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
