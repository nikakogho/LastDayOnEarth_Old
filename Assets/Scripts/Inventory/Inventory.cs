using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[RequireComponent(typeof(InventoryUI))]
public class Inventory : MonoBehaviour
{
    InventoryUI ui;

    public int size;
    public ItemBlueprint[] items = new ItemBlueprint[0];

    public delegate void UpdateUI();
    public UpdateUI updateUI;

    public delegate void OnClosed();
    public OnClosed onClosed;

    void Awake()
    {
        InitializeInventoryUI();
    }

    void OnDisable()
    {
        if (onClosed != null) onClosed.Invoke();
    }

    #region Item Methods

    public void SetupItems(ItemBlueprint[] items)
    {
        for(int i = 0; i < items.Length; i++)
        {
            this.items[i].item = items[i].item;
            this.items[i].amount = items[i].amount;
        }

        for(int i = items.Length; i < size; i++)
        {
            this.items[i].item = null;
            this.items[i].amount = 0;
        }

        updateUI.Invoke();
    }

    public void Swap(Inventory inventory, int theirIndex, int ourIndex)
    {
        var theirBlueprint = inventory.items[theirIndex];
        var ourBlueprint = items[ourIndex];

        if(ourBlueprint.item == null)
        {
            ourBlueprint.item = theirBlueprint.item;
            ourBlueprint.amount = theirBlueprint.amount;

            inventory.RemoveAt(theirIndex);
        }
        else if(ourBlueprint.item == theirBlueprint.item)
        {
            int canBeAdded = ourBlueprint.item.MaxInOneSlot - ourBlueprint.amount;

            int toBeSwapped = Mathf.Min(canBeAdded, theirBlueprint.amount);

            ourBlueprint.amount += toBeSwapped;
            theirBlueprint.amount -= toBeSwapped;
        }
        else
        {
            var item = ourBlueprint.item;
            int amount = ourBlueprint.amount;

            ourBlueprint.item = theirBlueprint.item;
            ourBlueprint.amount = theirBlueprint.amount;

            theirBlueprint.item = item;
            theirBlueprint.amount = amount;
        }

        ui.UpdateSlot(ourIndex);
        inventory.ui.UpdateSlot(theirIndex);
    }

    public void RemoveAt(int index)
    {
        items[index].item = null;
        items[index].amount = 0;

        updateUI.Invoke();
    }

    bool CanAdd(ItemBlueprint blueprint, int index = -1)
    {
        if (blueprint == null || blueprint.item == null || blueprint.amount == 0)
        {
            Debug.LogError("Wrong Blueprint!");
            return false;
        }

        if (blueprint.item.IsEquipment)
        {
            if (index > -1) return items[index].item == null;

            foreach(var item in items)
            {
                if (item.item == null) return true;
            }

            return false;
        }
        
        int amountLeft = blueprint.amount;

        if(index > -1)
        {
            return items[index].item == null || (items[index].item == blueprint.item && amountLeft + items[index].amount <= blueprint.item.MaxInOneSlot);
        }
        
        foreach(var item in items)
        {
            if (item.item == null) amountLeft -= blueprint.item.MaxInOneSlot;
            else if (item.item == blueprint.item) amountLeft -= Mathf.Min(item.item.MaxInOneSlot - item.amount, blueprint.amount);

            if (amountLeft <= 0) return true;
        }

        return false;
    }

    public bool Add(Item item, int amount)
    {
        return Add(new ItemBlueprint(item, amount));
    }

    public bool AddRange(List<ItemBlueprint> items)
    {
        throw new System.NotImplementedException("Please Wait");

        /*
        foreach (var blueprint in items) if (!CanAdd(blueprint)) return false;
        
        foreach (var blueprint in items) Add(blueprint);
        
        return true;
        */
    }

    public bool AddRange(ItemBlueprint[] items)
    {
        foreach (var blueprint in items) if (!CanAdd(blueprint)) return false;

        foreach (var blueprint in items) Add(blueprint);

        return true;
    }

