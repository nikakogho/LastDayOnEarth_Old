using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlotDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public EquipmentSlot slot;
    public static EquipmentSlotDrag slotBeingDragged = null;

    Vector3 startPos;

    void Awake()
    {
        startPos = transform.localPosition;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        if (slot.item != null && slot.item.item != null)
            slotBeingDragged = this;
    }

    public void OnDrag(PointerEventData data)
    {
        if (slot.item != null && slot.item.item != null)
            transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData data)
    {
        slotBeingDragged = null;

        transform.localPosition = startPos;
    }
}
