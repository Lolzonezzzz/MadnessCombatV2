using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class OpenLockerScript : MonoBehaviour
{
    [SerializeField] LootTable lootTable;
    private bool _playerInRange;

    private void Update()
    {
        if (_playerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            Roll();
        }
    }

    void Roll()
    {
        ItemData itemData = lootTable.GetDrop();
        
        // Cloning the item as a gameobject
        
        GameObject gunmodel = Instantiate(itemData.gunModel, transform.position, transform.rotation);
        Rigidbody2D rb = gunmodel.GetComponent<Rigidbody2D>();
        var rotation = gunmodel.transform.rotation;
        rotation.z = Random.Range(-100, 100);
        gunmodel.transform.rotation = rotation;
        gunmodel.GetComponent<ItemPickUp>().enabled = true;
        gunmodel.GetComponent<Animator>().enabled = false;
        
        
        rb.angularVelocity = 720f;
        
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
