using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public enum SlotType { Head, Body, Legs, Feet, Primary, BackPack, Secondary }
    public SlotType slotType;

    public Image icon;
    
    public Image amountImage;

    public ItemBlueprint item;

    void Start()
    {
        ApplyUI();
    }
    
    public bool CanPut(Equipment item)
    {
        if (item == null) return false;

        switch (slotType)
        {
            case SlotType.Head:
                return item.EquipSlot == EquipSlot.Head;
            case SlotType.Body:
                return item.EquipSlot == EquipSlot.Body;
            case SlotType.Legs:
                return item.EquipSlot == EquipSlot.Legs;
            case SlotType.Feet:
                return item.EquipSlot == EquipSlot.Feet;
            case SlotType.Primary:
                return item.EquipSlot == EquipSlot.Weapon;
            case SlotType.BackPack:
                return item.EquipSlot == EquipSlot.BackPack;
            case SlotType.Secondary:
                return item.CanBeUsed;
        }

        return false;
    }

    void ApplyUI()
    {
        if(item == null || item.item == null)
        {
            icon.sprite = GameMaster.instance.defaultPlayerEquipmentIcons[(int)slotType];
            amountImage.enabled = false;
            return;
        }

        icon.sprite = item.item.icon;

        if (item.item.AsEquipment.EquipSlot == EquipSlot.BackPack)
        {
            amountImage.enabled = false;
            return;
        }

        amountImage.enabled = true;

        float value = (float)item.amount / item.item.AsEquipment.CanTake;

        amountImage.fillAmount = value;
        amountImage.color = Color.Lerp(Color.red, Color.green, value);
    }

    public void UpdateSlot()
    {
        if (slotType != SlotType.Secondary)
        {
            var equipmentManager = Player.instance.equipmentManager;

            EquipSlot slot;

            switch (slotType)
            {
                case SlotType.Head: slot = EquipSlot.Head;
                    break;
                case SlotType.Body: slot = EquipSlot.Body;
                    break;
                case SlotType.Legs: slot = EquipSlot.Legs;
                    break;
                case SlotType.Feet: slot = EquipSlot.Feet;
                    break;
                case SlotType.Primary: slot = EquipSlot.Weapon;
                    break;
                case SlotType.BackPack:
                    slot = EquipSlot.BackPack;
                    break;
                default:
                    return;
            }

            if (item == null || item.item == null) equipmentManager.UnEquip(slot);
            else equipmentManager.Equip(item);
        }

        ApplyUI();
    }

    void Apply(ItemBlueprint item)
    {
        this.item = item;

        ApplyUI();
    }

    public bool Put(ItemBlueprint item)
    {
        if (!item.item.IsEquipment) return false;
        if (!CanPut(item.item.AsEquipment)) return false;

        Apply(item);

        return true;
    }
}
