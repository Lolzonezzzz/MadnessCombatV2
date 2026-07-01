using System.Collections;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private float respawnDelay = 3f;
    private Vector3 _spawnPoint;
    private HealthScript _health;
    private PlayerMovement _movement;
    private Animator _animator;
    private bool _isDead;

    void Start()
    {
        _spawnPoint = transform.position;
        _health = GetComponent<HealthScript>();
        _movement = GetComponent<PlayerMovement>();
        _animator = GetComponent<Animator>();
        
    }

    void Update()
    {
        if (_health.brainhealth <= 0 || _health.headhealth <= 0 || _health.bodyhealth <= 0 || _health.hearthealth <= 0 || _health.lungshealth <= 0)
        {
            if (!_isDead)
            {
                _health.OnDeath();
                _isDead = true;
                StartCoroutine(Respawn());
            }
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);

        // Reset position
        transform.position = _spawnPoint;

        // Reset health
        _health.brainhealth = _health.Brainhealth;
        _health.headhealth = _health.Headhealth;
        _health.bodyhealth = _health.Bodyhealth;
        _health.hearthealth = _health.Hearthealth;
        _health.lungshealth = _health.Lungshealth;

        // Reset ragdoll
        foreach (var rb in GetComponentsInChildren<Rigidbody2D>())
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.drag = 0f;
            rb.angularDrag = 0.05f;
        }

        foreach (var joint in GetComponentsInChildren<HingeJoint2D>())
        {
            joint.enabled = false;
        }

        foreach (var col in GetComponentsInChildren<Collider2D>())
        {
            col.isTrigger = true;
        }

        // Re-enable components
        _animator.enabled = true;
        _movement.enabled = true;
        _health.enabled = true;
        _health.DisableDeath();
        gameObject.GetComponent<PlayerDirection>().enabled = true;
        gameObject.GetComponent<ClothesScript>().enabled = true;
        gameObject.GetComponent<Collider2D>().isTrigger = false;
        gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        

        _isDead = false;
    }
}
