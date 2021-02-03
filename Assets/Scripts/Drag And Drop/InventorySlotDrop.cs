using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotDrop : MonoBehaviour, IDropHandler
{
    public InventorySlot slot;
    public bool BelongsToPlayerInventory { get { return slot.inventory == Player.instance.inventory; } }

    public void OnDrop(PointerEventData data)
    {
        if(InventorySlotDrag.slotBeingDragged != null)
        {
            //Debug.Log("DRAGGED TO INDEX " + slot.Index);

            var otherSlot = InventorySlotDrag.slotBeingDragged.slot;

            if(slot != otherSlot)
            {
                var oldItem = new ItemBlueprint(slot.Item);

                slot.Item = new ItemBlueprint(otherSlot.Item);
                otherSlot.Item = oldItem;
                
                slot.inventory.updateUI.Invoke();

                if (slot.inventory != otherSlot.inventory) otherSlot.inventory.updateUI.Invoke();
            }
        }
        else if(EquipmentSlotDrag.slotBeingDragged != null)
        {
            var otherSlot = EquipmentSlotDrag.slotBeingDragged.slot;

            Debug.Log(otherSlot.item.item.name);

            if (otherSlot.CanPut(slot.Item.item.AsEquipment))
            {
                var oldItem = new ItemBlueprint(slot.Item);

                slot.Item = otherSlot.item;
                otherSlot.item = oldItem;

                otherSlot.UpdateSlot();
            }
        }
    }
}
