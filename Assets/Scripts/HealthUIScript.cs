using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIScript : MonoBehaviour
{
    private HealthScript _healthScript;
    public HealthScript.BodyParts bodyPart;
    private Image image;

    private void Start()
    {
        _healthScript = GameObject.FindWithTag("Player").GetComponent<HealthScript>();
        image = gameObject.GetComponent<Image>();
    }

    private void Update()
    {

        switch (bodyPart)
        {
            case HealthScript.BodyParts.Brain:

                float yellow_threshold = _healthScript.Brainhealth * 0.7f;
                float orange_threshold = _healthScript.Brainhealth * 0.4f;
                float red_threshold = _healthScript.Brainhealth * 0.2f;

                // check if the health has reached the threshold

                if (_healthScript.brainhealth > yellow_threshold) // green fn
                {
                    image.color = Color.white;
                }
                else if (_healthScript.brainhealth > orange_threshold) // yellow
                {
                    image.color = Color.yellow;
                }
                else if (_healthScript.brainhealth > red_threshold) // orange
                {
                    image.color = new Color(1f, 0.5f, 0f); // orange
                }
                else if (_healthScript.brainhealth <= 0)
                    image.color = Color.red;
                break;

            case HealthScript.BodyParts.Head:

                yellow_threshold = _healthScript.Headhealth * 0.7f;
                orange_threshold = _healthScript.Headhealth * 0.4f;
                red_threshold = _healthScript.Headhealth * 0.2f;

                // check if the health has reached the threshold

                if (_healthScript.headhealth > yellow_threshold) // green fn
                {
                    image.color = Color.white;
                }
                else if (_healthScript.headhealth > orange_threshold) // yellow
                {
                    image.color = Color.yellow;
                }
                else if (_healthScript.headhealth > red_threshold) // orange
                {
                    image.color = new Color(1f, 0.5f, 0f); // orange
                }
                else if (_healthScript.headhealth <= 0)
                    image.color = Color.red;
                break;

            case HealthScript.BodyParts.Torso:

                yellow_threshold = _healthScript.Bodyhealth * 0.7f;
                orange_threshold = _healthScript.Bodyhealth * 0.4f;
                red_threshold = _healthScript.Bodyhealth * 0.2f;

                // check if the health has reached the threshold

                if (_healthScript.bodyhealth > yellow_threshold) // green fn
                {
                    image.color = Color.white;
                }
                else if (_healthScript.bodyhealth > orange_threshold) // yellow
                {
                    image.color = Color.yellow;
                }
                else if (_healthScript.bodyhealth > red_threshold) // orange
                {
                    image.color = new Color(1f, 0.5f, 0f); // orange
                }
                else if (_healthScript.bodyhealth <= 0)
                    image.color = Color.red;
                break;

            case HealthScript.BodyParts.Heart:

                yellow_threshold = _healthScript.Hearthealth * 0.7f;
                orange_threshold = _healthScript.Hearthealth * 0.4f;
                red_threshold = _healthScript.Hearthealth * 0.2f;

                Animator heart = gameObject.GetComponent<Animator>();

                // check if the health has reached the threshold

                if (_healthScript.hearthealth > yellow_threshold) // green fn
                {
                    image.color = Color.white;
                    heart.speed = 1;
                }
                else if (_healthScript.hearthealth > orange_threshold) // yellow
                {
                    image.color = Color.yellow;
                    heart.speed = 4;
                }
                else if (_healthScript.hearthealth > red_threshold) // orange
                {
                    image.color = new Color(1f, 0.5f, 0f); // orange
                    heart.speed = 12;
                }
                else if (_healthScript.hearthealth <= 0)
                {
                    image.color = Color.red;
                    heart.speed = 0;
                }
                break;

            case HealthScript.BodyParts.Lungs:

                yellow_threshold = _healthScript.Lungshealth * 0.7f;
                orange_threshold = _healthScript.Lungshealth * 0.4f;
                red_threshold = _healthScript.Lungshealth * 0.2f;

                Animator lungs = gameObject.GetComponent<Animator>();

                // check if the health has reached the threshold

                if (_healthScript.lungshealth > yellow_threshold) // green fn
                {
                    image.color = Color.white;
                    lungs.speed = 1;
                }
                else if (_healthScript.lungshealth > orange_threshold) // yellow
                {
                    image.color = Color.yellow;
                    lungs.speed = 3;
                }
                else if (_healthScript.lungshealth > red_threshold) // orange
                {
                    image.color = new Color(1f, 0.5f, 0f); // orange
                    lungs.speed = 12;
                }
                else if (_healthScript.lungshealth <= 0)
                {
                    image.color = Color.red;
                    lungs.speed = 0;
                }

                break;
        }
    }
    
}
