using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var item = target as Item;

        if (item.IsEquipment) item.maxInOneSlot = (Item.MaxInOneSlotEnum)item.AsEquipment.CanTake;
        else item.maxInOneSlot = (Item.MaxInOneSlotEnum)EditorGUILayout.EnumPopup("Max In One Slot", item.maxInOneSlot);
    }
}

[CustomEditor(typeof(Buildable))]
public class BuildableEditor : ItemEditor
{

}

[CustomEditor(typeof(BuildBlueprint))]
public class BuildBlueprintEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var blueprint = target as BuildBlueprint;

        if (blueprint.mustBuildOnGround)
        {
            blueprint.requiredFloorLevel = 0;
        }
        else
        {
            blueprint.requiredFloorLevel = EditorGUILayout.IntSlider("Required Minimum Floor Level", blueprint.requiredFloorLevel, 0, 4);
        }
    }
}

[CustomEditor(typeof(Consumable))]
public class ConsumableEditor : ItemEditor
{
    void Edit(Consumable item, bool food = true, bool drink = true, bool health = true, bool stink = true, bool pee = true)
    {
        if(food)   item.foodBonus = EditorGUILayout.Slider("Food Bonus", item.foodBonus, 0, 100);
        if(drink)  item.drinkBonus = EditorGUILayout.Slider("Drink Bonus", item.drinkBonus, 0, 100);
        if(health) item.healthBonus = EditorGUILayout.Slider("Health Bonus", item.healthBonus, 0, 100);
        if(stink)  item.makeStinkBy = EditorGUILayout.Slider("Make Stink By", item.makeStinkBy, 0, 100);
        if(pee)    item.makePeeBy = EditorGUILayout.Slider("Make Pee By", item.makePeeBy, 0, 100);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var item = target as Consumable;

        item.consumeMode = (Consumable.ConsumeMode)EditorGUILayout.EnumPopup("Consume Mode", item.consumeMode);

        switch (item.consumeMode)
        {
            case Consumable.ConsumeMode.food:
                Edit(item, pee: false);
                break;
            case Consumable.ConsumeMode.drink:
                Edit(item, food: false, health: false);
                break;
            case Consumable.ConsumeMode.health:
                Edit(item, false, false, true, false, false);
                break;
            case Consumable.ConsumeMode.custom:
                Edit(item);
                break;
        }
    }
}

[CustomEditor(typeof(Equipment))]
public class EquipmentEditor : ItemEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var equipment = target as Equipment;

        if (!equipment.attachToSkin)
        {
            equipment.offsetPosition = EditorGUILayout.Vector3Field("Offset Position", equipment.offsetPosition);
            equipment.offsetRotation = EditorGUILayout.Vector3Field("Offset Rotation", equipment.offsetRotation);
            equipment.offsetScale = EditorGUILayout.Vector3Field("Offset Scale", equipment.offsetScale);
        }

        if (equipment.NecessarySlot == null)
        {
            equipment.EquipSlot = (EquipSlot)EditorGUILayout.EnumPopup("Equip Slot", equipment.EquipSlot);
            equipment.Armor = EditorGUILayout.IntSlider("Armor", equipment.Armor, 0, 100);
        }

        if (equipment.CanTakeLimit)
        {
            equipment.CanTake = EditorGUILayout.IntSlider("Can Take", equipment.CanTake, 1, 100);
        }

        if(equipment.EquipSlot == EquipSlot.Feet)
        {
            equipment.speedBonus = EditorGUILayout.IntSlider("Speed Bonus", equipment.speedBonus, 1, 40);
        }
    }
}

[CustomEditor(typeof(Tool))]
public class ToolEditor : EquipmentEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var tool = target as Tool;
        
        tool.toolType = (Tool.ToolType)EditorGUILayout.EnumPopup("Tool Type", tool.toolType);

        switch (tool.toolType)
        {
            case Tool.ToolType.Breaker:
                int amount = EditorGUILayout.IntField("Break Amount", tool.breaks.Length);

                if(amount != tool.breaks.Length)
                {
                    var old = tool.breaks;

                    tool.breaks = new int[amount];

                    int minSize = Mathf.Min(amount, old.Length);

                    for(int i = 0; i < minSize; i++)
                    {
                        tool.breaks[i] = old[i];
                    }
                }

                for(int i = 0; i < amount; i++)
                {
                    tool.breaks[i] = EditorGUILayout.LayerField("Break Layer", tool.breaks[i]);
                }

                break;
            case Tool.ToolType.Weapon:
                break;
        }
    }
}