using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeUI : MonoBehaviour
{
    [SerializeField]private TMPro.TextMeshProUGUI text;
    [TextArea(15, 10)]
    public string UIText;
    
    [SerializeField]private RectTransform UIRect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && text != null)
        {
            text.text = UIText;
            
        }

        if (other.CompareTag("Player") && UIRect != null)
        {
            UIRect.gameObject.SetActive(true);
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && text != null)
        {
            text.text = null;
        }

        if (other.CompareTag("Player") && UIRect != null)
        {
            UIRect.gameObject.SetActive(false);
        }
    }
}
