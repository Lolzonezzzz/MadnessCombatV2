using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData itemData;
    [SerializeField] private GameObject[] hands;

    public void PickUp(InventoryUI inventoryUI)
    {
        if (inventoryUI == null) return;
        inventoryUI.items.Add(itemData);
        inventoryUI.ammo.Add(itemData.ammoCapacity);
        inventoryUI.ammoreserve.Add(itemData.currentAmmoReserve);
        inventoryUI.EquipNew();
        Destroy(gameObject);
    }

    void Update()
    {
        if (enabled)
        {
            foreach (var hand in hands)
            {
                hand.SetActive(false);
            }
        }
    }
}
