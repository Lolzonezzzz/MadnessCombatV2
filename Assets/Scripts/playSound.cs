using System.Collections;
using UnityEngine;

public class playSound : MonoBehaviour
{
    public AudioClip[] sounds;
    public bool canPlaySound = true;

    private AudioSource _soundSource;
    public int index;
    
    public IEnumerator PLaySound(int index1, float delay)
    {
        index = index1;
        if (sounds.Length == 0) yield break;
        _soundSource.PlayOneShot(sounds[index1]);
        canPlaySound = false;
        yield return new WaitForSeconds(delay);
        Invoke(nameof(ResetSound), sounds[index1].length);
    }
    
     
    void ResetSound()
    {
        canPlaySound = true;
    }

    void Start()
    {
        _soundSource = GetComponent<AudioSource>();
    }
}
