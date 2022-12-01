using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerP : MonoBehaviour
{
    private Rigidbody2D body;
    private int horizontal;
    private Vector3 direction;
    private float timer;
    private float jumpStamina;
    private bool grounded;
    private bool doublejump;
    private float spd;
    private float mJp;
    private bool fixJump;
    private bool dash;
    private float dashTimer;
    private EnergyHealth energy;
    private bool cling;

    [Header("keys")]
    public KeyCode DashMove;
    public KeyCode LeftMoving;
    public KeyCode RightMoving;
    public KeyCode JumpMove;

    [Header("parameters")]
    public float speed;
    public float jumpDur;
    public float jumpStrength;
    public float momentumJump;
    public float dashStrength;
    public float dashCooldown;
    public float RecForDouble;
    public float RecForDash;
    public bool DoubleJump;
    public bool Dash;
    public bool WallCling;

    [Header("attached")]
    public GameObject Robot;

    [Header("other")]
    public bool isFacingRight;

    Animator anim;

    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.transform.tag == "Ground")
        {
            jumpStamina = 0;
            grounded = true;
            if (DoubleJump == true) doublejump = true; else doublejump = false;
            if (Dash == true) dash = true; else dash = false;
        }
        if (coll.transform.tag == "Ceiling")
        {
            jumpStamina = jumpDur;
        }
        if (coll.transform.tag == "Wall")
        {
            if (WallCling == true)
            {
                cling = true;
                EnergyAction();
                jumpStamina = 0;
                grounded = true;
                if (DoubleJump == true) doublejump = true; else doublejump = false;
                if (Dash == true) dash = true; else dash = false;
            }
            else cling = false;

        }

        mJp = momentumJump;
        spd = speed;
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.transform.tag == "Ground")
        {
            grounded = false;
        }
        if (coll.transform.tag == "Wall")
        {
            grounded = false;
            cling = false;
        }
        spd = speed / 2;
        mJp = momentumJump * 1.25f;
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        energy = Robot.GetComponent<EnergyHealth>();
        anim = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        body.AddForce(direction * body.mass * spd);
    }
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void EnergyAction()
    {
        energy.cooldown = energy.timer + energy.RegenCooldown;
    }

    private float add;

    void Update()
    {

        // Walking

        if (Input.GetKey(LeftMoving)) horizontal = -1;
        else if (Input.GetKey(RightMoving)) horizontal = 1; else horizontal = 0;

        if (cling == true) body.drag = 50;
        else body.drag = 10;
        // Jumping

        if (energy.Energy >= RecForDouble)
        {
            if (Input.GetKeyUp(JumpMove))
            {
                fixJump = false;
                body.drag = 10;
            }
            if (jumpDur <= jumpStamina)
            {
                fixJump = false;
            }
            if (DoubleJump == true)
            {
                if (Input.GetKeyDown(JumpMove) && (doublejump == true))
                {
                    if (cling == true)
                    {
                        if (isFacingRight == true)body.velocity += new Vector2(-30, mJp);
                        else body.velocity += new Vector2(30, mJp);
                        Flip();
                    }
                    fixJump = true;
                    body.velocity += new Vector2(0, mJp);
                    if (grounded == false)
                    {
                        EnergyAction();

                        energy.Energy -= RecForDouble;
                        doublejump = false;
                        jumpStamina = 0;
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(JumpMove) && ((grounded == true) || (cling == true)))
                {
                    if (cling == true)
                    {
                        if (isFacingRight == true) body.velocity += new Vector2(-30, mJp);
                        else body.velocity += new Vector2(30, mJp);
                        Flip();
                    }
                    fixJump = true;
                    body.velocity += new Vector2(0, mJp);
                }
            }

            if (Input.GetKey(JumpMove) && (jumpDur > jumpStamina) && 
                (fixJump == true) && (grounded == false))
            {
                add = (jumpDur - jumpStamina) / jumpDur;
                body.AddForce(new Vector3(0, 1, 0) * body.mass * jumpStrength * add);
                jumpStamina += Time.deltaTime;
            }
        }
        // Dash

        if (energy.Energy >= RecForDash)
        {
            if (Input.GetKeyDown(DashMove) && (dash == true) && (dashTimer < timer))
            {
                EnergyAction();

                energy.Energy -= RecForDash;
                dashTimer = timer + dashCooldown;
                body.velocity = new Vector2(body.velocity.y, 10);
                if (isFacingRight == true) body.velocity += new Vector2(dashStrength, 0);
                else body.velocity += new Vector2(-dashStrength, 0);
                if (grounded == false)
                {
                    dash = false;
                }
            }
        }


        if (grounded == false) EnergyAction();

        direction = new Vector2(horizontal, 0);

        if (horizontal > 0 && !isFacingRight) Flip(); else if (horizontal < 0 && isFacingRight) Flip();

        timer += Time.deltaTime;
    }
}
