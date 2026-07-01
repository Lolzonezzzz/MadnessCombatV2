using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playSound : MonoBehaviour
{
    [SerializeField] private AudioClip sounds;



    public void PLaySound()
    {
        AudioSource.PlayClipAtPoint(sounds, transform.position);
    }
}
