using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventorySlot slot;

    public static InventorySlotDrag slotBeingDragged = null;

    Vector3 startPos;

    void Awake()
    {
        startPos = transform.localPosition;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        if(slot.Item != null && slot.Item.item != null)
        slotBeingDragged = this;
    }

    public void OnDrag(PointerEventData data)
    {
        if (slot.Item != null && slot.Item.item != null)
            transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData data)
    {
        transform.localPosition = startPos;

        slotBeingDragged = null;
    }
}
