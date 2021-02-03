using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlotDrop : MonoBehaviour, IDropHandler
{
    public EquipmentSlot slot;

    bool CanPut(Item item)
    {
        if(slot.slotType == EquipmentSlot.SlotType.Secondary)
        {
            return item.CanBeUsed;
        }

        return slot.CanPut(item.AsEquipment);
    }

    public void OnDrop(PointerEventData data)
    {
        if(InventorySlotDrag.slotBeingDragged != null)
        {
            var otherSlot = InventorySlotDrag.slotBeingDragged.slot;

            if (CanPut(otherSlot.Item.item))
            {
                var oldItem = new ItemBlueprint(otherSlot.Item);

                otherSlot.Item = slot.item;

                if(slot.item == null)
                slot.item = oldItem;
                else
                {
                    slot.item.item = oldItem.item;
                    slot.item.amount = oldItem.amount;
                }

                slot.UpdateSlot();
            }
        }
        else if(EquipmentSlotDrag.slotBeingDragged != null)
        {
            var otherSlot = EquipmentSlotDrag.slotBeingDragged.slot;

            if(slot != otherSlot)
            {
                if(CanPut(otherSlot.item.item) && CanPut(slot.item.item))
                {
                    var oldItem = new ItemBlueprint(slot.item);

                    slot.item = otherSlot.item;
                    otherSlot.item = oldItem;

                    slot.UpdateSlot();
                    otherSlot.UpdateSlot();
                }
            }
        }
    }
}
