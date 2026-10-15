using UnityEngine;
[DefaultExecutionOrder(100)]
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
    
    [Header("Direction Switching")]
    [SerializeField] private float switchMargin = 0.15f;
    [Tooltip("Mouse closer than this (world units) to the aim origin is ignored")]
    [SerializeField] private float minMouseDistance = 0.75f;
    [Tooltip("How far past the centerline the mouse must go to flip left/right or up/down")]
    [SerializeField] private float signDeadZone = 0.2f;
    [Tooltip("Point the mouse is measured from. Use the torso/head, not the feet")]
    [SerializeField] private Transform aimOrigin;
    
    [SerializeField] private float maxHeadTilt = 50f;
    [SerializeField] private float headTurnSpeed = 18f;
    private float _smoothTilt;

    private bool _isHorizontal = true;
    public bool IsHorizontal => _isHorizontal;
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
        
        if (aimOrigin == null) aimOrigin = Torso.transform;
    }

    private Quaternion _headRotation;


    private void LateUpdate()
    {
        if (_movementScript.IsDashing) return;

        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                -_mainCamera.transform.position.z));

        Vector2 toMouse = (Vector2)(mousePos - aimOrigin.position);

        // Too close: keep everything exactly as it was
        if (toMouse.magnitude < minMouseDistance) return;

        direction = toMouse.normalized;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float absX = Mathf.Abs(toMouse.x);
        float absY = Mathf.Abs(toMouse.y);

        // Hysteresis on horizontal vs vertical
        if (_isHorizontal && absY > absX * (1f + switchMargin))
            _isHorizontal = false;
        else if (!_isHorizontal && absX > absY * (1f + switchMargin))
            _isHorizontal = true;

        // Hysteresis on the sign (left/right, up/down)
        if (facingRight && toMouse.x < -signDeadZone) facingRight = false;
        else if (!facingRight && toMouse.x > signDeadZone) facingRight = true;

        if (facingUp && toMouse.y < -signDeadZone) facingUp = false;
        else if (!facingUp && toMouse.y > signDeadZone) facingUp = true;
        facingDown = !facingUp;

        bool horizontal = _isHorizontal;
        Vector3 scale = transform.localScale;

        if (horizontal)
        {
            scale.x = facingRight ? 1f : -1f;
            transform.localScale = scale;
            SetFlip(!facingRight);
            _headRenderer.sortingOrder = 2;
            _animator.SetFloat("LookX", 1);
            _animator.SetFloat("LookY", 0);
        }
        else
        {
            scale.x = 1;
            transform.localScale = scale;
            SetFlip(false);
            _headRenderer.sortingOrder = facingUp ? 0 : 2;
            _animator.SetFloat("LookX", 0);
            _animator.SetFloat("LookY", facingUp ? 1 : -1);
        }

        // Tilt measured against |x| so it stays continuous across the centerline
        float tilt = Mathf.Atan2(toMouse.y, Mathf.Abs(toMouse.x)) * Mathf.Rad2Deg;
        tilt = Mathf.Clamp(tilt, -maxHeadTilt, maxHeadTilt);

        float k = 1f - Mathf.Exp(-headTurnSpeed * Time.deltaTime);
        _smoothTilt = Mathf.LerpAngle(_smoothTilt, horizontal ? tilt : 0f, k);

        float headscale = 0.8476f;
        Vector2 hs = Head.transform.localScale;
        hs.x = (horizontal && facingRight) ? headscale : -headscale;
        hs.y = (horizontal && !facingRight) ? -headscale : headscale;
        Head.transform.localScale = hs;

        // Same result as the old facingRight ? angle : -angle, minus the discontinuities
        float z = (horizontal && !facingRight) ? _smoothTilt - 180f : _smoothTilt;
        Head.transform.localRotation = Quaternion.Euler(0, 0, z);
        
        
        
        
    }

    private void SetFlip(bool flip)
    {
        _headRenderer.flipX = flip;
        _torsoRenderer.flipX = flip;
        foreach (var leg in _legRenderers)
            leg.flipX = flip;
    }
}