using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chest Armour", menuName =  "Create Clothes/Chest/ Chest Armour")]
public class ChestArmour : ScriptableObject
{
    public string clothesName;
    public Sprite left;
    public Sprite right;
    public Sprite front;
    public Sprite back; 
}
