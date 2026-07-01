using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Torso Clothes", menuName =  "Create Clothes/Chest/ Torso Clothes")]
public class TorsoClothes : ScriptableObject
{
    public string clothesName;
    public Sprite left;
    public Sprite right;
    public Sprite front;
    public Sprite back; 
}
