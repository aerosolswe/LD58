using UnityEngine;
using UnityEngine.InputSystem;

public class Truck : MonoBehaviour
{
    [Header("Wheels")]
    public WheelJoint2D RearWheel;
    public WheelJoint2D FrontWheel;

    [Header("Input")]
    public InputActionReference MoveRightInputRef;
    public InputActionReference MoveLeftInputRef;

    [Header("Settings")]
    public float motorSpeed = 1000f;
    public float motorTorque = 2000f; 
    public float brakeDrag = 5f;
    public float normalDrag = 0.5f;

    public bool frontWheelDrive = true;
    public bool rearWheelDrive = true;

    private void Update()
    {
        float direction = 0f;

        if (MoveRightInputRef.action.IsPressed())
            direction = -1f;
        else if (MoveLeftInputRef.action.IsPressed())
            direction = 1f;

        if (Mathf.Abs(direction) > 0.01f)
        {
            EnableMotor(direction);
        } else
        {
            BrakeMotor();
        }
    }

    private void EnableMotor(float direction)
    {
        if (RearWheel != null)
            RearWheel.attachedRigidbody.linearDamping = normalDrag;
        if (FrontWheel != null)
            FrontWheel.attachedRigidbody.linearDamping = normalDrag;

        JointMotor2D motor = new JointMotor2D
        {
            motorSpeed = motorSpeed * direction,
            maxMotorTorque = motorTorque
        };

        if (rearWheelDrive && RearWheel != null)
        {
            RearWheel.motor = motor;
            RearWheel.useMotor = true;
        }

        if (frontWheelDrive && FrontWheel != null)
        {
            FrontWheel.motor = motor;
            FrontWheel.useMotor = true;
        }
    }

    private void BrakeMotor()
    {
        if (RearWheel != null)
            RearWheel.attachedRigidbody.linearDamping = brakeDrag;
        if (FrontWheel != null)
            FrontWheel.attachedRigidbody.linearDamping = brakeDrag;

        JointMotor2D motor = new JointMotor2D
        {
            motorSpeed = 0f,                         // stops wheel rotation
            maxMotorTorque = motorTorque // stronger brake torque
        };

        if (rearWheelDrive && RearWheel != null)
        {
            RearWheel.motor = motor;
            RearWheel.useMotor = true;
        }

        if (frontWheelDrive && FrontWheel != null)
        {
            FrontWheel.motor = motor;
            FrontWheel.useMotor = true;
        }
    }
}
