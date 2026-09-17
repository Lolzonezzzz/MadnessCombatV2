using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPOint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
