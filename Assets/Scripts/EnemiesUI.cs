using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemiesUI : MonoBehaviour
{
    private Image _image;
    private TMP_Text _text;
    [SerializeField]private TMP_Text catagoryText;
    public GameObject enemy;



    private void Start()
    {
        _image = GetComponentInChildren<Image>();
        _text = GetComponentInChildren<TMP_Text>();





        catagoryText.text = "Enemy";
        _text.text = enemy.name;
    }

    public void CreateEnemy()
    {
        Instantiate(enemy, new Vector3(0, 0, 0), Quaternion.identity);
    }
}