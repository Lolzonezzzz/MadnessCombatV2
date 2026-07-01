using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXAssitance : MonoBehaviour
{
    [SerializeField]private ItemData itemData;
    [SerializeField]private int reloadpoint;

    [SerializeField]private GameObject magobject;
    [SerializeField]private float fallingcount;
    private Rigidbody2D rb;

    public void PlayReloadSound()
    {
        AudioSource.PlayClipAtPoint(itemData.reloadSound[reloadpoint], transform.position, 5f);
    }

 
    public void DropMag()
    {
        GameObject createmag = Instantiate(magobject, transform.position, transform.rotation);

        rb = createmag.AddComponent<Rigidbody2D>();
        rb.velocity = Vector2.one;
        rb.gravityScale = 5f;
        createmag.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        createmag.GetComponent<SpriteRenderer>().sortingOrder = 1;

        StartCoroutine(StopMag());
        Destroy(createmag, 3f);
    }


    IEnumerator StopMag()
    {
        yield return new WaitForSeconds(fallingcount);

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.angularVelocity = 0f;
    }


}
