using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthDigits : MonoBehaviour
{
    public HealthScript healthScript;
    [SerializeField]private TMP_Text healthText;
    [SerializeField]private TMP_Text maxHealthText;
    
    public HealthScript.BodyParts bodyPart;


    private void Update()
    {
        switch (bodyPart)
        {
            case HealthScript.BodyParts.Brain:
                maxHealthText.text = healthScript.Brainhealth.ToString();
                break;
            case HealthScript.BodyParts.Head:
                maxHealthText.text = healthScript.Headhealth.ToString();
                break;
            case HealthScript.BodyParts.Torso:
                maxHealthText.text = healthScript.Bodyhealth.ToString();
                break;
            case HealthScript.BodyParts.Heart:
                maxHealthText.text = healthScript.Hearthealth.ToString();
                break;
            case HealthScript.BodyParts.Lungs:
                maxHealthText.text = healthScript.Lungshealth.ToString();
                break;
        }
        switch (bodyPart)
        {
            case HealthScript.BodyParts.Brain:
                healthText.text = healthScript.brainhealth.ToString();
                break;
            case HealthScript.BodyParts.Head:
                healthText.text = healthScript.headhealth.ToString();
                break;
            case HealthScript.BodyParts.Torso:
                healthText.text = healthScript.bodyhealth.ToString();
                break;
            case HealthScript.BodyParts.Heart:
                healthText.text = healthScript.hearthealth.ToString();
                break;
            case HealthScript.BodyParts.Lungs:
                healthText.text = healthScript.lungshealth.ToString();
                break;
        }
    }
}
