using GCG;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPicker : Singleton<ObjectPicker>
{
    public Camera cam;

    public InputActionReference clickActionRef;
    public InputActionReference rotateActionRef; // bind this to Space

    public Transform rotateTowardsTransform;

    public float pickupDistance = 5f;
    public float moveSmoothness = 10f;
    public float rotateSmoothness = 5f; // how quickly it rotates toward target

    public LayerMask LayerMask;

    private float tempDamping = 0.0f;
    private Rigidbody2D heldBody;
    private Vector3 holdPoint;

    public Rigidbody2D HeldBody => heldBody;

    private void Update()
    {
        if (clickActionRef.action.WasPressedThisFrame() && GameController.Instance.State == GameController.GameState.Active)
        {
            TryPickup();
        } else if (clickActionRef.action.WasReleasedThisFrame() || GameController.Instance.State != GameController.GameState.Active)
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

        Collider2D collider = Physics2D.OverlapPoint(worldPos, LayerMask);
        if (collider != null)
        {
            Rigidbody2D rb = collider.attachedRigidbody;
            if (rb != null && rb.bodyType != RigidbodyType2D.Kinematic)
            {
                heldBody = rb;
                heldBody.gravityScale = 0f;
                tempDamping = heldBody.linearDamping;
                heldBody.linearDamping = 10f;
                //GCGUtil.SetLayers(heldBody.gameObject, "PickedUpTrash");
            }
        }
    }

    public void Drop()
    {
        if (heldBody != null)
        {
            heldBody.gravityScale = 1f;
            heldBody.linearDamping = tempDamping;
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
