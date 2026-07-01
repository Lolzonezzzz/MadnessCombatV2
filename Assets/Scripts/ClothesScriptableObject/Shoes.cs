using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shoes", menuName =  "Create Clothes/ Shoes")]
public class Shoes : ScriptableObject
{
    public string shoesName;
    public Sprite left;
    public Sprite right;
    public Sprite front;
    public Sprite back; 
}
