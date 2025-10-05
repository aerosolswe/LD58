using GCG;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Truck : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource startSFX;
    public AudioSource idleSFX;
    public AudioSource giveGasSFX;
    public AudioSource stopGasSFX;
    public AudioSource idleGasSFX;

    [Header("Wheels")]
    public WheelJoint2D RearWheel0;
    public WheelJoint2D RearWheel1;
    public WheelJoint2D FrontWheel;
    public Rigidbody2D Body;

    [Header("Input")]
    public InputActionReference MoveRightInputRef;
    public InputActionReference MoveLeftInputRef;

    [Header("Upgrades")]
    public TruckPlow Plow;
    public TruckSideLoader SideLoader0;
    public TruckSideLoader SideLoader1;
    public TruckSideLoader SideLoader2;
    public TruckSideLoader SideLoader3;

    [Header("Settings")]
    public float motorSpeed = 1000f;
    public float motorTorque = 2000f;
    public float brakeDrag = 5f;
    public float normalDrag = 0.5f;

    public float brakeUpgradeAmount = 1.0f;
    public float engineUpgradeAmount = 100.0f;

    private float extraMotorSpeed = 0.0f;
    private float extraBrakeSpeed = 0.0f;

    public bool GivingGas
    {
        get; set;
    }

    public bool Braking
    {
        get; set;
    }

    private void Start()
    {
        Initialize();
        Audio();
    }

    public void Audio()
    {
        StartCoroutine(startSound());
        StartCoroutine(idleLoop());
        StartCoroutine(gasLoop());

        IEnumerator startSound()
        {
            yield return GCGUtil.Yield(1.0f);
            startSFX.Play();
        }

        IEnumerator idleLoop()
        {
            yield return GCGUtil.Yield(2.0f);
            idleSFX.Play();
        }


        IEnumerator gasLoop()
        {
            bool previouslyGaveGas = false;
            Coroutine routine = null;

            while (true)
            {
                yield return null;

                if (GivingGas && !previouslyGaveGas)
                {
                    previouslyGaveGas = true;
                    stopGasSFX.Stop();
                    giveGasSFX.Play();

                    routine = StartCoroutine(idleGas(giveGasSFX.clip.length - 0.0f));

                } else if (!GivingGas && previouslyGaveGas)
                {
                    previouslyGaveGas = false;
                    if (routine != null)
                        StopCoroutine(routine);
                    giveGasSFX.Stop();
                    idleGasSFX.Stop();

                    if (Body.linearVelocity.magnitude > 3.5f)
                    {
                        stopGasSFX.Play();
                    }
                }
            }
        }

        IEnumerator idleGas(float time)
        {
            yield return GCGUtil.Yield(time);
            idleGasSFX.Play();
        }
    }

    public void Initialize()
    {
        Plow.gameObject.SetActive(UserDataManager.GetSavedValue("upgrade_plow", "0") == "1");

        SideLoader0.gameObject.SetActive(int.Parse(UserDataManager.GetSavedValue("upgrade_side_loader", "0")) >= 1);
        SideLoader1.gameObject.SetActive(int.Parse(UserDataManager.GetSavedValue("upgrade_side_loader", "0")) >= 2);
        SideLoader2.gameObject.SetActive(int.Parse(UserDataManager.GetSavedValue("upgrade_side_loader", "0")) >= 3);
        SideLoader3.gameObject.SetActive(int.Parse(UserDataManager.GetSavedValue("upgrade_side_loader", "0")) >= 4);

        extraBrakeSpeed = 0.0f;
        extraBrakeSpeed += int.Parse(UserDataManager.GetSavedValue("upgrade_brakes", "0")) * brakeUpgradeAmount;

        extraMotorSpeed = 0.0f;
        extraMotorSpeed += int.Parse(UserDataManager.GetSavedValue("upgrade_engine", "0")) * engineUpgradeAmount;
    }

    private void Update()
    {
        float direction = 0f;

        if (MoveRightInputRef.action.IsPressed() && GameController.Instance.State == GameController.GameState.Active)
            direction = -1f;
        else if (MoveLeftInputRef.action.IsPressed() && GameController.Instance.State == GameController.GameState.Active)
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
        GivingGas = true;
        Braking = false;

        if (RearWheel0 != null)
            RearWheel0.attachedRigidbody.linearDamping = normalDrag;
        if (RearWheel1 != null)
            RearWheel1.attachedRigidbody.linearDamping = normalDrag;
        if (FrontWheel != null)
            FrontWheel.attachedRigidbody.linearDamping = normalDrag;

        JointMotor2D motor = new JointMotor2D
        {
            motorSpeed = (motorSpeed + extraMotorSpeed) * direction,
            maxMotorTorque = motorTorque + (extraMotorSpeed / 2.0f)
        };

        if (RearWheel0 != null)
        {
            RearWheel0.motor = motor;
            RearWheel0.useMotor = true;
        }

        if (RearWheel1 != null)
        {
            RearWheel1.motor = motor;
            RearWheel1.useMotor = true;
        }

        if (FrontWheel != null)
        {
            FrontWheel.motor = motor;
            FrontWheel.useMotor = true;
        }
    }

    private void BrakeMotor()
    {
        GivingGas = false;
        Braking = true;

        if (RearWheel0 != null)
            RearWheel0.attachedRigidbody.linearDamping = brakeDrag;
        if (RearWheel1 != null)
            RearWheel1.attachedRigidbody.linearDamping = brakeDrag;
        if (FrontWheel != null)
            FrontWheel.attachedRigidbody.linearDamping = brakeDrag;

        JointMotor2D motor = new JointMotor2D
        {
            motorSpeed = 0f,                         // stops wheel rotation
            maxMotorTorque = motorTorque + extraBrakeSpeed // stronger brake torque
        };

        if (RearWheel0 != null)
        {
            RearWheel0.motor = motor;
            RearWheel0.useMotor = true;
        }

        if (RearWheel1 != null)
        {
            RearWheel1.motor = motor;
            RearWheel1.useMotor = true;
        }

        if (FrontWheel != null)
        {
            FrontWheel.motor = motor;
            FrontWheel.useMotor = true;
        }
    }
}
