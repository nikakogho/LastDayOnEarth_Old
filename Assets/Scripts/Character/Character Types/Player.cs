using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Player : Human
{
    [Range(0, 100)]
    public float hunger = 100, thirst = 100, pee = 100, stink = 100;
    public float starveSpeed, dehydrateSpeed, wantToPeeSpeed, stinkSpeed;

    public float hungerHitDelta = 2, hungerDamage = 4;
    public float thirstHitDelta = 2, thirstDamage = 4;

    public static Player instance;
    bool autoMode = false;

    bool chasingTarget = false;

    public LayerMask pickUpAbleMask, groundMask, interactableMask;
    public float pickUpSpotRange = 160;

    [HideInInspector]public Interactable chosenInteractable;

    GameMaster master;

    public int level { get; private set; }
    public int xp { get; private set; }

    public int requiredXP { get; private set; }

    public override bool Stinks { get { return stink <= 0; } }

    bool crouching = false;
    public override bool IsCrouching { get { return crouching; } }

    public float spotRange = 12;

    float SpotRange { get { return Mathf.Max(spotRange, AttackRange + 3); } }

    protected override void AwakeStuff()
    {
        instance = this;

        level = 1;
        xp = 0;

        requiredXP = 200;

        InvokeRepeating("UpdateTarget", 0.4f, 0.5f);
    }

    protected override void DelegateReferences()
    {
        base.DelegateReferences();

        master = GameMaster.instance;
    }

    void UpdateTarget()
    {
        GetTarget(SpotRange, 360, targetMask);
    }

    protected override void Die()
    {
        // game over
    }

    public void AddXP(int value)
    {
        xp += value;

        if(xp >= requiredXP)
        {
            xp -= requiredXP;
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;

        health = 100;

        requiredXP = (int)(requiredXP * 1.3f);
    }

    void GetInput()
    {
        if (Input.GetKeyDown("i"))
        {
            if (master.playerUI.activeSelf)
            {
                if(chosenInteractable != null && chosenInteractable.GetType() == typeof(InteractableInventory))
                {
                    chosenInteractable = null;
                }

                master.CloseUI();
            }
            else
            {
                master.OpenUI(master.playerEquipmentUI);
            }
        }

        if (master.playerUI.activeSelf) return;

        if (Input.GetKeyDown("z"))
        {
            autoMode = !autoMode;

            crouching = false;
        }

        crouching = Input.GetKey(KeyCode.LeftControl);

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h != 0 || v != 0)
        {
            motor.target = null;
            chasingTarget = false;

            Vector3 direction = new Vector3(h, 0, v);

            motor.MoveAt(transform.position + direction);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (target == null)
            {
                anim.SetTrigger("Attack");
            }
            else
            {
                motor.Chase(target);
                chasingTarget = true;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100, groundMask))
                {
                    chasingTarget = false;
                    autoMode = false;

                    motor.MoveAt(hit.point);
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100, interactableMask) && hit.transform != transform)
                {
                    autoMode = false;

                    Interactable interactable = hit.transform.GetComponent<Interactable>();

                    if (interactable == null)
                    {
                        Character character = hit.transform.GetComponent<Character>();

                        chasingTarget = true;
                        motor.Chase(character);
                    }
                    else
                    {
                        if (interactable.CanInteract)
                        {
                            chosenInteractable = interactable;
                            chasingTarget = false;

                            motor.MoveAt(chosenInteractable.transform.position);
                        }
                    }
                }
            }
        }
    }

    bool hungerHitting = false;
    bool thirstHitting = false;

    void Update()
    {
        GetInput();

        float t = Time.deltaTime;

        if (hunger > 0)
            hunger -= starveSpeed * t;
        else if (!hungerHitting) StartCoroutine(GetHitByStat(true, hungerHitDelta, hungerDamage));

        if (thirst > 0)
            thirst -= dehydrateSpeed * t;
        else if (!thirstHitting) StartCoroutine(GetHitByStat(false, thirstHitDelta, thirstDamage));

        if (pee > 0)
            pee -= wantToPeeSpeed * t;

        if (stink > 0)
            stink -= stinkSpeed * t;
    }

    IEnumerator GetHitByStat(bool hunger, float hitAfter, float damage)
    {
        if (hunger) hungerHitting = true;
        else thirstHitting = true;

        yield return new WaitForSeconds(hitAfter);
        health -= damage;

        if (hunger) hungerHitting = false;
        else thirstHitting = false;
    }

    void FixedUpdate()
    {
        if (autoMode)
        {
            if(target == null)
            {
                if(chosenInteractable == null)
                {
                    Collider[] cols = Physics.OverlapSphere(transform.position, pickUpSpotRange, pickUpAbleMask);

                    float minDist = float.MaxValue;

                    foreach (Collider col in cols)
                    {
                        float dist = Vector3.Distance(transform.position, col.transform.position);

                        if (dist > minDist) continue;

                        var interactable = col.GetComponent<Interactable>();

                        if (interactable is Pickup)
                        {
                            chosenInteractable = interactable;
                            minDist = dist;
                        }
                        else
                        {
                            var breakable = interactable as Breakable;

                            if (breakable.CanInteract)
                            {
                                chosenInteractable = interactable;
                                minDist = dist;
                            }
                        }
                    }

                    if(chosenInteractable == null)
                    {
                        //ToDo: change this Debug.Log() with some Text
                        Debug.Log("Nothing To Do So Exiting Auto Mode");
                        autoMode = false;
                    }
                    else
                    {
                        motor.MoveAt(chosenInteractable.InteractionTransform.position);
                    }
                }
                else
                {
                    if(Vector3.Distance(transform.position, chosenInteractable.InteractionTransform.position) <= chosenInteractable.interactRange)
                    {
                        chosenInteractable.Interact();
                    }
                }
            }
            else
            {
                chosenInteractable = null;

                if (!chasingTarget)
                {
                    chasingTarget = true;
                    motor.Chase(target);
                }

                if (Vector3.Distance(transform.position, target.transform.position) <= AttackRange)
                {
                    if (attackCountdown <= 0)
                    {
                        Attack(target);
                    }
                }
            }

            return;
        }

        if (chasingTarget)
        {
            if(target == null)
            {
                chasingTarget = false;
                motor.StopMoving();
                return;
            }

            if (chosenInteractable != null) chosenInteractable = null;

            if(Vector3.Distance(transform.position, target.transform.position) <= AttackRange)
            {
                if(attackCountdown <= 0)
                {
                    Attack(target);

                    chasingTarget = false;
                    motor.StopMoving();
                }
            }
        }
        else if(chosenInteractable != null)
        {
            if (Vector3.Distance(transform.position, chosenInteractable.InteractionTransform.position) <= chosenInteractable.interactRange)
            {
                chosenInteractable.Interact();

                chosenInteractable = null;
                motor.StopMoving();
            }
        }
    }

    protected override void GizmoStuff()
    {
        base.GizmoStuff();

        SeeGizmo(SpotRange, 360);
    }
}
