using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxLootBox : MonoBehaviour
{
    [SerializeField] private GameObject debrisGO;
    [SerializeField] private Sprite[] debris;

    [SerializeField] private float healthpoint = 100f;
    [SerializeField] private LootTable LootTable;
    [SerializeField] private float rollDistanceMin = 2f, rollDistanceMax = 4f;
    [SerializeField] private float rollForce = 8f;
    private float health;
    // Start is called before the first frame update
    void Start()
    {
        health = healthpoint;
    }

    private IEnumerator StopRoll(Rigidbody2D rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!rb) yield break;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Static;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            float rand = Random.Range(0, 10);
            if (rand == 0)
            {
                     ItemData itemData = LootTable.GetDrop();
                
                GameObject gunmodel = Instantiate(itemData.gunModel, transform.position, Quaternion.identity);
                Rigidbody2D gunrb = gunmodel.GetComponent<Rigidbody2D>();
                
                gunrb.bodyType = RigidbodyType2D.Dynamic;
                gunrb.gravityScale = 0f;
                gunrb.drag = 2.5f;
                gunrb.angularDrag = 2.5f;
                gunrb.constraints = RigidbodyConstraints2D.None;
                
                float GunAng = Random.Range(0f, 360f);
                Vector2 GunDir = new Vector2(Mathf.Cos(GunAng * Mathf.Deg2Rad), Mathf.Sin(GunAng * Mathf.Deg2Rad));
                float GunDist = Random.Range(rollDistanceMin, rollDistanceMax);
                gunrb.AddForce(GunDir * rollForce * GunDist, ForceMode2D.Impulse);
                gunrb.angularVelocity = Random.Range(-720f, 720f);
            
                gunmodel.GetComponent<ItemPickUp>().enabled = true;
                gunmodel.GetComponent<Animator>().enabled = false;

            }
            for (int i = 0; i < debris.Length; i++)
            {
                    
                GameObject debis = Instantiate(debrisGO, transform.position, Quaternion.identity);
                debis.GetComponent<SpriteRenderer>().sprite = debris[i];
                    
                Rigidbody2D rb = debis.GetComponent<Rigidbody2D>();
                if (!rb) rb = debis.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 0f;
                rb.drag = 2.5f;
                rb.angularDrag = 2.5f;
                rb.constraints = RigidbodyConstraints2D.None;

                float ang = Random.Range(0f, 360f);
                Vector2 dir = new Vector2(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad));
                float dist = Random.Range(rollDistanceMin, rollDistanceMax);
                rb.AddForce(dir * rollForce * dist, ForceMode2D.Impulse);
                rb.angularVelocity = Random.Range(-720f, 720f);
                GetComponent<SpriteRenderer>().enabled = false;
                StartCoroutine(StopRoll(rb, 0.8f));
                debis.isStatic = true;
                Destroy(gameObject); 
            }
        }
    }
}
