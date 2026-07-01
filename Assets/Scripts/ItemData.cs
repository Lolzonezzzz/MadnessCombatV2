using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu (menuName = "Create a weapon", fileName = "ItemData")]
public class ItemData : ScriptableObject
{
    public string itemname;
    public WeaponTypes weaponTypes;
    [Header("Gunz Settings")]
    public float damage = 5;
    public float range = 40;
    public float fireRate = 1;
    public float bulletSpeed = 20;
    public int bulletPirecing = 1;
    public StatusEffects statusEffectOnHit;
    public int statusEffectCooldown;
    public int burstCount = 1;
    
    [Header("Shotgun Settings")]
    public int bulletsPerShot = 1;
    public int bulletSpread = 5;

    [Header("Explosives Settings")] 
    public int explosionRadius;
    
    [Header("Reload Settings")]
    public int ammoCapacity = 6;
    public int currentAmmoReserve = 100;
    public int gunReloadSpeed = 1;
    
    [Header("Icons,GunModel and Sounds")]
    public Sprite icon;
    public GameObject gunModel;
    public GameObject shellEject;
    public AudioClip equipSound;
    public AudioClip firingSound;
    public AudioClip[] reloadSound;

    [Header("Tetris Inventory Stats")] 
    public int height;
    public int width;
    public bool rotating = true;
    
    public enum StatusEffects
    {
        None,
        Bleed_I,
        Bleed_II,
        Bleed_III,
        Poision,
        Poision_II,
        Poision_III,
        Shock,
        Concussion,
        Rupture,
        Fatique,
        Freezing,
        Fear_I,
        Fear_II,
        Fear_III,
    }

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
        
    }
    
    
    public enum WeaponTypes { Pistols,  Rifles, Shotguns, MachineGuns, Explosives, Knives, Axes, Machetes, Saws, Hands, }

}
