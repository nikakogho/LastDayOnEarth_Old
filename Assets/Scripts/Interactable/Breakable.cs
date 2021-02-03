using UnityEngine;

public class Breakable : Interactable
{
    public override bool CanInteract { get { return Player.instance.equipmentManager.Weapon.breaks.Contains(gameObject.layer); } }
    public int health = 3;

    public GameObject remain;
    public float remainLifeTime = 3;

    protected override void Interaction()
    {
        Player player = Player.instance;

        player.equipmentManager[(int)EquipSlot.Weapon].amount--;

        if (player.equipmentManager[(int)EquipSlot.Weapon].amount == 0) player.equipmentManager.Remove(EquipSlot.Weapon);

        health--;

        if(health == 0)
        {
            if (remain != null) Destroy(Instantiate(remain, transform.position, transform.rotation), remainLifeTime);

            Destroy(gameObject);
        }
    }
}
