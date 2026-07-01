using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWallScript : MonoBehaviour
{
    [SerializeField] private float wallHealth;
    private float _wallhealth;
    [SerializeField] private float Durability = 1f; // this will divide by the damage given to the wall

    float _canTakeDamage = 0;

    private void Start()
    {
        _wallhealth = wallHealth;
    }

    private void Update()
    {
        if (_canTakeDamage > 0)
        {
            _canTakeDamage -= Time.deltaTime;
        }
    }

    public void TakeDamage(float damage)
    {
        if (_canTakeDamage <= 0)
        {
            _wallhealth -= damage / Durability;
            _canTakeDamage = 2;
        }


        if (_wallhealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
