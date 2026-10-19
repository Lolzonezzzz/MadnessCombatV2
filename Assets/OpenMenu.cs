using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OpenMenu : MonoBehaviour
{
    
    [Tooltip("@Select from 1 - child count length")]
    [SerializeField] private int openedUIs = 0;
    [SerializeField] private KeyCode keycodeToOpenMenu = KeyCode.BackQuote;
    private bool _menuOpen = false;
    

    void Update()
    {
        if (Input.GetKeyDown(keycodeToOpenMenu))
        {

            if (_menuOpen)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                     transform.GetChild(i).gameObject.SetActive(false);
                     _menuOpen = false;
                }
            }
            else
            {
                for (int i = 0; i < openedUIs; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                    _menuOpen = true;
                }
            }

        }
    }
    
}


