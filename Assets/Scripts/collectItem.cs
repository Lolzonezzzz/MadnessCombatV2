using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectItem : MonoBehaviour
{
    [SerializeField] private HealingUIScript healingUIScript;
    private Camera _mainCamera;
    
    void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = _mainCamera.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                    -_mainCamera.transform.position.z)
            );

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0.1f, LayerMask.GetMask("HealthParts"));
            if (hit.collider == null) return;
            if (hit.collider.gameObject != gameObject) return;
            healingUIScript.capacity++;
            healingUIScript.ChangeUIText();
            Destroy(gameObject);
        }
    }
}
