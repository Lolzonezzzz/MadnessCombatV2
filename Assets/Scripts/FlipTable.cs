using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlipTable : MonoBehaviour
{
  [SerializeField] private Sprite flippedLeft,
    flippedRight,
    flippedUp,
    flippedDown;

  [SerializeField] private float flipforce;
  
  [NonSerialized]
  public bool nearToPlayer;

  [NonSerialized]
  public FlipState filpstate;


  private void Update()
  {
    if (nearToPlayer && Input.GetKeyDown(KeyCode.Space))
    {
      Flip(filpstate);
      StartCoroutine(changecolider());
    }
  }

  IEnumerator changecolider()
  {
    Destroy(GetComponent<BoxCollider2D>());
    yield return new WaitForSeconds(0.1f);
    gameObject.AddComponent<BoxCollider2D>();
    BoxCollider2D bc = GetComponent<BoxCollider2D>();
  }
  void Flip(FlipState state)
  {
    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    rb.bodyType = RigidbodyType2D.Dynamic;
    Vector2 dir = default;
    GameObject[] thingsontable = new GameObject[transform.childCount];
    for (int i = 0; i < transform.childCount; i++)
      thingsontable[i] = transform.GetChild(i).gameObject;

    var filtered = thingsontable.Where(go => go.CompareTag("tablestuff")).ToArray();
 
    switch (state)
    {
      case FlipState.FlippedDown:
        GetComponent<SpriteRenderer>().sprite = flippedDown;
        dir = Quaternion.identity * Vector2.down;
        rb.AddForce(dir * flipforce, ForceMode2D.Impulse);
        rb.angularVelocity = 720f;
        rb.drag = 3f;         // = linearDamping in Unity 6+
        rb.angularDrag = 3f;
        break;
      
      case FlipState.FlippedUp:
        GetComponent<SpriteRenderer>().sprite = flippedUp;
        dir = Quaternion.identity * Vector2.up;
        rb.AddForce(dir * flipforce, ForceMode2D.Impulse);
        rb.angularVelocity = 720f;
        rb.drag = 3f;       
        rb.angularDrag = 3f;
        break;
      
      case FlipState.FlippedLeft:
        GetComponent<SpriteRenderer>().sprite = flippedLeft;
        dir = Quaternion.identity * Vector2.left;
        rb.AddForce(dir * flipforce, ForceMode2D.Impulse);
        rb.angularVelocity = 720f;
        rb.drag = 3f;
        rb.angularDrag = 3f;
        break;
      
      case FlipState.FlippedRight:
        GetComponent<SpriteRenderer>().sprite = flippedRight;
        dir = Quaternion.identity * Vector2.right;
        rb.AddForce(dir * flipforce, ForceMode2D.Impulse);
        rb.angularVelocity = 720f;
        rb.drag = 3f;
        rb.angularDrag = 3f;
        break;
    }
    
    float baseAng = filpstate switch {
      FlipState.FlippedRight => 0f,
      FlipState.FlippedUp => 90f,
      FlipState.FlippedLeft => 180f,
      FlipState.FlippedDown => 270f,
      _ => 0f
    };
    
    for (int i = 0; i < filtered.Length; i++)
    {
      GameObject itemOnTable =  Instantiate(filtered[i], filtered[i].transform.position, transform.rotation);
      Rigidbody2D objRb = itemOnTable.AddComponent<Rigidbody2D>();
      objRb.bodyType = RigidbodyType2D.Dynamic;
      objRb.gravityScale = 0f;
      objRb.drag = 3f;
      objRb.angularDrag = 3f;

      itemOnTable.transform.localScale = filtered[i].transform.lossyScale;
      float ang = baseAng + Random.Range(-10f, 10f); // ±10° scatter
      Quaternion rot = Quaternion.Euler(0f, 0f, ang);
      dir = new Vector2(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad));
      
      objRb.AddForce(dir.normalized * flipforce / 2, ForceMode2D.Impulse);
      objRb.angularVelocity = 720f;
      
      Destroy(filtered[i]);
      itemOnTable.isStatic = true;
    }


    
    
  }
  

  
}

public enum FlipState
{
  None,
  FlippedLeft,
  FlippedRight,
  FlippedUp,
  FlippedDown
}