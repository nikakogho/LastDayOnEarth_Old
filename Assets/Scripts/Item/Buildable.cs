using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Item/Buildable")]
public sealed class Buildable : Item
{
    public BuildBlueprint blueprint;

    public override bool CanBeUsed { get { return true; } }
    protected override string UseTitle { get { return "BUILD"; } }

    protected override void UseMethod()
    {
        blueprint.Build();
    }
}
