using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class InventoryUI : MonoBehaviour
{
    Inventory inventory;

    public Transform slotParent;
    public GameObject slotPrefab;

    InventorySlot[] slots;

    public void Init()
    {
        inventory = GetComponent<Inventory>();

        slots = new InventorySlot[inventory.size];

        for(int i = 0; i < slots.Length; i++)
        {
            var slot = Instantiate(slotPrefab, transform.position, Quaternion.identity, slotParent).GetComponent<InventorySlot>();

            slot.Setup(inventory, i);
            slot.UpdateUI();

            slots[i] = slot;
        }
    }

    public void UpdateSlot(int index)
    {
        slots[index].Setup(inventory, index);
        slots[index].UpdateUI();
    }

    public void UpdateAllSlots()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i].Setup(inventory, i);
            slots[i].UpdateUI();
        }
    }
}
