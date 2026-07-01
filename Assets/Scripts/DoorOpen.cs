using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    [SerializeField]LayerMask playerLayerMask;
    [SerializeField] private AudioClip doorOpenSound;
    
    Animator _animator;
    bool _doorOpened = false;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        RaycastHit2D detectionCircle = Physics2D.CircleCast(transform.position, 12f, Vector2.zero, 0f, playerLayerMask);
        if (detectionCircle.collider != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                _doorOpened = true;
                Destroy(gameObject, 1f);
                AudioSource.PlayClipAtPoint(doorOpenSound, transform.position);
            }
        }


        if (_doorOpened)
        {
            Vector2 vector2 = transform.position;
            vector2.y = vector2.y + 1f;
            transform.position = vector2;

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 12f);
    }
}
