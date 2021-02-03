using UnityEngine;

public class Pickup : Interactable
{
    public Item item;
    public int amount = 1;

    [Header("Optional")]
    public bool hasBonusItem;
    [HideInInspector]public Item bonusItem;
    [HideInInspector]public float chance;

    public GameObject remain;

    bool picked = false;

    protected override void Interaction()
    {
        PickUp();
    }

    void PickUp()
    {
        if (picked) return;

        picked = true;

        Player player = Player.instance;

        player.inventory.Add(item, amount);

        if (hasBonusItem)
        {
            if (Random.Range(0f, 1f) < chance)
            {
                player.inventory.Add(bonusItem);
            }
        }

        if (remain != null) Instantiate(remain, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
