using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;

    public Text amountText;
    public Image amountImage;

    [HideInInspector]public Inventory inventory;
    int index;

    public int Index { get { return index; } }

    public ItemBlueprint Item { get { return inventory.items[index]; } set { inventory.items[index].item = value.item; inventory.items[index].amount = value.amount; UpdateUI(); } }

    public void Setup(Inventory inventory, int index)
    {
        this.inventory = inventory;
        this.index = index;
    }

    public void UpdateUI()
    {
        var blueprint = inventory.items[index];
        var item = blueprint.item;

        bool hasItem = item != null;

        icon.sprite = hasItem ? blueprint.item.icon : GameMaster.instance.defaultInventorySlotIcon;

        if (hasItem)
        {
            amountText.enabled = !(amountImage.enabled = item.IsEquipment);
        }
        else
        {
            amountText.enabled = amountImage.enabled = false;
        }

        if(hasItem)
        {
            if (item.IsEquipment)
            {
                float value = (float)blueprint.amount / item.MaxInOneSlot;

                amountImage.fillAmount = value;
                amountImage.color = Color.Lerp(Color.red, Color.green, value);
            }
            else
            {
                amountText.text = blueprint.amount.ToString();
            }
        }
    }
}
