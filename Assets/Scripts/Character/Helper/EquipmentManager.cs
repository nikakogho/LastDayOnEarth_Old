using UnityEngine;

[RequireComponent(typeof(Character))]
public class EquipmentManager : MonoBehaviour
{
    public static int Size { get { return System.Enum.GetNames(typeof(EquipSlot)).Length; } }

    ItemBlueprint[] equipment = new ItemBlueprint[Size];
    [SerializeField]Transform[] placeHolders = new Transform[Size];
    GameObject[] equippedMeshes = new GameObject[Size];

    public delegate void OnEquipmentChanged();
    public OnEquipmentChanged onEquipmentChanged;

    public int armor { get; private set; }
    public float speedBonus { get; private set; }

    public Tool Weapon
    {
        get
        {
            var blueprint = this[EquipSlot.Weapon];

            if (blueprint == null) return null;

            var item = blueprint.item;

            return item == null ? null : item as Tool;
        }
    }

    public ItemBlueprint this[int index] { get { return equipment[index]; } }
    public ItemBlueprint this[EquipSlot slot] { get { return this[(int)slot]; } }

    public RuntimeAnimatorController defaultController;

    Animator anim;

    void Awake()
    {
        onEquipmentChanged += ChangeAnims;

        anim = GetComponent<Animator>();

        anim.runtimeAnimatorController = defaultController;
    }

    void ChangeAnims()
    {
        var weapon = Weapon;

        if(weapon == null)
        {
            anim.runtimeAnimatorController = defaultController;
        }
        else
        {
            anim.runtimeAnimatorController = weapon.animatorController;
        }
    }
    
    public void Equip(ItemBlueprint item)
    {
        if (!item.item.IsEquipment)
        {
            Debug.LogError("Only Equipment Can Be Applied To The Equipment Manager!");
        }

        var equipment = item.item.AsEquipment;

        EquipSlot slot = equipment.EquipSlot;
        int index = (int)slot;

        var oldItem = this.equipment[index];

        if (oldItem != null) UnEquip(slot, false);

        this.equipment[index] = item;

        var placeHolder = placeHolders[index];

        var clone = Instantiate(equipment.prefab, placeHolder.position, placeHolder.rotation, placeHolder);

        clone.transform.localPosition = equipment.offsetPosition;
        clone.transform.localRotation = Quaternion.Euler(equipment.offsetRotation);
        clone.transform.localScale = equipment.offsetScale;

        equippedMeshes[index] = clone;

        armor += equipment.Armor;
        speedBonus += equipment.speedBonus;

        if (onEquipmentChanged != null) onEquipmentChanged.Invoke();
    }

    public void Remove(EquipSlot slot, bool justThis = true)
    {
        int index = (int)slot;

        var item = equipment[index];

        if (item == null) return;

        armor -= item.item.AsEquipment.Armor;
        speedBonus -= item.item.AsEquipment.speedBonus;

        Destroy(equippedMeshes[index]);
        equipment[index] = null;

        if (justThis)
            if (onEquipmentChanged != null) onEquipmentChanged.Invoke();
    }

    public void UnEquip(EquipSlot slot, bool justThis = true)
    {
        int index = (int)slot;

        var item = equipment[index];

        if (item == null) return;

        //inventory.Add(item);

        Remove(slot, justThis);
    }

    public void UnEquipAll()
    {
        for(int i = 0; i < Size; i++)
        {
            UnEquip((EquipSlot)i, false);
        }

        if (onEquipmentChanged != null) onEquipmentChanged.Invoke();
    }
}
