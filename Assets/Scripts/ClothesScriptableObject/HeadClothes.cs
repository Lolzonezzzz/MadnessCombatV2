using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeadClothes", menuName =  "Create Clothes/Head/ HeadClothes")]
public class HeadClothes : ScriptableObject
{
    public string ClothesName;
    public Sprite left;
    public Sprite right;
    public Sprite front;
    public Sprite back; 
}
