using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealthScript : MonoBehaviour
{
    [HideInInspector] 
    public float brainhealth, headhealth, bodyhealth, hearthealth, lungshealth;
    public float Brainhealth, Headhealth, Bodyhealth, Hearthealth, Lungshealth;
    private float _invincibility;
    private bool _canTakeDamage = true;
    [SerializeField]public bool dashingInvincibility = true; 

    GameObject[] _playerBodyPart;

    public enum BodyParts
    {
        Head,
        Brain,
        Torso,
        Heart,
        Lungs
    }

    [SerializeField] private Vector2 knockbackForce;

    [SerializeField] private float detachDistance = 8f;
    private bool _isDead;
    private Transform _torso;
    [HideInInspector] public GameObject lastAttacker;

    private EnemyAI _enemyAI;

    private void Start()
    {
        _enemyAI = GetComponent<EnemyAI>();

        if (Brainhealth == 0 || Headhealth == 0 || Bodyhealth == 0 || Hearthealth == 0 || Lungshealth == 0)
        {
            Debug.LogError("A body part doesn't have a health number, it cannot have 0");
        }
        else
        {
            brainhealth = Brainhealth;
            headhealth = Headhealth;
            bodyhealth = Bodyhealth;
            hearthealth = Hearthealth;
            lungshealth = Lungshealth;
        }

        lastAttacker = GameObject.FindGameObjectWithTag("Player");

        _playerBodyPart = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            _playerBodyPart[i] = transform.GetChild(i).gameObject;
        }
    }

    public void DisableDeath()
    {
        _isDead = false;
        _torso = null;
    }



    private void Update()
    {
        if (_invincibility > 0)
            _invincibility -= Time.deltaTime;

        if (_invincibility <= 0)
            _canTakeDamage = true;

        if (!_isDead && (brainhealth <= 0 || headhealth <= 0 || bodyhealth <= 0 || hearthealth <= 0 || lungshealth <= 0))
        {
            OnDeath();
        }

        if (_isDead && _torso != null && gameObject.CompareTag("Enemy"))
        {
            foreach (var rb in GetComponentsInChildren<Rigidbody2D>(true))
            {
                if (rb.gameObject == _torso.gameObject) continue;
                float dist = Vector2.Distance(_torso.position, rb.transform.position);
                if (dist > detachDistance)
                {
                    rb.transform.SetParent(null);
                    Destroy(rb.GetComponent<HingeJoint2D>());
                    Destroy(rb, 5f);
                }
            }
        }
    }

    public void OnDamageTaken(BodyParts bodypart, float damage, GameObject target)
    {
        if (_canTakeDamage && dashingInvincibility)
        {
            switch (bodypart)
            {
                case BodyParts.Brain:
                    brainhealth -= damage;
                    break;
                case BodyParts.Head:
                    headhealth -= damage;
                    break;
                case BodyParts.Torso:
                    bodyhealth -= damage;
                    break;
                case BodyParts.Heart:
                    hearthealth -= damage;
                    break;
                case BodyParts.Lungs:
                    lungshealth -= damage;
                    break;
            }

            brainhealth = Mathf.Clamp(brainhealth, 0, Brainhealth);
            headhealth = Mathf.Clamp(headhealth, 0, Headhealth);
            bodyhealth = Mathf.Clamp(bodyhealth, 0, Bodyhealth);
            hearthealth = Mathf.Clamp(hearthealth, 0, Hearthealth);
            lungshealth = Mathf.Clamp(lungshealth, 0, Lungshealth);

            if (CompareTag("Player"))
            {
                _invincibility = 0;
                _canTakeDamage = false;
            }
            if (CompareTag("Player"))
            {
                Transform head = transform.Find("Head");
                if (head != null)
                {
                    Rigidbody2D headRb = head.GetComponent<Rigidbody2D>();
                    if (headRb != null)
                    {
                        Vector2 knockDir = ((Vector2)transform.position - (Vector2)target.transform.position)
                            .normalized;
                        headRb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);
                    }
                }
            }


        }
    }

    public void RestoreHealth(BodyParts bodypart, float healingamount)
    {
        switch (bodypart)
        {
            case BodyParts.Brain:
                brainhealth += healingamount;
                break;
            case BodyParts.Head:
                headhealth += healingamount;
                break;
            case BodyParts.Torso:
                bodyhealth += healingamount;
                break;
            case BodyParts.Heart:
                hearthealth += healingamount;
                break;
            case BodyParts.Lungs:
                lungshealth += healingamount;
                break;
        }

        brainhealth = Mathf.Clamp(brainhealth, 0, Brainhealth);
        headhealth = Mathf.Clamp(headhealth, 0, Headhealth);
        bodyhealth = Mathf.Clamp(bodyhealth, 0, Bodyhealth);
        hearthealth = Mathf.Clamp(hearthealth, 0, Hearthealth);
        lungshealth = Mathf.Clamp(lungshealth, 0, Lungshealth);
  
    }



    public void OnDeath()
    {
        if (_isDead) return;
        _isDead = true;

        foreach (var joint in GetComponentsInChildren<HingeJoint2D>(true))
        {
            joint.enabled = true;
        }

        foreach (var rb in GetComponentsInChildren<Rigidbody2D>(true))
        {
            rb.isKinematic = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0;
            rb.drag = 10f;
            rb.angularDrag = 10f;
        }

        GetComponentInChildren<AimAndShoot>().OnDeath();

        foreach (var d in GetComponentsInChildren<Collider2D>(true))
        {
            d.isTrigger = false;
        }


        _torso = transform;
        print("DEAD");
        GetComponent<Animator>().enabled = false;
        GetComponent<ClothesScript>().enabled = false;
        bool horizontal = false;


        Rigidbody2D headrb;
        if (gameObject.CompareTag("Enemy"))
        {
            GetComponent<EnemyAI>().enabled = false;
            horizontal = Mathf.Abs(_enemyAI.direction.x) > Mathf.Abs(_enemyAI.direction.y);
            headrb = _enemyAI.Head.GetComponent<Rigidbody2D>();
            Destroy(GetComponent<NavMeshAgent>());
        }
        else
        {
            headrb = _playerBodyPart[0].GetComponent<Rigidbody2D>();
            GetComponent<PlayerMovement>().enabled = false;
            PlayerDirection playerDirection = gameObject.GetComponent<PlayerDirection>();
            playerDirection.enabled = false;
            horizontal = Mathf.Abs(playerDirection.direction.x) > Mathf.Abs(playerDirection.direction.y);
        }




        AimAndShoot aimAndShoot = GetComponentInChildren<AimAndShoot>();
        if (aimAndShoot != null)
        {
            aimAndShoot.hands[0].SetActive(true);
            aimAndShoot.hands[1].SetActive(true);
        }

        aimAndShoot.RemoveWeaponOnDeath();


        if (horizontal)
        {
            switch (_enemyAI.facingRight)
            {
                case true:
                    headrb.AddForce(new Vector2(-knockbackForce.x, 0), ForceMode2D.Impulse);
                    break;
                case false:
                    headrb.AddForce(new Vector2(knockbackForce.x, 0), ForceMode2D.Impulse);
                    break;
            }
        }
        else
        {

            switch (_enemyAI.facingUp)
            {
                case true:
                    headrb.AddForce(new Vector2(0, -knockbackForce.x), ForceMode2D.Impulse);
                    break;
                case false:
                    headrb.AddForce(new Vector2(0, knockbackForce.x), ForceMode2D.Impulse);
                    break;
            }
        }
    }
}