    public bool Add(ItemBlueprint blueprint)
    {
        if (!CanAdd(blueprint)) return false;

        if (blueprint.item.IsEquipment)
        {
            foreach (var item in items)
            {
                if (item.item == null)
                {
                    item.item = blueprint.item;
                    item.amount = blueprint.amount;

                    break;
                }
            }
        }
        else
        {
            foreach (var item in items)
            {
                if (item.item == blueprint.item)
                {
                    int canBeAdded = item.item.MaxInOneSlot - item.amount;

                    int toBeAdded = Mathf.Min(canBeAdded, blueprint.amount);

                    item.amount += toBeAdded;
                    blueprint.amount -= toBeAdded;

                    if (blueprint.amount == 0) break;

                    if (blueprint.amount < 0)
                    {
                        Debug.LogError("Blueprint Amount Must Be Non-Negative!");
                    }
                }
            }

            if (blueprint.amount > 0)
            {
                for (int i = 0; i < size; i++)
                {
                    if (items[i].item == null)
                    {
                        items[i].item = blueprint.item;
                        items[i].amount = blueprint.amount;

                        break;
                    }
                }
            }
        }

        if (!invUIInit)
            InitializeInventoryUI();

        updateUI.Invoke();

        return true;
    }

    public bool Add(ItemBlueprint blueprint, int index)
    {
        if (!CanAdd(blueprint, index)) return false;

        if(items[index].item == null)
        {
            items[index] = blueprint;
        }
        else if(items[index].item == blueprint.item && items[index].amount + blueprint.amount <= blueprint.item.MaxInOneSlot)
        {
            items[index].amount += blueprint.amount;
        }

        updateUI.Invoke();

        return true;
    }

    #endregion

    bool invUIInit = false;

    void InitializeInventoryUI()
    {
        if (invUIInit) return;

        invUIInit = true;
        ui = GetComponent<InventoryUI>();
        ui.Init();

        updateUI += ui.UpdateAllSlots;
    }

    void OnValidate()
    {
        if(items.Length != size)
        {
            var old = items;

            items = new ItemBlueprint[size];

            int min = Mathf.Min(size, old.Length);

            for(int i = 0; i < min; i++)
            {
                items[i] = old[i];
            }
        }
    }
}

[System.Serializable]
public class ItemBlueprint : ItemBlueprintStuff.IEditable
{
    public Item item;
    public int amount;

    public void Edit()
    {
        item = EditorGUILayout.ObjectField("Item", item, typeof(Item), false) as Item;
        amount = EditorGUILayout.IntField("Amount", amount);
    }

    public ItemBlueprint()
    {

    }

    public ItemBlueprint(Item item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }

    public ItemBlueprint(ItemBlueprint blueprint) : this(blueprint.item, blueprint.amount)
    {

    }

    public static implicit operator ItemBlueprint(Item item)
    {
        return new ItemBlueprint(item, item.IsEquipment ? item.AsEquipment.CanTake : 1);
    }
}

namespace ItemBlueprintStuff
{
    public interface IEditable
    {
        void Edit();
    }

    [System.Serializable]
    public class MinMaxItemBlueprint : IEditable
    {
        public Item item;
        public int minAmount;
        public int maxAmount;

        public void Edit()
        {
            item = EditorGUILayout.ObjectField("Item", item, typeof(Item), false) as Item;

            minAmount = EditorGUILayout.IntSlider("Min Amount", minAmount, 0, maxAmount);
            maxAmount = EditorGUILayout.IntSlider("Max Amount", maxAmount, minAmount, 200);
        }
    }

    public class ChanceItemBlueprint : IEditable
    {
        public ItemBlueprint blueprint;
        public float chance;

        public void Edit()
        {
            blueprint.Edit();

            chance = EditorGUILayout.FloatField("Chance", chance);
        }

        public ChanceItemBlueprint()
        {
            blueprint = new ItemBlueprint();
        }
    }

    [System.Serializable]
    public class ChanceMinMaxBlueprint : IEditable
    {
        public MinMaxItemBlueprint blueprint;

        [Range(0, 1)]
        public float chance;

        public void Edit()
        {
            blueprint.Edit();

            chance = EditorGUILayout.Slider("Chance", chance, 0, 1);
        }

        public ChanceMinMaxBlueprint()
        {
            blueprint = new MinMaxItemBlueprint();
        }
    }
}