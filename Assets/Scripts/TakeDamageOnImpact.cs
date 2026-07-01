using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TakeDamageOnImpact : MonoBehaviour
{
    private GameObject _player;
    [SerializeField]private float damage;


    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HealthScript health = other.gameObject.GetComponent<HealthScript>();
            health.OnDamageTaken((HealthScript.BodyParts)Random.Range(0, 5), damage, null);
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HealthScript health = other.gameObject.GetComponent<HealthScript>();
            health.OnDamageTaken((HealthScript.BodyParts)Random.Range(0, 5), damage, null);
        }
    }
}
