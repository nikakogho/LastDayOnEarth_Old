using UnityEngine;

[CreateAssetMenu(fileName = "New BackPack", menuName = "Inventory/Item/BackPack")]
public class BackPack : Equipment
{
    public int extraSpace;

    [Header("Only For Special Occasions")]
    public float changeMoveSpeedBy = 1;

    public sealed override EquipSlot? NecessarySlot { get { return EquipSlot.BackPack; } }
    public override bool CanTakeLimit { get { return false; } }

    protected override void Equip()
    {
        base.Equip();

        //storage++;
    }
}
