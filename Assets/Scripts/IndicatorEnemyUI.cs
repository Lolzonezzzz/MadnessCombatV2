using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IndicatorEnemyUI : MonoBehaviour
{
    [SerializeField] private GameObject healthbarPrefab;
    [SerializeField] private Vector3 offset =  new Vector3(0, 1.5f, 0);
    [SerializeField] private HealthScript.BodyParts bodyPart;
    [SerializeField] private Canvas canvas;
    private RectTransform _rectTransform;
    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
        GameObject bar = Instantiate(healthbarPrefab, canvas.transform);
        HealthDigits health = bar.GetComponent<HealthDigits>();
        health.healthScript = gameObject.GetComponentInParent<HealthScript>();
        health.bodyPart = bodyPart;
        
        _rectTransform = bar.GetComponent<RectTransform>();
    }

    void Update()
    {
        _rectTransform.position = _cam.WorldToScreenPoint(transform.position + offset);
    }


    private void OnDestroy()
    {
        if (_rectTransform != null) Destroy(_rectTransform.gameObject);
    }
}
