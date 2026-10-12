using UnityEngine;

public class PlayerPusher : MonoBehaviour
{
    [SerializeField] private float pushSpeed = 2f;
    private PlayerSize playerSize;
    private void Start()
    {
        playerSize = GetComponent<PlayerSize>();
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        PushableBox box = hit.collider.GetComponent<PushableBox>();
        if (box == null) return;
        if (!box.CanBePushedBy(playerSize.CurrentSizeMask)) return;
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic) return;
        if (hit.moveDirection.y < -0.3f) return;
        Vector3 direction = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
        body.linearVelocity = direction * pushSpeed;
    }
}