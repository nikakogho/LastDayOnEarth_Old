using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterMotor))]
[DisallowMultipleComponent]
public abstract class Character : MonoBehaviour
{
    new public string name;
    public LayerMask targetMask;

    [HideInInspector]public Animator anim;
    [HideInInspector]public CharacterMotor motor;

    public float health, defaultDamage, defaultMoveSpeed, defaultAttackSpeed, defaultAttackRange, defaultAttackAfter;

    public float startHealth { get; private set; }
    
    public Character target;

    protected float attackCountdown = 0;
    float attackCountdownUpdateDelta = 0.04f;
    
    public delegate void OnGetHit();
    public OnGetHit onGetHit;

    [HideInInspector]
    public bool dead = false;

    public virtual float Damage { get { return defaultDamage; } }
    public virtual float AttackSpeed { get { return defaultAttackSpeed; } }
    public virtual float AttackRange { get { return defaultAttackRange; } }
    public virtual float AttackAfter { get { return defaultAttackAfter; } }
    public virtual float MoveSpeed { get { return defaultMoveSpeed; } }

    public virtual int Armor { get { return 0; } }

    public virtual bool Stinks { get { return false; } }

    public virtual bool IsCrouching { get { return false; } }

    public enum MoveAnimType { Float, Boolean }
    public MoveAnimType moveAnimType;

    void Awake()
    {
        onGetHit += CheckForDieing;

        anim = GetComponent<Animator>();
        motor = GetComponent<CharacterMotor>();

        InvokeRepeating("UpdateAttackCountdown", attackCountdownUpdateDelta, attackCountdownUpdateDelta);
        InvokeRepeating("UpdateMoveAnim", 0, 0.2f);

        startHealth = health;

        AwakeStuff();
    }

    protected virtual void AwakeStuff() { }

    void UpdateAttackCountdown()
    {
        if (attackCountdown > 0 && !attacking) attackCountdown -= attackCountdownUpdateDelta;
    }

    void UpdateMoveAnim()
    {
        switch (moveAnimType)
        {
            case MoveAnimType.Boolean:
                anim.SetBool("Moving", motor.MoveSpeed != 0);
                break;

            case MoveAnimType.Float:
                anim.SetFloat("Move", motor.MoveSpeed / MoveSpeed);
                break;
        }
    }

    void CheckForDieing()
    {
        if (dead) return;

        if (health <= 0) Die();
    }

    protected virtual void Die()
    {
        health = 0;
        dead = true;

        Destroy(gameObject);
    }

    bool attacking = false;

    protected virtual void Attack(Character target)
    {
        if(!attacking)
        StartCoroutine(AttackRoutine(target));
    }

    IEnumerator AttackRoutine(Character target)
    {
        attacking = true;
        attackCountdown = 1f / AttackSpeed;
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(AttackAfter);

        attacking = false;
        target.TakeDamage(Damage);
    }

    double GetDamageReductionPercentage(int armor)
    {
        return 100.0 * armor / (armor + 7);
    }

    protected void TakeDamage(float damage)
    {
        if (dead) return;

        float damageReductionPercentage = (float)GetDamageReductionPercentage(Armor);

        float actualDamage = damage * (100 - damageReductionPercentage) / 100;

        if(actualDamage > 0)
        {
            health -= actualDamage;

            onGetHit.Invoke();
        }
    }

    protected void FollowTarget()
    {
        if (target == null)
        {
            if (motor.target != null) motor.StopMoving();

            return;
        }

        if (motor.target == null) motor.Chase(target);

        if (Vector3.Distance(transform.position, target.transform.position) < AttackRange && attackCountdown <= 0)
        {
            Attack(target);
        }
    }

    protected void EscapeTarget()
    {
        if (target == null)
        {
            if (motor.target != null) motor.StopMoving();

            return;
        }

        if (motor.target == null) motor.RunAwayFrom(target, target.AttackRange + 8);
    }

    protected void GetTarget(float seeRange, float seeAngle, LayerMask targetMask)
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, seeRange, targetMask);

        float minDist = float.MaxValue;
        Collider chosenCol = null;

        foreach(Collider col in cols)
        {
            if (col.transform == transform) continue;

            Vector3 direction = col.transform.position - transform.position;
            float distance = direction.magnitude;

            if (distance > minDist) continue;

            bool canSee = Vector3.Angle(transform.forward, direction / distance) <= seeAngle / 2;

            if (canSee)
            {
                minDist = distance;
                chosenCol = col;
            }
            else
            {
                var character = col.GetComponent<Character>();

                bool canSmell = character.Stinks;
                bool canSpot = !character.IsCrouching;

                if (canSmell || canSpot)
                {
                    minDist = distance;
                    chosenCol = col;
                }
            }
        }

        target = chosenCol == null ? null : chosenCol.GetComponent<Character>();
    }

    protected virtual void GizmoStuff()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }

    protected void SeeGizmo(float seeRange, float seeAngle)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, seeRange);

        if(seeAngle > 0 && seeAngle < 360)
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawLine(transform.position, transform.position + (seeAngle / 2).DirFromAngle() * seeRange);
            Gizmos.DrawLine(transform.position, transform.position + (-seeAngle / 2).DirFromAngle() * seeRange);
        }
    }

    void OnDrawGizmosSelected()
    {
        GizmoStuff();
    }
}