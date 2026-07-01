using UnityEngine;

public class ThrownGun : MonoBehaviour
{
    public float damage = 50f;
    public float lifetime = 50f;
    private Rigidbody2D _rb;
    private bool _thrown = true;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (_thrown)
        {
            _rb.angularVelocity = 720f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall") && _rb != null)
        {
            _thrown = false;
            _rb.velocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }
}
