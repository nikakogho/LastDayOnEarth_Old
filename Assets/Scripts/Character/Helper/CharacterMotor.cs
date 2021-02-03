using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterMotor : MonoBehaviour
{
    [HideInInspector]public Character target;
    NavMeshAgent agent;

    enum TargetMode { Chase, Escape }
    TargetMode targetMode;
    float runAwayDistance;

    Character character;

    public float MoveSpeed { get { return agent.velocity.magnitude; } }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.stoppingDistance = 0.3f;

        character = GetComponent<Character>();

        target = null;

        InvokeRepeating("UpdateMoveSpeed", 0, 0.3f);
    }

    void UpdateMoveSpeed()
    {
        agent.speed = character.MoveSpeed;
        agent.velocity = agent.desiredVelocity;
    }

    public void Chase(Character target)
    {
        this.target = target;

        targetMode = TargetMode.Chase;
    }

    public void RunAwayFrom(Character target, float runAwayDistance)
    {
        this.target = target;
        this.runAwayDistance = runAwayDistance;

        targetMode = TargetMode.Escape;
    }

    public void MoveAt(Vector3 point)
    {
        point.y = 0;
        target = null;

        agent.SetDestination(point);
    }

    public void StopMoving()
    {
        agent.SetDestination(transform.position);
        target = null;
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector3 dir = target.transform.position - transform.position;
        dir.y = 0;
        dir.Normalize();

        switch (targetMode)
        {
            case TargetMode.Chase:
                if(Vector3.Distance(agent.destination, target.transform.position) > character.AttackRange)
                agent.SetDestination(target.transform.position);

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), agent.angularSpeed * Time.fixedDeltaTime);
                break;

            case TargetMode.Escape:
                dir *= -1;

                agent.SetDestination(transform.position + dir * runAwayDistance);

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), agent.angularSpeed * Time.fixedDeltaTime);
                break;
        }
    }
}
