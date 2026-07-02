using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HealingUIScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float healAmount;
    [SerializeField] private HealthScript.BodyParts healsBodyPart;
    public float capacity;
    [SerializeField] private TMP_Text capacityText;

    public  HealingUIScript SelectedItem;
    public static bool IsHealing;

    private HealthScript _health;

    void Start()
    {
        _health = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthScript>();
        if (capacityText != null)
            capacityText.text = capacity.ToString();
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

    public void OnBodyPartClicked()
    {
        if (!IsHealing || SelectedItem == null) return;

        _health.RestoreHealth(healsBodyPart, SelectedItem.healAmount);
        SelectedItem.capacity--;
        if (SelectedItem.capacityText != null)
            SelectedItem.capacityText.text = SelectedItem.capacity.ToString();

        SelectedItem = null;
        IsHealing = false;
    }
}