using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detection : MonoBehaviour
{
    private FlipTable _filptable;
    [SerializeField] private FlipState flipState;

    void Start()
    {
        _filptable = gameObject.GetComponentInParent<FlipTable>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _filptable.nearToPlayer =  true;
            _filptable.filpstate =  flipState;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _filptable.nearToPlayer = false;
            _filptable.filpstate =  FlipState.None;
        }
    }
}
