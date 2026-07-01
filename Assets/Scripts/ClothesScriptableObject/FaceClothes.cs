using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Face Clothes", menuName =  "Create Clothes/Head/Face Clothes")]
public class FaceClothes : ScriptableObject
{
    public string ClothesName;
    public Sprite left;
    public Sprite right;
    public Sprite front;
    public Sprite back; 
}
