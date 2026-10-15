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

    // Original limb pose, so the ragdoll doesn't stay scattered after respawn.
    private Transform[] _childTransforms;
    private Vector3[] _childLocalPositions;
    private Quaternion[] _childLocalRotations;

    void Start()
    {
        _spawnPoint = transform.position;
        _health = GetComponent<HealthScript>();
        _movement = GetComponent<PlayerMovement>();
        _animator = GetComponent<Animator>();

        // Snapshot the assembled pose from the prefab.
        _childTransforms = GetComponentsInChildren<Transform>(true);
        _childLocalPositions = new Vector3[_childTransforms.Length];
        _childLocalRotations = new Quaternion[_childTransforms.Length];
        for (int i = 0; i < _childTransforms.Length; i++)
        {
            _childLocalPositions[i] = _childTransforms[i].localPosition;
            _childLocalRotations[i] = _childTransforms[i].localRotation;
        }
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

        // Reset position + rotation of root
        transform.position = _spawnPoint;
        transform.rotation = Quaternion.identity;

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

        // Re-assemble limbs where the ragdoll flung them.
        // Without this, hands/shoes stay floating detached after every death.
        if (_childTransforms != null)
        {
            for (int i = 0; i < _childTransforms.Length; i++)
            {
                if (_childTransforms[i] == null) continue;
                _childTransforms[i].localPosition = _childLocalPositions[i];
                _childTransforms[i].localRotation = _childLocalRotations[i];
            }
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
