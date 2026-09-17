using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckforwallNaviagator : MonoBehaviour
{
    public FloorScript floorcheck;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
            floorcheck.StartCoroutine(floorcheck.GetWalls());
    }
}
