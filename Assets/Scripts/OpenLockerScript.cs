using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class OpenLockerScript : MonoBehaviour
{
    [SerializeField] LootTable lootTable;
    [SerializeField] Sprite openLockerSprite;
    private bool _playerInRange;
    private bool _openedLocker;
    [SerializeField] private GameObject droppoint;

    private void Update()
    {
        if (_playerInRange && Input.GetKeyDown(KeyCode.Space) && !_openedLocker)
        {
            _openedLocker = true;
            Roll();
        }
    }

    void Roll()
    {
        GetComponent<SpriteRenderer>().sprite = openLockerSprite;
        ItemData itemData = lootTable.GetDrop();
        
        // Cloning the item as a gameobject
        
        GameObject gunmodel = Instantiate(itemData.gunModel, droppoint.transform.position, Quaternion.identity);
        Rigidbody2D rb = gunmodel.GetComponent<Rigidbody2D>();
        
        gunmodel.GetComponent<ItemPickUp>().enabled = true;
        gunmodel.GetComponent<Animator>().enabled = false;
        
        // pop outward:
        rb.bodyType = RigidbodyType2D.Dynamic;
        Vector2 dir = Quaternion.identity * Vector2.right;
        rb.AddForce(dir * 15f, ForceMode2D.Impulse);
        rb.angularVelocity = 720f;
        rb.drag = 4f;         // = linearDamping in Unity 6+
        rb.angularDrag = 3f;

        StartCoroutine(StopSlide(rb, 0.6f));
    }

    IEnumerator StopSlide(Rigidbody2D rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!rb) yield break;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            _playerInRange = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            _playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            _playerInRange = false;
    }
}
