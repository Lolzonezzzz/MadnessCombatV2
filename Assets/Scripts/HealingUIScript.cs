using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(HealingUIScript))]
public class HealingUIScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float healAmount;
    [SerializeField] private HealthScript.BodyParts[] healsBodyPart;
    public float capacity;
    [SerializeField] private TMP_Text capacityText;
    
    public  HealthScript.BodyParts selectedBodyParts = HealthScript.BodyParts.None;
    
    public static HealingUIScript SelectedItem;
    public static bool IsHealing;
    
    
    Camera _mainCamera;
    private HealthScript _health;

    void Start()
    {
        _health = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthScript>();
        if (capacityText != null)
            capacityText.text = capacity.ToString(CultureInfo.CurrentCulture);
        
        _mainCamera = Camera.main;
        

    }

    public void ChangeUIText()
    {
        capacityText.text = capacity.ToString(CultureInfo.CurrentCulture);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (capacity <= 0) return;

        if (SelectedItem == this)
        {
            SelectedItem = null;
            IsHealing = false;
            return;
        }

        SelectedItem = this;
        IsHealing = true;
    }


    void Update()
    {
        if (SelectedItem != null && selectedBodyParts != HealthScript.BodyParts.None && healsBodyPart.Contains(selectedBodyParts))
        {
            print(selectedBodyParts);
            bool health = _health.GetHealth(selectedBodyParts);
            if (health)
            {
                _health.RestoreHealth(selectedBodyParts, healAmount);
                capacity--;
                capacityText.text = capacity.ToString(CultureInfo.CurrentCulture);
            }
            SelectedItem = null;
            IsHealing = false;
            selectedBodyParts = HealthScript.BodyParts.None;
        }
    }
}