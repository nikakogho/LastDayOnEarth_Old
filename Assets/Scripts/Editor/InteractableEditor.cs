using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using ItemBlueprintStuff;

[CustomEditor(typeof(Interactable))]
public class InteractableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var interactable = target as Interactable;

        interactable.seeRange = EditorGUILayout.FloatField("See Range", interactable.seeRange);
        interactable.interactRange = EditorGUILayout.Slider("Interact Range", interactable.interactRange, 0, interactable.seeRange);
    }
}

[CustomEditor(typeof(Pickup))]
public class PickupEditor : InteractableEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var pickup = target as Pickup;

        if (pickup.hasBonusItem)
        {
            pickup.bonusItem = EditorGUILayout.ObjectField("Bonus Item", pickup.bonusItem, typeof(Item), false) as Item;
            pickup.chance = EditorGUILayout.Slider("Chance", pickup.chance, 0, 1);
        }
    }
}

[CustomEditor(typeof(InteractableInventory))]
public class InteractableInventoryEditor : InteractableEditor
{
    InteractableInventory.JunkData junkData;

    void ListField<T>(ref List<T> list, string name) where T : class, IEditable, new()
    {
        int size = EditorGUILayout.IntSlider(name + " List Size : ", list.Count, 0, 10);

        if(size != list.Count)
        {
            if(size < list.Count)
            {
                list.RemoveRange(size, list.Count - size);
            }
            else
            {
                for(int i = list.Count; i < size; i++)
                {
                    list.Add(new T());
                }
            }
        }

        for(int i = 0; i < size; i++)
        {
            GUILayout.Label(name + " Element " + i + " : ");

            list[i].Edit();
        }
    }

    void InitLists(bool justPicks, bool minMaxPicks, bool chanceItems, bool chanceMinMaxBlueprints)
    {
        if (justPicks) ListField(ref junkData.justPicks, "Always There"); else junkData.justPicks.Clear();
        if (minMaxPicks) ListField(ref junkData.minMaxPicks, "Min Max"); else junkData.minMaxPicks.Clear();
        if (chanceItems) ListField(ref junkData.chanceItems, "Random"); else junkData.chanceItems.Clear();
        if (chanceMinMaxBlueprints) ListField(ref junkData.chanceMinMaxBlueprints, "Min Max Random"); else junkData.chanceMinMaxBlueprints.Clear();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var interactable = target as InteractableInventory;

        interactable.locked = EditorGUILayout.Toggle("Locked", interactable.locked);

        if (interactable.locked)
        {
            interactable.openTime = EditorGUILayout.FloatField("Open Time", interactable.openTime);
        }

        interactable.initAtAwake = EditorGUILayout.Toggle("Initialize At Start", interactable.initAtAwake);

        if (!interactable.initAtAwake)
        {
            interactable.getItemBlueprints = null;
            return;
        }

        GUILayout.Label("Item Blueprint Types:");

        if (junkData == null) junkData = interactable.junkData;

        junkData.justPick = EditorGUILayout.Toggle("Always There", junkData.justPick);
        junkData.minMax = EditorGUILayout.Toggle("Between minimum and maximum amounts", junkData.minMax);
        junkData.randomBetween = EditorGUILayout.Toggle("Random Ones", junkData.randomBetween);

        if (junkData.randomBetween)
        {
            junkData.minRandomPicks = EditorGUILayout.IntSlider("Minimum Random Picks", junkData.minRandomPicks, 0, junkData.maxRandomPicks);
            junkData.maxRandomPicks = EditorGUILayout.IntSlider("Maximum Random Picks", junkData.maxRandomPicks, junkData.minRandomPicks, 200);
        }

        if (junkData.justPick)
        {
            if (junkData.minMax)
            {
                if (junkData.randomBetween)
                {
                    InitLists(justPicks: false, minMaxPicks: true, chanceItems: false, chanceMinMaxBlueprints: true);
                }
                else
                {
                    InitLists(justPicks: false, minMaxPicks: true, chanceItems: false, chanceMinMaxBlueprints: false);
                }
            }
            else if (junkData.randomBetween)
            {
                InitLists(justPicks: true, minMaxPicks: false, chanceItems: true, chanceMinMaxBlueprints: false);
            }
            else
            {
                InitLists(justPicks: true, minMaxPicks: false, chanceItems: false, chanceMinMaxBlueprints: false);
            }
        }
        else if (junkData.minMax)
        {
            if (junkData.randomBetween)
            {
                InitLists(justPicks: false, minMaxPicks: true, chanceItems: false, chanceMinMaxBlueprints: true);
            }
            else
            {
                InitLists(justPicks: false, minMaxPicks: true, chanceItems: false, chanceMinMaxBlueprints: false);
            }
        }
        else if (junkData.randomBetween)
        {
            InitLists(justPicks: false, minMaxPicks: false, chanceItems: true, chanceMinMaxBlueprints: false);
        }
    }
}
