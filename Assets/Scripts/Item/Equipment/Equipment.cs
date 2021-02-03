using UnityEngine;

public enum EquipSlot { Head, Body, Legs, Feet, Weapon, BackPack }

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Item/Equipment")]
public class Equipment : Item
{
    EquipSlot _equipSlot;
    int armor;
    int canTake;

    public GameObject prefab;

    public bool attachToSkin = false;

    [HideInInspector]public Vector3 offsetPosition, offsetRotation, offsetScale = Vector3.one;

    public virtual EquipSlot? NecessarySlot { get { return null; } }

    public virtual bool CanTakeLimit { get { return true; } }

    public int Armor { get { return NecessarySlot != null ? 0 : armor; } set { armor = value; } }
    public int CanTake { get { return CanTakeLimit ? canTake : 1; } set { canTake = value; } }
    public EquipSlot EquipSlot { get { return NecessarySlot != null ? NecessarySlot.Value : _equipSlot; } set { _equipSlot = value; } }
    
    public sealed override bool CanBeUsed { get { return true; } }
    protected override string UseTitle { get { return "EQUIP"; } }
    public override int MaxInOneSlot { get { return CanTake; } }
    public override bool IsEquipment { get { return true; } }

    [HideInInspector]public int speedBonus;

    protected sealed override void UseMethod()
    {
        Equip();
    }

    protected virtual void Equip()
    {
        Player.instance.equipmentManager.Equip(this);
    }
}
