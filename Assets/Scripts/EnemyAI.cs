using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]private float speed;
    [SerializeField]private AIType aiType;

    
    private Transform _target;
    private NavMeshAgent _agent;
    private float _runspeed = 4;
    public float angle;
    [HideInInspector]public Vector2 direction; 

    [ShowIf("aiType", AIType.Rusher)] [SerializeField]
    private bool startAttacking;
    
    [ShowIf("aiType", AIType.Rusher)] [SerializeField]
    private float attackCooldown;
    
    [ShowIf("aiType", AIType.Shooter)][SerializeField]
    private float headshotpreference;// how likely to shoot a headshot
    
    [ShowIf("aiType", AIType.Shooter)]
    public ShootingType shootingType;
    
    [ShowIf("aiType", AIType.Shooter)]
    private AimAndShoot _gun;

    [ShowIf("aiType", AIType.Shooter)]
    [SerializeField]private float fireTimer;
    
    [ShowIf("aiType", AIType.Shooter)]
    [SerializeField]private float distanceToDetectPlayer = 15;
    
    [ShowIf("aiType", AIType.Shooter)]
    [SerializeField]private ItemData[] itemData;

    [ShowIf("aiType", AIType.Shooter)] [SerializeField]
    private GameObject[] playerBodyPart;
    
    [ShowIf("aiType", AIType.Flanker)] [SerializeField]
    private float timetoAttack;
    
    [ShowIf("aiType", AIType.Flanker)] [SerializeField]
    private float dashdistance;
    
    [ShowIf("aiType", AIType.Flanker)][SerializeField]
    private float circlingDistance = 12;

    private Animator _animator;
    public bool facingRight, facingUp, facingDown;

    private SpriteRenderer _headRenderer;
    private SpriteRenderer _torsoRenderer;
    
    [Space(40)]
    [Header("Body Parts")]
    public GameObject Head;
    [SerializeField] private GameObject Torso;
    [SerializeField] private GameObject[] Legs;
    
    private SpriteRenderer[] _legRenderers;
    
    [HideInInspector] public bool IsKnockedBack;
    [SerializeField] private float knockbackDuration = 0.2f;
    private Coroutine _knockbackCoroutine;

    public enum AIType
    {
        Rusher, // Chases the player.
        Shooter, // Shoots the player at a certain distance.
        Flanker // Circles around the player, occasionally dashes to the player.
    }

    public enum ShootingType
    {
       SEMI_SLOW,
       SEMI_FAST,
       BURST_SLOW,
       BURST_FAST
    }

    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _agent =  GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _gun = gameObject.GetComponentInChildren<AimAndShoot>();
        
        
        _animator = GetComponent<Animator>();
        _headRenderer = Head.GetComponent<SpriteRenderer>();
        _torsoRenderer = Torso.GetComponent<SpriteRenderer>();
        _legRenderers = new SpriteRenderer[Legs.Length];
        for (int i = 0; i < Legs.Length; i++)
            _legRenderers[i] = Legs[i].GetComponent<SpriteRenderer>();

        playerBodyPart = new GameObject[_target.childCount];
        for (int i = 0; i < _target.childCount; i++)
        {
            playerBodyPart[i] = _target.GetChild(i).gameObject;
        }
        
        _gun.WeaponSwitching(itemData[Random.Range(0, itemData.Length)]);
        pickrandombodypart = playerBodyPart[Random.Range(0, 2)].transform;
    }


    private bool _isShooting;
    private Transform pickrandombodypart;

    void Update()
    {
        
        if (IsKnockedBack) return;
        switch (aiType)
        {
            case AIType.Rusher:
                RushPlayer();
                break;
            case AIType.Shooter:
                if (!_isShooting)
                {
                    StartCoroutine(ShootThePlayer());
                    CheckdistancetoPlayer();
                }

                break;
            case AIType.Flanker:
                FlankPlayer();
                break;
        }

        if (aiType == AIType.Shooter)
        {
            direction = (pickrandombodypart.position - transform.position).normalized;
        }
        else
        {
            direction = (_agent.destination - transform.position).normalized;
        }
        
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (aiType == AIType.Shooter && pickrandombodypart.name == "Head")
        {
            angle -= headshotpreference;
        }
        RotateEnemy();
    }
    
    void RotateEnemy()
    {
        bool horizontal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
        Vector3 scale = transform.localScale;
        
        _animator.SetBool("Moving", _agent.velocity.sqrMagnitude > 0);
        //_animator.SetBool("IsRunning", _agent.velocity.sqrMagnitude > 0);
        if (horizontal)
        {
            facingRight = direction.x > 0;
            scale.x = facingRight ? 1f : -1f;  
            transform.localScale = scale;
            SetFlip(!facingRight);
            _headRenderer.sortingOrder = 2;
            
            _animator.SetFloat("LookX", 1);
            _animator.SetFloat("XInput", -1);
            _animator.SetFloat("Yinput", 0);
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
            _animator.SetFloat("LookY", 1);
            _animator.SetFloat("XInput",0);
            _animator.SetFloat("Yinput", -1);
        }
        Head.transform.localRotation = horizontal
            ? Quaternion.Euler(0, 0, Mathf.Clamp(direction.y * 30f, -30, 30f))
            : Quaternion.identity;
    }

    void RushPlayer()
    {
        _agent.SetDestination(_target.position);
        _agent.speed = speed;
    }

    IEnumerator ShootThePlayer()
    {
        _isShooting = true;


        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0 && Vector2.Distance(transform.position, _target.position) <= _gun.weaponRange)
        {
            int burstcounter = 1; // 1 is the default

            float burstcoundown = 0.5f; // default

            switch (shootingType)
            {
                case ShootingType.SEMI_SLOW:
                    burstcounter = 1;
                    burstcoundown = 0.7f;
                    break;
                case ShootingType.SEMI_FAST:
                    burstcounter = 1;
                    burstcoundown = 0.5f;
                    break;
                case ShootingType.BURST_SLOW:
                    burstcounter = Random.Range(3, 7);
                    burstcoundown = 0.3f;
                    break;
                case ShootingType.BURST_FAST:
                    burstcounter = Random.Range(3, 7);
                    burstcoundown = 0.1f;
                    break;
                    
            }

            _agent.SetDestination(transform.position);
            pickrandombodypart = playerBodyPart[Random.Range(0, 2)].transform;
            
            for (int i = burstcounter; i > 0; i--)
            {
                Debug.Log("Aiming at: " + pickrandombodypart.name);
                
                _gun.animator.Play("Firing");
                AudioSource.PlayClipAtPoint(_gun.firingSfx, transform.position, 6f);
                _gun.EnemyShoot(pickrandombodypart.position);
                
                yield return new WaitForSeconds(burstcoundown);
            }
            fireTimer = 0.3f;
      
        }

        yield return new WaitForSeconds(0.1f);
        _isShooting = false;
    }

    void CheckdistancetoPlayer()
    {
        if (Vector2.Distance(transform.position, _target.position) <= _gun.weaponRange)
        {
            _agent.speed = 0;
        }
        else
        {
            _agent.SetDestination(_target.position);
            _agent.speed = _runspeed;
        }
    }
    
    void FlankPlayer()
    {
        
    }
    
    
    private void SetFlip(bool flip)
    {
        _headRenderer.flipX = flip;
        _torsoRenderer.flipX = flip;
        foreach (var leg in _legRenderers)
            leg.flipX = flip;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 12 + distanceToDetectPlayer);
    }
}
