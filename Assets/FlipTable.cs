using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipTable : MonoBehaviour
{
  [SerializeField] private Sprite flippedLeft,
    flippedRight,
    flippedUp,
    flippedDown;
  
  
  [NonSerialized]
  public bool nearToPlayer;

  [NonSerialized]
  public FlipState filpstate;


  private void Update()
  {
    if (nearToPlayer && Input.GetKeyDown(KeyCode.Space))
    {
      Flip(filpstate);
    }
  }

  void Flip(FlipState state)
  {
    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    
    rb.bodyType = RigidbodyType2D.Dynamic;
    Vector2 dir;
    switch (state)
    {
      case FlipState.FlippedDown:
        GetComponent<SpriteRenderer>().sprite = flippedDown;
        dir = Quaternion.identity * Vector2.down;
        rb.AddForce(dir * 15f, ForceMode2D.Impulse);
        rb.angularVelocity = 720f;
        rb.drag = 4f;         // = linearDamping in Unity 6+
        rb.angularDrag = 3f;
        return;
      
      case FlipState.FlippedUp:
        GetComponent<SpriteRenderer>().sprite = flippedUp;
        dir = Quaternion.identity * Vector2.up;
        rb.AddForce(dir * 15f, ForceMode2D.Impulse);
        rb.angularVelocity = 720f;
        rb.drag = 4f;       
        rb.angularDrag = 3f;
        return;
      
      case FlipState.FlippedLeft:
        GetComponent<SpriteRenderer>().sprite = flippedLeft;
        dir = Quaternion.identity * Vector2.right;
        rb.AddForce(dir * 15f, ForceMode2D.Impulse);
        rb.angularVelocity = 720f;
        rb.drag = 4f;
        rb.angularDrag = 3f;
        return;
      
      case FlipState.FlippedRight:
        GetComponent<SpriteRenderer>().sprite = flippedRight;
        dir = Quaternion.identity * Vector2.right;
        rb.AddForce(dir * 15f, ForceMode2D.Impulse);
        rb.angularVelocity = 720f;
        rb.drag = 4f;
        rb.angularDrag = 3f;
        return;
    }
    StartCoroutine(StopSlide(rb, 0.6f));
  }
  
  IEnumerator StopSlide(Rigidbody2D rb, float delay)
  {
    yield return new WaitForSeconds(delay);
    if (!rb) yield break;
    rb.velocity = Vector2.zero;
    rb.angularVelocity = 0f;
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