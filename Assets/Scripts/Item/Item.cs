using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item/Item")]
public class Item : ScriptableObject
{
    new public string name;
    public Sprite icon;

    public enum MaxInOneSlotEnum { One = 1, Twenty = 20, Fifty = 50, Max = int.MaxValue }
    [HideInInspector]public MaxInOneSlotEnum maxInOneSlot = MaxInOneSlotEnum.Twenty;

    public virtual bool IsEquipment { get { return false; } }
    public Equipment AsEquipment { get { return IsEquipment ? this as Equipment : null; } }

    public virtual int MaxInOneSlot { get { return maxInOneSlot.ToInt(); } }

    #region Use

    public virtual bool CanBeUsed { get { return false; } }
    protected virtual string UseTitle { get { return "USE"; } }
    protected virtual void UseMethod() { }

    public bool Use()
    {
        if (CanBeUsed)
        {
            UseMethod();

            return true;
        }

        return false;
    }

    #endregion
}
