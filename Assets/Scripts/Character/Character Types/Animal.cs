using System.Collections.Generic;
using UnityEngine;

public class Animal : Character
{
    public float seeRange;
    [Range(0, 180)]
    public float seeAngle;

    public Item meat;
    public Item fur;
    [Range(0, 1)]
    public float furChance = 0.4f;
    public GameObject remain;

    public bool hostile;

    void Start()
    {
        InvokeRepeating("UpdateTarget", 0.5f, 0.5f);

        if (hostile)
        {
            InvokeRepeating("FollowTarget", 0.5f, 0.2f);
        }
        else
        {
            InvokeRepeating("EscapeTarget", 0.5f, 0.2f);
        }
    }

    void UpdateTarget()
    {
        if(target == null)
        {
            GetTarget(seeRange, seeAngle, targetMask);
        }
    }

    protected override void Die()
    {
        List<ItemBlueprint> items = new List<ItemBlueprint>();
        items.Add(meat);

        if (Random.Range(0f, 1f) < furChance) items.Add(fur);

        if(remain != null)
        Instantiate(remain, transform.position, transform.rotation).GetComponent<InteractableInventory>().SetItems(items.ToArray());

        base.Die();
    }

    protected override void GizmoStuff()
    {
        base.GizmoStuff();

        SeeGizmo(seeRange, seeAngle);
    }
}
