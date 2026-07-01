using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField]private float speed = 5;
    private float _inputX, _inputY;
    
    [SerializeField]private float sprintMultiplier = 1.2f;
    private bool _canSprint = true;
    private bool _isSprinting;
    
    [Space(12)]
    [Header("Dash Settings")]
    [SerializeField]private float dashSpeed = 15;

    private bool _isDashing;
    public bool IsDashing => _isDashing;
    private bool _canDash = true;
    
    public MovementState state;
    public MovementDirection direction;
    private Animator _animator;
    private PlayerDirection _playerDirection;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        _animator = GetComponent<Animator>();
        _playerDirection = GetComponent<PlayerDirection>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDashing)
            return;
        
        _rb.angularVelocity = 0f;
        _inputX = Input.GetAxisRaw("Horizontal");
        _inputY = Input.GetAxisRaw("Vertical");
        
        Vector2 input = new Vector2(_inputX, _inputY);
        if (input.sqrMagnitude > 1)
            input.Normalize();



        if (direction == MovementDirection.Down || direction == MovementDirection.Up)
        {
            _animator.SetFloat("XInput", 0f);
            _animator.SetFloat("Yinput", _playerDirection.facingUp ? -_inputY : _inputY);
        }
        else
        {
            _animator.SetFloat("XInput", _playerDirection.facingRight ? -_inputX : _inputX);
        }

        

        switch (_inputX, _inputY)
        {
            
            case(-1, 0):
                direction = MovementDirection.Left;
                _animator.SetBool("Moving", true);
                break;
            case(1, 0):
                direction = MovementDirection.Right;
                _animator.SetBool("Moving", true);
                break;
            case(0,-1):
                direction = MovementDirection.Down;
                _animator.SetBool("Moving", true);
                break;
            case(0, 1):
                direction = MovementDirection.Up;
                _animator.SetBool("Moving", true);
                break;
            case (-1, 1):
                _animator.SetBool("Moving", true);
                break;
            case (1, -1):
                _animator.SetBool("Moving", true);
                break;
            case (-1, -1):
                _animator.SetBool("Moving", true);
                break;
            case (1, 1):
                _animator.SetBool("Moving", true);
                break;
            default:
                direction = MovementDirection.Right;
                state = MovementState.Idle;
                _animator.SetBool("Moving", false);
                break;
        }

        float currentSpeed = speed * (_isSprinting ? sprintMultiplier : 1f);
        
        _rb.velocity = input * currentSpeed;

        if (Input.GetKeyDown(KeyCode.LeftShift) && _canSprint)
        {
           _isSprinting = !_isSprinting;
           _animator.SetBool("IsRunning",_isSprinting);
        }

        if (Input.GetKeyDown(KeyCode.Q) && _canDash)
        {
            StartCoroutine(Dash());
            
        }
        

        float target = _rb.velocity.magnitude;
        float current = _animator.GetFloat("Speed");

        _animator.SetFloat("Speed", Mathf.Lerp(current, target, 0.15f));
 
    }

    IEnumerator Dash()
    {
        _isDashing = true;
        _canDash = false;
        HealthScript health = gameObject.GetComponentInChildren<HealthScript>();
        health.dashingInvincibility = false;
        
        yield return new WaitForSeconds(0.7f);
        _rb.velocity = Vector2.zero;        

        _isDashing = false;
        AimAndShoot aim = gameObject.GetComponentInChildren<AimAndShoot>();
        aim.unequipHands();
        health.dashingInvincibility = true;

        yield return new WaitForSeconds(0.9f);
        _canDash = true;
    }

    public enum MovementState
    {
        Idle,
        Walking,
        Running,
        Dashing
    }

    public enum MovementDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    private void FixedUpdate()
    {
        if (_isDashing)
        {
            
            switch (direction)
            {
                case MovementDirection.Left:
                    _rb.velocity = new Vector2(-dashSpeed, 0f);
                    _animator.Play("Rolling Left");
                    break;
                case MovementDirection.Right:
                    _rb.velocity = new Vector2(dashSpeed, 0f);
                    _animator.Play("Rolling Left"); 
                    break;
                case MovementDirection.Up:
                    _rb.velocity = new Vector2(0f, dashSpeed);
                    _animator.Play("Rolling Up");
                    break;
                case MovementDirection.Down:
                    _rb.velocity = new Vector2(0f, -dashSpeed);
                    _animator.Play("Rolling Up");
                    break;
            }
        }
    }
}
