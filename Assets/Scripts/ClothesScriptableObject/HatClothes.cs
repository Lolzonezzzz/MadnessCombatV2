using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HatClothes", menuName =  "Create Clothes/Head/ HatClothes")]
public class HatClothes : ScriptableObject
{
    public string ClothesName;
    public Sprite left;
    public Sprite right;
    public Sprite front;
    public Sprite back; 
}
