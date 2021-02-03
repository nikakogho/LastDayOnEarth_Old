using UnityEngine;
using System.Collections.Generic;

public class HumanBot : Human
{
    public GameObject remain;
    public bool runningAway = false;
    Vector3? targetPosition = null;

    public static Vector2 minPos, maxPos;

    public float seeAngle;

    [Range(8, 100)]
    public int bravery;

    protected override void AwakeStuff()
    {
        base.AwakeStuff();

        InvokeRepeating("UpdateTarget", 0.2f, 0.4f);
        InvokeRepeating("LogicUpdate", 0.4f, 0.2f);
    }

    void UpdateTarget()
    {
        if (target == null) GetTarget(AttackRange + 2, seeAngle, targetMask);
    }

    void LogicUpdate()
    {
        if(target == null)
        {
            if (motor.target != null) motor.StopMoving();

            if(targetPosition == null)
            {
                targetPosition = new Vector3(Random.Range(minPos.x, maxPos.x), 0, Random.Range(minPos.y, maxPos.y));
                motor.MoveAt(targetPosition.Value);
            }
            else
            {
                if(Vector3.Distance(transform.position, targetPosition.Value) < 0.1f)
                {
                    targetPosition = null;
                }
            }
        }
        else
        {
            if (targetPosition != null) targetPosition = null;

            if (runningAway)
            {
                motor.RunAwayFrom(target, target.AttackRange + 20);
            }
            else
            {
                motor.Chase(target);

                if(Vector3.Distance(transform.position, target.transform.position) <= AttackRange)
                {
                    if(attackCountdown <= 0)
                    {
                        Attack(target);
                    }
                }

                if (Random.Range(0, bravery) < health) runningAway = true;
            }
        }
    }

    protected override void DelegateReferences()
    {
        onWeaponLose += OnWeaponLose;
        onArmorLose += OnArmorLose;
        
        base.DelegateReferences();
    }
    
    void OnWeaponLose(EquipSlot weaponSlot)
    {
        float bestFitness = 0;

        int index = 0;

        int chosenIndex = -1;

        foreach(var blueprint in inventory.items)
        {
            var item = blueprint.item;

            if (item.IsEquipment)
            {
                var equipment = item.AsEquipment;

                if(equipment.EquipSlot == weaponSlot)
                {
                    var weapon = equipment as Tool;

                    float fitness = weapon.damage * weapon.attackSpeed;

                    if(fitness > bestFitness)
                    {
                        bestFitness = fitness;
                        chosenIndex = index;
                    }
                }
            }

            index++;
        }

        if(chosenIndex > -1)
        {
            var item = inventory.items[chosenIndex];

            inventory.RemoveAt(chosenIndex);
            equipmentManager.Equip(item);
        }
    }

    void OnArmorLose(EquipSlot slot)
    {
        float bestFitness = 0;
        int chosenIndex = -1;

        for(int i = 0; i < inventory.items.Length; i++)
        {
            var blueprint = inventory.items[i];
            var item = blueprint.item;

            var equipment = item.AsEquipment;

            if (equipment == null) continue;
            if (equipment.EquipSlot != slot) continue;

            float fitness = equipment.Armor + equipment.speedBonus;

            if(fitness > bestFitness)
            {
                bestFitness = fitness;
                chosenIndex = i;
            }
        }

        if(chosenIndex > -1)
        {
            var item = inventory.items[chosenIndex];

            inventory.RemoveAt(chosenIndex);
            equipmentManager.Equip(item);
        }
    }

    protected override void Die()
    {
        var clone = Instantiate(remain, transform.position, transform.rotation);
        var interactableInventory = clone.GetComponent<InteractableInventory>();

        List<ItemBlueprint> items = new List<ItemBlueprint>();

        items.AddRange(inventory.items);

        for(int i = 0; i < EquipmentManager.Size; i++)
        {
            var blueprint = equipmentManager[i];

            if(blueprint != null && blueprint.amount > 0 && blueprint.item != null)
            {
                items.Add(blueprint);
            }
        }

        interactableInventory.SetItems(items.ToArray());

        base.Die();
    }
}
