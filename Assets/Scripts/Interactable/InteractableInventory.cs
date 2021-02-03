using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ItemBlueprintStuff;

public class InteractableInventory : Interactable
{
    Inventory inventory;

    public delegate ItemBlueprint[] GetItemBlueprints();
    public GetItemBlueprints getItemBlueprints;

    public ItemBlueprint[] Items { get { return getItemBlueprints == null ? new ItemBlueprint[0] : getItemBlueprints.Invoke(); } }

    public List<ItemBlueprint> actualItems = new List<ItemBlueprint>();

    [HideInInspector]public bool locked;

    [HideInInspector]
    public float openTime;

    bool opening = false;

    [HideInInspector]public bool initAtAwake = false;
    public JunkData junkData = new JunkData();

    GameMaster master;

    void Awake()
    {
        if (initAtAwake) Apply();
    }

    void Start()
    {
        master = GameMaster.instance;
        inventory = master.interactableInventory;
    }

    public void SetItems(ItemBlueprint[] items)
    {
        getItemBlueprints = () =>
        {
            return items;
        };
    }
    
    protected override void Interaction()
    {
        if (opening) return;

        if (locked)
        {
            StartCoroutine(StartOpening());
        }
        else
        {
            Open();
        }
    }

    IEnumerator StartOpening()
    {
        opening = true;

        yield return new WaitForSeconds(openTime);

        opening = false;

        OpenForFirst();
    }

    void OpenForFirst()
    {
        if(initAtAwake)
        actualItems.AddRange(Items);

        Open();
    }

    void Open()
    {
        master.OpenUI(master.interactableInventoryUI);

        foreach(var item in actualItems)
        {
            bool added = inventory.Add(item);

            if (!added)
            {
                Debug.LogError("Could Not Add " + item.item.name + " With The Amount of " + item.amount);
            }
        }

        inventory.onClosed += OnInventoryClosed;
    }

    void OnInventoryClosed()
    {
        inventory.onClosed -= OnInventoryClosed;

        actualItems.Clear();

        foreach(var item in inventory.items)
        {
            if (item != null && item.item != null && item.amount > 0)
                actualItems.Add(item);
        }
    }

    GetItemBlueprints Apply()
    {
        if (junkData.justPick)
        {
            if (junkData.minMax)
            {
                if (junkData.randomBetween)
                {
                    return () =>
                    {
                        List<ItemBlueprint> list = new List<ItemBlueprint>();

                        list.AddRange(junkData.minMaxPicks.ToItemBlueprintList());
                        list.AddRange(junkData.chanceMinMaxBlueprints.ToItemBlueprintList());

                        return list.ToArray();
                    };
                }
                else
                {
                    return () =>
                    {
                        List<ItemBlueprint> list = new List<ItemBlueprint>();

                        list.AddRange(junkData.minMaxPicks.ToItemBlueprintList());

                        return list.ToArray();
                    };
                }
            }
            else if (junkData.randomBetween)
            {
                return () =>
                {
                    List<ItemBlueprint> list = new List<ItemBlueprint>();

                    list.AddRange(junkData.justPicks);
                    list.AddRange(junkData.chanceItems.ToItemBlueprintList(junkData.minRandomPicks, junkData.maxRandomPicks));

                    return list.ToArray();
                };
            }
            else
            {
                return () =>
                {
                    List<ItemBlueprint> list = new List<ItemBlueprint>();

                    list.AddRange(junkData.justPicks);

                    return list.ToArray();
                };
            }
        }
        else if (junkData.minMax)
        {
            if (junkData.randomBetween)
            {
                return () =>
                {
                    List<ItemBlueprint> list = new List<ItemBlueprint>();

                    list.AddRange(junkData.chanceMinMaxBlueprints.ToItemBlueprintList());

                    return list.ToArray();
                };
            }
            else
            {
                return () =>
                {
                    List<ItemBlueprint> list = new List<ItemBlueprint>();

                    list.AddRange(junkData.minMaxPicks.ToItemBlueprintList());

                    return list.ToArray();
                };
            }
        }
        else if (junkData.randomBetween)
        {
            return () =>
            {
                List<ItemBlueprint> list = new List<ItemBlueprint>();

                list.AddRange(junkData.chanceItems.ToItemBlueprintList(junkData.minRandomPicks, junkData.maxRandomPicks));

                return list.ToArray();
            };
        }

        Debug.LogError("One Of The Options Must Always Be Selected!");
        return null;
    }

    public class JunkData
    {
        public bool justPick, minMax, randomBetween;

        public List<ItemBlueprint> justPicks = new List<ItemBlueprint>();
        public List<MinMaxItemBlueprint> minMaxPicks = new List<MinMaxItemBlueprint>();
        public List<ChanceItemBlueprint> chanceItems = new List<ChanceItemBlueprint>();
        public List<ChanceMinMaxBlueprint> chanceMinMaxBlueprints = new List<ChanceMinMaxBlueprint>();

        public int minRandomPicks, maxRandomPicks;

        public JunkData()
        {
            justPick = minMax = randomBetween = false;
            minRandomPicks = maxRandomPicks = 0;
        }
    }
}
