using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();
    public List<int> ammo = new List<int>();
    public List<int> ammoreserve = new List<int>();

    private AimAndShoot _aimAndShoot;
    public int itemCount = 0;

    void Start()
    {
        _aimAndShoot = gameObject.GetComponentInChildren<AimAndShoot>();
    }

    public void EquipNew()
    {
        if (items.Count > 0)
        {
            itemCount = items.Count - 1;
            _aimAndShoot.WeaponSwitching(items[itemCount]);
        }
    }

    private void Update()
    {
        if (items.Count == 0) return;

        float scroll = Input.mouseScrollDelta.y;

        if (scroll > 0)
        {
            itemCount = (itemCount + 1) % items.Count;
            _aimAndShoot.WeaponSwitching(items[itemCount]);
        }
        else if (scroll < 0)
        {
            itemCount--;
            if (itemCount < 0) itemCount = items.Count - 1;
            _aimAndShoot.WeaponSwitching(items[itemCount]);
        }
    }

    public void AddItem(ItemData item)
    {
        items.Add(item);
        ammo.Add(items.Capacity);
        ammoreserve.Add(999999999);
        EquipNew();
    }
}
