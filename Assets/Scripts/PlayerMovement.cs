using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
    public MovementDirection dashDirection;
    private Animator _animator;
    private PlayerDirection _playerDirection;
    
    private playSound _playSound;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        _animator = GetComponent<Animator>();
        _playerDirection = GetComponent<PlayerDirection>();
        _playSound =  GetComponent<playSound>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDashing || EventSystem.current.IsPointerOverGameObject())
            return;
        
        _inputX = Input.GetAxisRaw("Horizontal");
        _inputY = Input.GetAxisRaw("Vertical");



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
                _animator.SetBool("Moving", true);
                direction = MovementDirection.Right;
                break;
            case(1, 0):
                _animator.SetBool("Moving", true);
                direction = MovementDirection.Left;
                break;
            case(0,-1):
                _animator.SetBool("Moving", true);
                direction = MovementDirection.Down;
                break;
            case(0, 1):
                _animator.SetBool("Moving", true);
                direction = MovementDirection.Up;
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
                _animator.SetBool("Moving", false);
                break;
        }

        float currentSpeed = speed * (_isSprinting ? sprintMultiplier : 1f);
        Vector2 input = new Vector2(_inputX, _inputY).normalized;
        
        _rb.velocity = input * currentSpeed;
        
        if (_inputX != 0 || _inputY != 0)
        {
            if (_playSound.canPlaySound)
            {
                _playSound.canPlaySound = false;
                StartCoroutine(_playSound.PLaySound(1 - _playSound.index, _isSprinting ? 0.1f : 0.3f));
                
            }
        }


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
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0f;

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

    private void LateUpdate()
    {
        if (_isDashing)
        {
            
            switch (dashDirection)
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
