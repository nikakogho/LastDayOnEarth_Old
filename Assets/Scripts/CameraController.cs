using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 20, -8);
    Transform player;

    void Start()
    {
        player = Player.instance.transform;
    }

    void FixedUpdate()
    {
        transform.position = player.position + offset;
    }
}
