using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Inventory/Item/Tool")]
public class Tool : Equipment
{
    public float damage, attackSpeed, attackRange, attackAfter;

    public RuntimeAnimatorController animatorController;

    public bool canRun = true;

    [Header("Only For Special Occasions")]
    public float changeMoveSpeedBy = 1;

    public sealed override EquipSlot? NecessarySlot { get { return EquipSlot.Weapon; } }

    public enum ToolType { Weapon, Breaker }
    [HideInInspector]public ToolType toolType = ToolType.Weapon;

    [HideInInspector]
    public int[] breaks;
}
