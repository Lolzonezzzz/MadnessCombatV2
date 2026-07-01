using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HealingUIScript : MonoBehaviour
{
    [SerializeField] private float healAmount;
    [SerializeField] private HealthScript.BodyParts[] selectedbodyPart;
    public float capacity;
    [SerializeField] private TMP_Text capacityText;


    public void StartHealing()
    {
        StartCoroutine(Healamount());
    }
    
    IEnumerator Healamount()
    {
        HealthScript healthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthScript>();
        
        Camera _mainCamera = Camera.main;
        
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                -_mainCamera.transform.position.z));

        yield return new WaitForSeconds(0.1f);

        if (Input.GetMouseButtonDown(0) && !capacity.Equals(0))
        {
            RaycastHit2D mouseClick = Physics2D.Raycast(mousePos, Vector2.zero); 
            if (mouseClick.collider != null)
            {
                HealthUIScript healthUIScript = mouseClick.collider.GetComponent<HealthUIScript>();
                
                if (selectedbodyPart.Contains(healthUIScript.bodyPart) && healthUIScript != null)
                    healthScript.RestoreHealth(healthUIScript.bodyPart, healAmount);
            }
        }

    }
}
