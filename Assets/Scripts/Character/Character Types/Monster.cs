using UnityEngine;
using ItemBlueprintStuff;
using System.Collections.Generic;

public class Monster : Character
{
    public int xpValue = 50;
    public float seeRange;

    [Range(30, 180)]
    public float seeAngle;

    public List<ChanceMinMaxBlueprint> possibleRewards;
    public GameObject remain;

    public int armor = 0;

    public override int Armor { get { return armor; } }

    void Start()
    {
        InvokeRepeating("UpdateTarget", 0, 0.2f);
        InvokeRepeating("FollowTarget", 0.3f, 0.2f);
        InvokeRepeating("UpdateAnim", 0.2f, 0.4f);
    }

    void UpdateTarget()
    {
        if (target == null)
        {
            GetTarget(seeRange, seeAngle, targetMask);
        }
    }

    void UpdateAnim()
    {
        bool moving = target != null && motor.MoveSpeed > 0.01f;

        anim.SetBool("Moving", moving);
    }

    protected override void Die()
    {
        if(this.remain != null)
        {
            var remain = Instantiate(this.remain, transform.position, transform.rotation);

            var interactableInventory = remain.GetComponent<InteractableInventory>();

            var reward = possibleRewards.ToItemBlueprintList().ToArray();

            interactableInventory.SetItems(reward);
        }

        Player.instance.AddXP(xpValue);

        base.Die();
    }

    protected override void GizmoStuff()
    {
        base.GizmoStuff();

        SeeGizmo(seeRange, seeAngle);
    }
}
