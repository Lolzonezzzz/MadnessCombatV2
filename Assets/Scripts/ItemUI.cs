using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    private Image _image;
    private TMP_Text _text;
    public ItemData item;



    private void Start()
    {
        _image = GetComponentInChildren<Image>();
        _text = GetComponentInChildren<TMP_Text>();
        
        
        
        _image.sprite = item.icon;
        _text.text = item.name;
    }

    public void GiveItemToPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Inventory inventoryUI = player.GetComponentInChildren<Inventory>();
            
            inventoryUI.AddItem(item);
        }
    }
}
