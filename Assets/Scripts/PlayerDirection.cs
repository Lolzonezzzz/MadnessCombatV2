using UnityEngine;
public class PlayerDirection : MonoBehaviour
{
    public float angle;
    public Vector2 direction;
    public bool facingRight, facingUp, facingDown;
    private PlayerMovement _movementScript;
    private Animator _animator;
    private Camera _mainCamera;
    [Header("Body Parts")]
    [SerializeField] private GameObject Head;
    [SerializeField] private GameObject Torso;
    [SerializeField] private GameObject[] Legs;
    private SpriteRenderer _headRenderer;
    private SpriteRenderer _torsoRenderer;
    private SpriteRenderer[] _legRenderers;

    void Start()
    {
        _movementScript = GetComponent<PlayerMovement>();
        _animator = GetComponent<Animator>();
        _mainCamera = Camera.main;
        _headRenderer = Head.GetComponent<SpriteRenderer>();
        _torsoRenderer = Torso.GetComponent<SpriteRenderer>();
        _legRenderers = new SpriteRenderer[Legs.Length];
        for (int i = 0; i < Legs.Length; i++)
            _legRenderers[i] = Legs[i].GetComponent<SpriteRenderer>();
    }

    private Quaternion _headRotation;

    void LateUpdate()
    {
        Head.transform.localRotation = _headRotation;
    }

    private void Update()
    {
        if (_movementScript.IsDashing) return;

        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                        -_mainCamera.transform.position.z)

        );
        direction = (mousePos - transform.position).normalized;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        bool horizontal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
        Vector3 scale = transform.localScale;

        if (horizontal)
        {
            facingRight = direction.x > 0;
            scale.x = facingRight ? 1f : -1f;  
            transform.localScale = scale;
            SetFlip(!facingRight);
            _headRenderer.sortingOrder = 2;
            _animator.SetFloat("LookX", 1);
            _animator.SetFloat("LookY", 0);
        }
        else
        {
            facingUp = direction.y > 0;
            facingDown = direction.y < 0;
            scale.x = 1;
            transform.localScale = scale;
            SetFlip(false);
            _headRenderer.sortingOrder = facingUp ? 0 : 2;
            _animator.SetFloat("LookX", 0);
            _animator.SetFloat("LookY", facingUp ? 1 : -1);
        }

        float headscale = 0.8476f;
        Vector2 vector2 = Head.transform.localScale;
        vector2.x = (horizontal && facingRight ? headscale : -headscale);
        vector2.y = (horizontal && !facingRight ? -headscale : headscale);
        Head.transform.localScale = vector2;
        _headRotation = horizontal
            ? Quaternion.Euler(0, 0, facingRight ? angle : -angle)
            : Quaternion.identity;
    }

    private void SetFlip(bool flip)
    {
        _headRenderer.flipX = flip;
        _torsoRenderer.flipX = flip;
        foreach (var leg in _legRenderers)
            leg.flipX = flip;
    }
}