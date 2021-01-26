using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    Rigidbody rb;
    PartBodyController pbc;
    PlayerStabilizer ps;
    bool headRotate = false;
    bool flying;
    bool swimming;
    bool grounded;
    public float walkSpeed;
    public float runSpeed;
    public float maxVelocityChange;
    public float rotSpeed;
    public float jumpForce;
    public float jumpHeight;
    public float jumpReload;
    float lastJumpTime;
    public PlayerGrip playerGripL;
    public PlayerGrip playerGripR;
    // Use this for initialization

    public AnimationCurve moveSpeedX;
    public AnimationCurve moveSpeedY;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pbc = GetComponent<PartBodyController>();
        ps = GetComponent<PlayerStabilizer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        grounded = (ps.distanceToGround > -1) ? true : false;
    }

    public void Move(Vector2 direction, bool jump)
    {
        float finalMoveSpeed = walkSpeed;
        float magnitude = direction.magnitude;
        //Vector3 targetVelocity = new Vector3(direction.x, 0, direction.y).normalized; // * finalMoveSpeed;
        Vector3 targetVelocity = new Vector3(moveSpeedX.Evaluate(direction.x),0, moveSpeedY.Evaluate(direction.y)) * finalMoveSpeed;
        targetVelocity = pbc.head.transform.TransformDirection(targetVelocity);
        //targetVelocity = Quaternion.Euler(new Vector3(0, pbc.head.transform.rotation.y, 0)) * targetVelocity;
        //targetVelocity *= finalMoveSpeed;

        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = targetVelocity - velocity;

        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);

        if(swimming)
        {
            velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);
        }
        else
        {
            velocityChange.y = 0;
        }

        if (jump && grounded && Time.time - lastJumpTime > jumpReload)
        {
            lastJumpTime = Time.time;
            velocityChange.y = jumpHeight;
        }

        //if (grounded)
            rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }
    public void Jump(Vector2 direction)
    {
        Vector3 targetVelocity = new Vector3(direction.x, 0, direction.y);
        targetVelocity = pbc.head.transform.TransformDirection(targetVelocity);
        targetVelocity *= walkSpeed;

        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = targetVelocity - velocity;

        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }
    public void Brake()
    {

    }
    public void Rotate(float rotation)
    {
        if (flying)
        {
            rb.AddTorque(new Vector3(0, rotation * rotSpeed, 0));
        }
    }
    public void Rotate(Vector2 axis)
    {
        Vector3 rot = new Vector3(axis.y * rotSpeed, 0, -axis.x * rotSpeed);
        rb.AddRelativeTorque(rot);
    }
}
