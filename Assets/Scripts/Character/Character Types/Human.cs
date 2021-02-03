using UnityEngine;

[RequireComponent(typeof(EquipmentManager))]
public abstract class Human : Character
{
    public EquipmentManager equipmentManager;
    public Inventory inventory;

    public override int Armor { get { return equipmentManager.armor; } }
    public override float Damage { get { return equipmentManager.Weapon != null ? equipmentManager.Weapon.damage : defaultDamage; } }
    public override float AttackRange { get { return equipmentManager.Weapon != null ? equipmentManager.Weapon.attackRange : defaultAttackRange; } }
    public override float AttackAfter { get { return equipmentManager.Weapon != null ? equipmentManager.Weapon.attackAfter : defaultAttackAfter; } }
    public override float AttackSpeed { get { return equipmentManager.Weapon != null ? equipmentManager.Weapon.attackSpeed : defaultAttackSpeed; } }
    public override float MoveSpeed { get { return defaultMoveSpeed + equipmentManager.speedBonus; } }

    protected delegate void OnEquipmentLose(EquipSlot slot);

    protected OnEquipmentLose onWeaponLose;
    protected OnEquipmentLose onArmorLose;

    void Start()
    {
        DelegateReferences();
    }

    protected virtual void DelegateReferences()
    {
        onGetHit += UpdateArmorState;
    }

    protected override void Attack(Character target)
    {
        base.Attack(target);

        if (equipmentManager.Weapon != null)
        {
            equipmentManager[(int)EquipSlot.Weapon].amount--;

            if (equipmentManager[(int)EquipSlot.Weapon].amount == 0)
            {
                equipmentManager.Remove(EquipSlot.Weapon);

                if (onWeaponLose != null) onWeaponLose.Invoke(EquipSlot.Weapon);
            }
        }
    }

    void UpdateArmorState()
    {
        EquipSlot[] statesToAffect = new EquipSlot[] { EquipSlot.Head, EquipSlot.Body, EquipSlot.Legs, EquipSlot.Feet };

        foreach (var state in statesToAffect)
        {
            var item = equipmentManager[state];

            if (item == null) continue;

            item.amount--;

            if (item.amount == 0)
            {
                equipmentManager.Remove(state, false);

                if (onArmorLose != null)
                    onArmorLose.Invoke(state);
            }
        }

        if(equipmentManager.onEquipmentChanged != null)
        equipmentManager.onEquipmentChanged.Invoke();
    }

    void OnValidate()
    {
        if (equipmentManager == null) equipmentManager = GetComponent<EquipmentManager>();
    }
}
