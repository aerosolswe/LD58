using GCG;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPicker : MonoBehaviour
{
    public Camera cam;

    public InputActionReference clickActionRef;
    public InputActionReference rotateActionRef; // bind this to Space

    public Transform rotateTowardsTransform;

    public float pickupDistance = 5f;
    public float moveSmoothness = 10f;
    public float rotateSmoothness = 5f; // how quickly it rotates toward target

    private Rigidbody2D heldBody;
    private Vector3 holdPoint;

    private void Update()
    {
        if (clickActionRef.action.WasPressedThisFrame())
        {
            TryPickup();
        } else if (clickActionRef.action.WasReleasedThisFrame())
        {
            Drop();
        }

        if (heldBody != null)
        {
            MoveHeldObject();

            if (rotateActionRef.action.IsPressed())
            {
                RotateHeldObject();
            }
        }
    }

    private void TryPickup()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = cam.ScreenToWorldPoint(mousePos);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider != null)
        {
            Rigidbody2D rb = hit.collider.attachedRigidbody;
            if (rb != null && rb.bodyType != RigidbodyType2D.Kinematic)
            {
                heldBody = rb;
                heldBody.gravityScale = 0f;
                heldBody.linearDamping = 10f;
                //GCGUtil.SetLayers(heldBody.gameObject, "PickedUpTrash");
            }
        }
    }

    private void Drop()
    {
        if (heldBody != null)
        {
            heldBody.gravityScale = 1f;
            heldBody.linearDamping = 0f;
            heldBody = null;
        }
    }

    private void MoveHeldObject()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        holdPoint = cam.ScreenToWorldPoint(mousePos);

        Vector2 moveDirection = (Vector2)holdPoint - heldBody.position;
        heldBody.linearVelocity = moveDirection * moveSmoothness;
    }

    private void RotateHeldObject()
    {
        if (rotateTowardsTransform == null)
            return;

        // Get direction to target
        Vector2 direction = ((Vector2)rotateTowardsTransform.position - heldBody.position).normalized;

        // Convert to angle in degrees
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Smoothly rotate towards target
        float newAngle = Mathf.LerpAngle(heldBody.rotation, targetAngle, rotateSmoothness * Time.deltaTime);
        heldBody.MoveRotation(newAngle);
    }
}
