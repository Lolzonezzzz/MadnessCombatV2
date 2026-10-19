using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseTooltip : MonoBehaviour
{
  private Camera _mainCamera;
  [SerializeField] private LayerMask toolTipMask;
  void Start()
  {
    _mainCamera = Camera.main;
  }
  private void Update()
  {
    Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
    RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, toolTipMask);
    if (hit.collider != null)
    {
      ItemPickUp itemData = hit.collider.GetComponent<ItemPickUp>();
      Tooltip.ShowTooltip_Static(itemData.itemData.name);
    }
    else
    {
      Tooltip.HideTooltip_Static();
    }
  }
}
