using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrosshairScript : MonoBehaviour
{
    [Range(0, 100)]
    public float Value;
    public float speed;

    public float margin;  
    private Camera _mainCamera;
    private GunManagement _gunManagement;

    public RectTransform Top, Bottom, Left, Right, Center;
    void Start()
    {
        _mainCamera = Camera.main;
        _gunManagement = GameObject.FindWithTag("Player").GetComponentInChildren<GunManagement>();
    }
    
    void Update()
    {

        if (_gunManagement.aimPoint == null)
        {
            Value = 0;
        }
        else{ Value = _gunManagement.currentyHolding.bulletSpread;}
        
        float TopValue, BottomValue, LeftValue, RightValue;

        TopValue = Mathf.Lerp(Top.position.y, Center.position.y + margin + Value, speed * Time.deltaTime);
        BottomValue = Mathf.Lerp(Bottom.position.y, Center.position.y - margin - Value, speed * Time.deltaTime);

        LeftValue = Mathf.Lerp(Left.position.x, Center.position.x - margin - Value, speed * Time.deltaTime);
        RightValue = Mathf.Lerp(Right.position.x, Center.position.x + margin + Value, speed * Time.deltaTime);

        Top.position = new Vector2(Top.position.x, TopValue);
        Bottom.position = new Vector2(Bottom.position.x, BottomValue);

        Left.position = new Vector2(LeftValue, Center.position.y);
        Right.position = new Vector2(RightValue, Center.position.y);
    }

    void LateUpdate()
    {
        Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        gameObject.transform.position = mousePos;
    }

}
