using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class AimAndShoot : MonoBehaviour
{
    private PlayerDirection _playerDirection;
    private PlayerMovement _playerMovement;
    private bool _flipped = false;

    [SerializeField] private GameObject referenceObject;
    private List<GameObject> _gunParts = new List<GameObject>();

    private EnemyAI _enemyAI;
    [SerializeField] private float pickUpRange = 2f;
    [SerializeField] private LayerMask pickUpMask = -1;

    [Header("Gun Settings")] [SerializeField]
    private float damage;

    public float weaponRange = 30f;
    [SerializeField] private int burstCount = 1;
    [SerializeField] private float burstDelay = 0.07f;
    [SerializeField] private float spreadAngle = 5f; // Degrees
    [SerializeField] private int piercing = 1; // How many enemies it can hit
    [SerializeField] private float fireRate = 0.5f;


    public int maxCapacity = 6;
    private float _gunReloadSpeed = 1f;
    private GameObject _weapomPrefab;
    private bool _canShoot = true;
    private ItemData _lastHolding = null;
    private AudioClip _firingSfx;
    [SerializeField] private AudioClip emptyMagSfx;
    private bool _emptymag;
    public LayerMask canDamage;
    [SerializeField] public LayerMask wallMask = -1;
    private int _headShotMultiplier;
    [SerializeField] private int bulletsPerShot = 1;


    [Header("Weapons")] public Transform aimPoint;
    [SerializeField] private GameObject bulletTrail;
    private GameObject _shellPrefab;

    private Camera _mainCamera;
    private ItemData.WeaponTypes _weaponTypes;
    [HideInInspector] public AudioClip firingSfx;
    [SerializeField] private AudioClip[] wallHitSfx;
    private ItemData _currentyHolding;
    [HideInInspector]public Animator animator;
    private Transform _shellEject;

    [Header("Throw Settings")] [SerializeField]
    private float throwForce = 20f;

    [SerializeField] private float throwDamage = 50f;

    [HideInInspector]public GameObject[] hands;
    private Inventory _inventoryUI;
    private bool _canThrown = true;
    private bool _canSwing = true;
    private bool _isEquipping = false;
    private bool _isReloading = false;


    float GetWeaponMultiplier()
    {
        return _weaponTypes switch
        {
            ItemData.WeaponTypes.Pistols => 1f,
            ItemData.WeaponTypes.Shotguns => 8f,
            ItemData.WeaponTypes.Rifles => 1.5f,
            ItemData.WeaponTypes.Explosives => 12f,
            ItemData.WeaponTypes.MachineGuns => 3f,
            ItemData.WeaponTypes.Knives => 4f,
            _ => 2f
        };
    }

    public void unequipHands()
    {
        _canShoot = true;
        _canSwing = true;
        if (_gunParts.Count != 0)
        {
            foreach (GameObject hand in hands)
            {
                hand.SetActive(false);
            }
        }
    }

    public void RemoveWeaponOnDeath()
    {
        Destroy(_weapomPrefab);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _playerDirection = GetComponentInParent<PlayerDirection>();
        _playerMovement = GetComponentInParent<PlayerMovement>();
        _inventoryUI = GetComponentInParent<Inventory>();
        _mainCamera = Camera.main;

        _enemyAI = GetComponentInParent<EnemyAI>();


        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            if (sr.sortingLayerName == "Gun")
            {
                _gunParts.Add(sr.gameObject);
            }

        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_playerDirection != null) // filtering input by who is the holder of this script
        {
            // Mouse click pickup
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(mousePos, pickUpMask);
                if (hit != null)
                {
                    ItemPickUp pickup = hit.GetComponent<ItemPickUp>();
                    if (pickup != null &&
                        Vector2.Distance(transform.position, pickup.transform.position) <= pickUpRange)
                    {
                        pickup.PickUp(_inventoryUI);
                        return;
                    }
                }
            }

            if (_playerMovement.IsDashing)
            {
                referenceObject.SetActive(false);
                foreach (GameObject hand in hands)
                {
                    hand.SetActive(true);
                }

                _canShoot = false;
                _canSwing = false;
            }
            else
            {
                referenceObject.SetActive(true);

            }


            if (aimPoint)
            {
                if (Input.GetMouseButton(0) && _canShoot && !_isEquipping && !_isReloading && !EventSystem.current.IsPointerOverGameObject())
                {
                    StartCoroutine(BurstFire());
                }
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                _lastHolding = null;
                Destroy(_weapomPrefab);
                _gunParts.Clear();
                aimPoint = null;


                foreach (GameObject hand in hands)
                {
                    hand.SetActive(true);
                }
            }


            if (Input.GetMouseButtonDown(0) && aimPoint == null && (_currentyHolding == null || _currentyHolding.shellEject == null) &&
                _canSwing &&!EventSystem.current.IsPointerOverGameObject()) // melee
            {
                StartCoroutine(SwingMeleeScript());
            }

            if (Input.GetKeyDown(KeyCode.G) && _canThrown && !_isEquipping && !_isReloading)
            {
                StartCoroutine(ThrowGun());
            }

            if (Input.GetKeyDown(KeyCode.R) && !_isReloading && !_isEquipping && aimPoint != null)
            {
                StartCoroutine(Reload());
            }
        }
    }
    
     public IEnumerator EnemySwingMeleeScript(Vector2 targetPosition)
    {
        Vector2 dirToTarget = (targetPosition - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg;
        
        animator.Play("Swing");
        AudioSource.PlayClipAtPoint(firingSfx, transform.position, 45f);
        _canSwing = false;
        LayerMask targettedLayerMask = default;
        int hitLayer = -1;
        
        
        yield return new WaitForSeconds(0.1f);
        
        RaycastHit2D ray = Physics2D.BoxCast(transform.position, new Vector2(12f, 2f), angle,
            dirToTarget, 0.7f,targettedLayerMask.value != 0 ? targettedLayerMask : canDamage);
        _canSwing = true;
        
        if (ray.collider != null)
        {
            // Deal damage to player body part
            HealthScript healthScript = ray.collider.GetComponentInParent<HealthScript>();
            if (healthScript != null)
            {
                healthScript.lastAttacker = gameObject;
                print("touching enemy");
                switch (LayerMask.LayerToName(ray.collider.gameObject.layer))
                {
                    case "PlayerHead":
                        int randomPart = Random.Range(0, 1);
                        if (randomPart == 0) // skull
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Head, damage, ray.collider.gameObject);
                        }
                        else if (randomPart == 1) // brain
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Head, damage, ray.collider.gameObject);
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Brain, damage, ray.collider.gameObject);
                        }

                        break;
                    case "PlayerTorso":
                        randomPart = Random.Range(0, 2);
                        if (randomPart == 0) // ribcage
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Torso, damage, ray.collider.gameObject);
                        }
                        else if (randomPart == 1) // heart
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Heart, damage, ray.collider.gameObject);
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Torso, damage, ray.collider.gameObject);
                        }
                        else if (randomPart == 2) // lungs
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Lungs, damage, ray.collider.gameObject);
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Torso, damage, ray.collider.gameObject);
                        }

                        break;
                }
            }
        }
    }


    IEnumerator SwingMeleeScript()
    {
        animator.Play("Swing");
        AudioSource.PlayClipAtPoint(firingSfx, transform.position, 45f);
        _canSwing = false;
        LayerMask targettedLayerMask = default;
        int hitLayer = -1;
        
        
        yield return new WaitForSeconds(0.1f);
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                -_mainCamera.transform.position.z)
        );
        mousePos.z = 0f;
        Collider2D mouseOverlap = Physics2D.OverlapPoint(mousePos, canDamage);
        if (mouseOverlap != null)
        {
            hitLayer = mouseOverlap.gameObject.layer;
            targettedLayerMask = 1 << hitLayer;
        }
        
        RaycastHit2D ray = Physics2D.BoxCast(transform.position, new Vector2(12f, 2f), _playerDirection.angle,
            Vector2.right, 0.7f,targettedLayerMask.value != 0 ? targettedLayerMask : canDamage);
        int idx = Mathf.Clamp(_inventoryUI.itemCount, 0, _inventoryUI.items.Count - 1);
        _canSwing = true;
        
        if (ray.collider != null)
        {
            // Deal damage to enemy body part
            HealthScript healthScript = ray.collider.GetComponentInParent<HealthScript>();
            if (healthScript != null)
            {
                healthScript.lastAttacker = gameObject;
                print("touching enemy");
                switch (LayerMask.LayerToName(ray.collider.gameObject.layer))
                {
                    case "EnemyHead":
                        int randomPart = Random.Range(0, 1);
                        if (randomPart == 0) // skull
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Head, damage, ray.collider.gameObject);
                        }
                        else if (randomPart == 1) // brain
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Head, damage, ray.collider.gameObject);
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Brain, damage, ray.collider.gameObject);
                        }

                        break;
                    case "EnemyTorso":
                        randomPart = Random.Range(0, 2);
                        if (randomPart == 0) // ribcage
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Torso, damage, ray.collider.gameObject);
                        }
                        else if (randomPart == 1) // heart
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Heart, damage, ray.collider.gameObject);
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Torso, damage, ray.collider.gameObject);
                        }
                        else if (randomPart == 2) // lungs
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Lungs, damage, ray.collider.gameObject);
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Torso, damage, ray.collider.gameObject);
                        }

                        break;
                }
            }
            else if (ray.collider.CompareTag("BreakableWall"))
            {
                AudioSource.PlayClipAtPoint(wallHitSfx[Random.Range(0, wallHitSfx.Length)], transform.position, 40f);
                _inventoryUI.ammo[idx]--;
                BreakableWallScript breakableWallScript = ray.collider.GetComponent<BreakableWallScript>();
                breakableWallScript.TakeDamage(_currentyHolding.damage);
            }
            else if (ray.collider.CompareTag("Box"))
            {
                AudioSource.PlayClipAtPoint(wallHitSfx[Random.Range(0, wallHitSfx.Length)], transform.position, 40f);
                _inventoryUI.ammo[idx]--;
                BoxLootBox boxLootBox =  ray.collider.GetComponent<BoxLootBox>();
                boxLootBox.TakeDamage(_currentyHolding.damage);
            }
        }
    }
    

    IEnumerator BurstFire()
    {
        if (_inventoryUI.items.Count == 0) yield break;
        int idx = Mathf.Clamp(_inventoryUI.itemCount, 0, _inventoryUI.items.Count - 1);
        if (_inventoryUI.ammo[idx] <= 0)
        {
            if (emptyMagSfx != null)
                AudioSource.PlayClipAtPoint(emptyMagSfx, transform.position);
            yield break;
        }

        _canShoot = false;
        for (int i = 0; i < burstCount; i++)
        {
            if (_weaponTypes != ItemData.WeaponTypes.Pistols)
            {
                int randomShootingAim = Random.Range(0, 2);

                switch (randomShootingAim)
                {
                    case 0:
                        animator.Play("Firing");
                        break;
                    case 1:
                        animator.Play("Firing1");
                        break;
                }
            }
            else
            {
                animator.Play("Firing");
            }
            
            if (firingSfx != null)
                AudioSource.PlayClipAtPoint(firingSfx, transform.position, 6f);
            for (int s = 0; s < bulletsPerShot; s++)
            {
                float t = bulletsPerShot == 1 ? 0.5f : (float)i / (bulletsPerShot - 1);

                float shotAngle = _playerDirection.angle +
                                  Mathf.Lerp(-spreadAngle, spreadAngle, t);

                Shoot(shotAngle);
            }

            if (i < burstCount - 1)
                yield return new WaitForSeconds(burstDelay);
        }

        _inventoryUI.ammo[idx]--;
        SpawnShell();
        yield return new WaitForSeconds(fireRate);
        _canShoot = true;
    }

    private void Shoot(float shotAngle)
    {
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                -_mainCamera.transform.position.z)
        );
        mousePos.z = 0f;
        float offset = Random.Range(-spreadAngle, spreadAngle);

        Vector2 shootDirection =
            Quaternion.Euler(0, 0, offset) *
            (Vector2)aimPoint.right;
			
        float distance = Vector2.Distance(mousePos, aimPoint.position);
        LayerMask targetLayerMask = default;
        int hitLayer = -1;
        Collider2D mouseHit = Physics2D.OverlapPoint(mousePos, canDamage);
        if (mouseHit != null)
        {
            hitLayer = mouseHit.gameObject.layer;
            targetLayerMask = 1 << hitLayer;
        }
        
        // Find nearest wall hit (exclude self)
        int selfLayer = gameObject.layer;
        int mask = wallMask & ~(1 << selfLayer);
        RaycastHit2D wallHit = Physics2D.Raycast(
            aimPoint.position,
            shootDirection,
            weaponRange,
            mask
        );

        // RaycastAll for enemies (piercing)
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            aimPoint.position,
            shootDirection,
            weaponRange,
            targetLayerMask.value != 0 ? targetLayerMask : canDamage
        );

        // Instantiate bullet trail
        var bullet = Instantiate(bulletTrail, aimPoint.position, Quaternion.identity);
        bullet.transform.position = aimPoint.position;
        var trailScript = bullet.GetComponent<BulletTrail>();

        float wallDist = wallHit ? wallHit.distance : weaponRange;
        Vector3 endPos = aimPoint.position + (Vector3)shootDirection * weaponRange;

        void PlayWallHit(Vector3 pos)
        {
            if (wallHitSfx == null || wallHitSfx.Length == 0) return;
            var clip = wallHitSfx[Random.Range(0, wallHitSfx.Length)];
            AudioSource.PlayClipAtPoint(clip, pos, 12f);
        }

        if (hits.Length == 0)
        {
            if (wallHit)
            {
                endPos = wallHit.point;
                PlayWallHit(wallHit.point);
                if (wallHit.collider.CompareTag("BreakableWall"))
                {
                    var breakable = wallHit.collider.GetComponent<BreakableWallScript>();
                    if (breakable != null) breakable.TakeDamage(damage);
                }
                else if (wallHit.collider.CompareTag("Box"))
                {
                    AudioSource.PlayClipAtPoint(wallHitSfx[Random.Range(0, wallHitSfx.Length)], transform.position, 40f);
                    BoxLootBox boxLootBox =  wallHit.collider.GetComponent<BoxLootBox>();
                    boxLootBox.TakeDamage(_currentyHolding.damage);
                }
            }
            trailScript.SetTargetPosition(endPos);
            return;
        }

        // Handle piercing
        int hitCount = 0;
        Vector3 lastHitPoint = endPos;

        bool hitWall = false;
        foreach (var hit in hits)
        {
            if (hit.distance > wallDist)
            {
                hitWall = true;
                break;
            }
            lastHitPoint = hit.point;
            hitCount++;

            // Deal damage to enemy body part
            HealthScript healthScript = hit.collider.GetComponentInParent<HealthScript>();
            if (healthScript != null)
            {
                healthScript.lastAttacker = gameObject;
                print(LayerMask.LayerToName(hit.collider.gameObject.layer));
                switch (LayerMask.LayerToName(hit.collider.gameObject.layer))
                {
                    case "EnemyHead":
                        int randomPart = Random.Range(0, 2);
                        if (randomPart == 0) // skull
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Head, damage, hit.collider.gameObject);
                        }
                        else if (randomPart == 1) // brain
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Head, damage, hit.collider.gameObject);
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Brain, damage, hit.collider.gameObject);
                        }
                        break;
                    case "EnemyTorso":
                        randomPart = Random.Range(0, 3);
                        if (randomPart == 0) // ribcage
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Torso, damage, hit.collider.gameObject);
                        }
                        else if (randomPart == 1) // heart
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Heart, damage, hit.collider.gameObject);
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Torso, damage, hit.collider.gameObject);
                        }
                        else if (randomPart == 2) // lungs
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Lungs, damage, hit.collider.gameObject);
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Torso, damage, hit.collider.gameObject); 
                        }
                        break;
                }
                
            }               
            else if (wallHit.collider.CompareTag("Box"))
            {
                AudioSource.PlayClipAtPoint(wallHitSfx[Random.Range(0, wallHitSfx.Length)], transform.position, 40f);
                BoxLootBox boxLootBox =  wallHit.collider.GetComponent<BoxLootBox>();
                boxLootBox.TakeDamage(_currentyHolding.damage);
            }
            

            if (hitCount >= piercing) break;
        }

        if (hitWall)
        {
            PlayWallHit(wallHit.point);
            if (wallHit.collider.CompareTag("BreakableWall"))
            {
                var breakable = wallHit.collider.GetComponent<BreakableWallScript>();
                if (breakable != null) breakable.TakeDamage(damage);
            }
            else if (wallHit.collider.CompareTag("Box"))
            {
                AudioSource.PlayClipAtPoint(wallHitSfx[Random.Range(0, wallHitSfx.Length)], transform.position, 40f);
                BoxLootBox boxLootBox =  wallHit.collider.GetComponent<BoxLootBox>();
                boxLootBox.TakeDamage(_currentyHolding.damage);
            }
        }

        trailScript.SetTargetPosition(lastHitPoint);
    }

    public void EnemyShoot(Vector2 targetPosition)
    {
        // Rotate gun to face target
        Vector2 dirToTarget = (targetPosition - (Vector2)aimPoint.position).normalized;
        float angle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg;

        float offset = Random.Range(-spreadAngle, spreadAngle);
        Vector2 shootDirection = Quaternion.Euler(0, 0, offset) * aimPoint.right;

        // Find nearest wall hit
        int selfLayer = gameObject.layer;
        int mask = wallMask & ~(1 << selfLayer);
        RaycastHit2D wallHit = Physics2D.Raycast(aimPoint.position, shootDirection, weaponRange, mask);

        // RaycastAll for enemies (piercing)
        RaycastHit2D[] hits = Physics2D.RaycastAll(aimPoint.position, shootDirection, weaponRange, canDamage);

        // Instantiate bullet trail
        var bullet = Instantiate(bulletTrail, aimPoint.position, Quaternion.identity);
        bullet.transform.position = aimPoint.position;
        
        var trailScript = bullet.GetComponent<BulletTrail>();

        float wallDist = wallHit ? wallHit.distance : weaponRange;
        Vector3 endPos = aimPoint.position + (Vector3)shootDirection * weaponRange;

        void PlayWallHit(Vector3 pos)
        {
            if (wallHitSfx == null || wallHitSfx.Length == 0) return;
            var clip = wallHitSfx[Random.Range(0, wallHitSfx.Length)];
            AudioSource.PlayClipAtPoint(clip, pos, 12f);
        }

        if (hits.Length == 0)
        {
            if (wallHit)
            {
                endPos = wallHit.point;
                PlayWallHit(wallHit.point);
                if (wallHit.collider.CompareTag("BreakableWall"))
                {
                    var breakable = wallHit.collider.GetComponent<BreakableWallScript>();
                    if (breakable != null) breakable.TakeDamage(damage);
                }
            }
            trailScript.SetTargetPosition(endPos);
            return;
        }

        int hitCount = 0;
        Vector3 lastHitPoint = endPos;

        bool hitWall = false;
        foreach (var hit in hits)
        {
            if (hit.distance > wallDist)
            {
                hitWall = true;
                break;
            }
            lastHitPoint = hit.point;
            hitCount++;

            HealthScript healthScript = hit.collider.GetComponentInParent<HealthScript>();
            if (healthScript != null)
            {
                int hitLayer = hit.collider.gameObject.layer;
                switch (LayerMask.LayerToName(hitLayer))
                {
                    case "PlayerHead":
                        int randomPart = Random.Range(0, 2);
                        if (randomPart == 0)
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Head, damage, hit.collider.gameObject);
                        }
                        else
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Head, damage, hit.collider.gameObject);
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Brain, damage, hit.collider.gameObject);
                        }
                        break;
                    case "PlayerTorso":
                        randomPart = Random.Range(0, 3);
                        if (randomPart == 0)
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Torso, damage, hit.collider.gameObject);
                        }
                        else if (randomPart == 1)
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Heart, damage, hit.collider.gameObject);
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Torso, damage, hit.collider.gameObject);
                        }
                        else
                        {
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Lungs, damage, hit.collider.gameObject);
                            healthScript.OnDamageTaken(HealthScript.BodyParts.Torso, damage, hit.collider.gameObject);
                        }
                        break;
                }
            }

            if (hitCount >= piercing) break;
        }
        
        if (hitWall)
        {
            lastHitPoint = wallHit.point;
            PlayWallHit(wallHit.point);
            if (wallHit.collider.CompareTag("BreakableWall"))
            {
                var breakable = wallHit.collider.GetComponent<BreakableWallScript>();
                if (breakable != null) breakable.TakeDamage(damage);
            }
        }

        trailScript.SetTargetPosition(lastHitPoint);
    }

    private void SpawnShell()
    {
        if (_shellPrefab == null) return;
        Vector3 pos = _shellEject != null ? _shellEject.position : transform.position;
        GameObject shell = Instantiate(_shellPrefab, pos, Quaternion.identity);
        Rigidbody2D rb = shell.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(Random.Range(-2f, 2f), Random.Range(1f, 3f));
            rb.angularVelocity = Random.Range(-720f, 720f);
        }
        StartCoroutine(StopShell(shell, rb));
        Destroy(shell, 1.5f);
    }

    IEnumerator StopShell(GameObject shell, Rigidbody2D rb)
    {
        yield return new WaitForSeconds(1.1f);
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.angularVelocity = 0f;
        }
    }




    IEnumerator ThrowGun()
    {
        _canShoot = false;
        _lastHolding = null;

        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                -_mainCamera.transform.position.z));
        mousePos.z = 0f;

        Vector2 throwDir = (mousePos - transform.position).normalized;

        // Spawn thrown gun
        GameObject thrown = Instantiate(_weapomPrefab, transform.position, Quaternion.identity);
        Animator thrownAnimator = thrown.GetComponent<Animator>();
        thrown.layer = LayerMask.NameToLayer("ItemPickUP");
        Destroy(thrownAnimator);
        bool itemPickUp = thrown.GetComponent<ItemPickUp>().enabled = true;
        bool throwgun = thrown.GetComponent<ThrownGun>().enabled = true;
        Rigidbody2D rb = thrown.GetComponent<Rigidbody2D>();


        // Rotate to face direction

        thrown.transform.rotation = Quaternion.Euler(0, 0, _playerDirection.angle);
        // Cleanup current weapon
        Destroy(_weapomPrefab);
        _gunParts.Clear();
        aimPoint = null;

        _inventoryUI.items.RemoveAt(_inventoryUI.itemCount);
        _inventoryUI.ammo.RemoveAt(_inventoryUI.itemCount);
        _inventoryUI.ammoreserve.RemoveAt(_inventoryUI.itemCount);

        if (_inventoryUI.itemCount >= _inventoryUI.items.Count)
            _inventoryUI.itemCount = _inventoryUI.items.Count - 1;

        foreach (GameObject hand in hands)
            hand.SetActive(true);

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.velocity = throwDir * throwForce;

        yield return new WaitForSeconds(fireRate);
        // wait until animation finishes
        if (animator != null)
        {
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                _canShoot = false;
                _canThrown = false;
                yield return null;

            }
        }
        _canThrown = true;
        _canShoot = true;
    }

    IEnumerator Reload()
    {
        if (_inventoryUI.items.Count == 0) yield break;
        int idx = Mathf.Clamp(_inventoryUI.itemCount, 0, _inventoryUI.items.Count - 1);

        _isReloading = true;
        _canShoot = false;

        if (_inventoryUI.ammo[idx] >= maxCapacity)
        {
            _isReloading = false;
            _canShoot = true;
            yield break;
        }

        if (_inventoryUI.ammoreserve[idx] <= 0)
        {
            if (emptyMagSfx != null)
                AudioSource.PlayClipAtPoint(emptyMagSfx, transform.position);
            _isReloading = false;
            _canShoot = true;
            yield break;
        }

        if (animator != null)
            animator.Play("Reloading");

        while (animator != null && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        int need = maxCapacity - _inventoryUI.ammo[idx];
        int fromReserve = Mathf.Min(need, _inventoryUI.ammoreserve[idx]);
        _inventoryUI.ammo[idx] += fromReserve;
        _inventoryUI.ammoreserve[idx] -= fromReserve;

        _isReloading = false;
        _canShoot = true;
    }

    IEnumerator WaitForEquip()
    {
        yield return null;
        while (animator != null && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        _isEquipping = false;
    }

    public void WeaponSwitching(ItemData SwitchedWeapon)
    {
        _currentyHolding = SwitchedWeapon;

        if (_currentyHolding != _lastHolding)
        {
            _gunParts.Clear();
            _lastHolding = _currentyHolding;

            if (_currentyHolding != null)
            {
                Destroy(_weapomPrefab);
            }



            _canShoot = true;
            damage = _currentyHolding.damage;
            spreadAngle = _currentyHolding.bulletSpread;
            burstCount = _currentyHolding.burstCount;
            piercing = _currentyHolding.bulletPirecing;
            weaponRange = _currentyHolding.range;
            fireRate = _currentyHolding.fireRate;
            bulletsPerShot = _currentyHolding.bulletsPerShot;
            _weaponTypes = _currentyHolding.weaponTypes;
            firingSfx = _currentyHolding.firingSound;
            maxCapacity = _currentyHolding.ammoCapacity;
            _gunReloadSpeed = _currentyHolding.gunReloadSpeed;
            _shellPrefab = _currentyHolding.shellEject;
            GetWeaponMultiplier();

            // Parent correctly
            _weapomPrefab = Instantiate(_currentyHolding.gunModel, referenceObject.transform);

            // Snap in place (good for animation)
            _weapomPrefab.transform.localPosition = Vector3.zero;
            _weapomPrefab.transform.localRotation = Quaternion.identity;
            _weapomPrefab.layer = 0;
            Debug.Log("Gun parent: " + _weapomPrefab.transform.parent.name);

            // Update references
            animator = _weapomPrefab.GetComponent<Animator>();

            if (_currentyHolding.shellEject != null)
            {
                _shellEject = _weapomPrefab.transform.Find("ShellEject");
            }

            aimPoint = _weapomPrefab.transform.Find("AimPoint")?.transform;


            AudioSource.PlayClipAtPoint(_currentyHolding.equipSound, transform.position, 2f);
            foreach (SpriteRenderer sr in _weapomPrefab.GetComponentsInChildren<SpriteRenderer>())
            {
                if (sr.sortingLayerName == "Gun" || sr.sortingLayerName == "GunFacingUp")
                {
                    _gunParts.Add(sr.gameObject);
                }
            }

            foreach (GameObject hand in hands)
            {
                hand.SetActive(false);
            }

            if (animator != null)
            {
                animator.Play("Equip");
                _isEquipping = true;
                StartCoroutine(WaitForEquip());
            }
        }
    }

    public void OnDeath()
    {
        _lastHolding = null;
        Destroy(_weapomPrefab);
        _gunParts.Clear();
        aimPoint = null;
    }

    void LateUpdate()
    {
        if (_playerDirection != null) // player
        {
            float angle = _playerDirection.angle;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            const float flipThreshold = 90f;
            bool shouldFlip = Mathf.Abs(angle) >= flipThreshold;
            if (shouldFlip != _flipped)
                _flipped = shouldFlip;

            float playerScaleX = Mathf.Sign(_playerDirection.transform.localScale.x);
            if (playerScaleX == 0f) playerScaleX = 1f;

            transform.localScale = new Vector3(
                playerScaleX,
                _flipped ? -1f : 1f,
                1f
            );

            if (_playerMovement.dashDirection == PlayerMovement.MovementDirection.Up && _gunParts.Count > 0)
            {
                for (int i = 0; i < _gunParts.Count; i++)
                {
                    if (_gunParts[i] == null)
                        continue;
                    _gunParts[i].GetComponent<SpriteRenderer>().sortingLayerName = "GunFacingUp";
                }

            }
            else
            {
                if (_gunParts.Count > 0)
                {
                    for (int i = 0; i < _gunParts.Count; i++)
                    {
                        if (_gunParts[i] == null)
                            continue;
                        _gunParts[i].GetComponent<SpriteRenderer>().sortingLayerName = "Gun";
                    }

                }
            }
        }
        else // enemy
        {
            float angle = _enemyAI.angle;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            const float flipThreshold = 90f;
            bool shouldFlip = Mathf.Abs(angle) >= flipThreshold;
            if (shouldFlip != _flipped)
                _flipped = shouldFlip;

            // Mirror the player's X flip so the net visual X is always 1 (unmirrored)
            // parent(-1) × child(-1) = 1  ✓  |  parent(1) × child(1) = 1  ✓
            float enemyScaleX = Mathf.Sign(_enemyAI.transform.localScale.x);
            if (enemyScaleX == 0f) enemyScaleX = 1f; // safety guard

            transform.localScale = new Vector3(
                enemyScaleX,
                _flipped ? -1f : 1f,
                1f
            );

            if (_enemyAI.direction.y > 0 && _gunParts.Count > 0)
            {
                for (int i = 0; i < _gunParts.Count; i++)
                {
                    if (_gunParts[i] == null)
                        continue;
                    _gunParts[i].GetComponent<SpriteRenderer>().sortingLayerName = "GunFacingUp";
                }

            }
            else
            {
                if (_gunParts.Count > 0)
                {
                    for (int i = 0; i < _gunParts.Count; i++)
                    {
                        if (_gunParts[i] == null)
                            continue;
                        _gunParts[i].GetComponent<SpriteRenderer>().sortingLayerName = "Gun";
                    }

                }
            }
        }
    }
}
