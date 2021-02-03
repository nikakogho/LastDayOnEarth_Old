using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
    [SerializeField]Transform interactionTransform;

    public Transform InteractionTransform { get { return interactionTransform != null ? interactionTransform : transform; } }
    [HideInInspector]public float seeRange;
    [HideInInspector]public float interactRange;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, seeRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }

    void OnValidate()
    {
        if (interactRange > seeRange) interactRange = seeRange - 0.1f;
    }

    public virtual bool CanInteract { get { return true; } }
    protected abstract void Interaction();

    public void Interact()
    {
        if (CanInteract) Interaction();
    }
}
