using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Item/Consumable")]
public sealed class Consumable : Item
{
    [Range(0, 100)]
    [HideInInspector]public float foodBonus, drinkBonus, healthBonus, makePeeBy, makeStinkBy;

    public Item remain;

    public enum ConsumeMode { food, drink, health, custom }
    [HideInInspector]
    public ConsumeMode consumeMode;

    public override bool CanBeUsed { get { return true; } }

    protected override void UseMethod()
    {
        Player player = Player.instance;

        player.hunger += foodBonus;
        player.thirst += drinkBonus;
        player.health += healthBonus;

        player.pee += makePeeBy;
        player.stink += makeStinkBy;

        if(remain != null)
        player.inventory.Add(remain);
    }
}
