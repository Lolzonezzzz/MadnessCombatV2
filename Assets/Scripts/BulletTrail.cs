using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float Progress;
    
    public float _speed = 40f;
    void Start()
    {
        Destroy(gameObject, 2f);
    }
    
    void Update()
    {
        Progress += Time.deltaTime * _speed;
        transform.position = Vector3.Lerp(_startPosition, _targetPosition, Progress);
        
    }


    public void SetTargetPosition(Vector3 targetPosition)
    {
        _startPosition = transform.position.WithAxis(Axis.Z, -1);
        _targetPosition = targetPosition.WithAxis(Axis.Z, -1);
    }
}
