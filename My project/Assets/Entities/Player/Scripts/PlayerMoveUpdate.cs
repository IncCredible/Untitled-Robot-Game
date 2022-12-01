using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveUpdate : MonoBehaviour
{
    public float JumpMomentum;
    public float JumpDur;
    public float JumpForce;
    public float DashSpeed;
    public float DashDur;
    public float DashRelaxTime;
    public float Speed;
    public float AirMovingFactor;
    public float SprintFactor;
    public float Damage;
    public float SmoothWalkFactor;
    public float clingForce;
    public float wallJumpMomentum;
    public float movementblock;
    public float platformspeed;
    public float hit_reload;
    public float hit_scale;
    public float hit_direction;

    public bool doublejump;
    public bool isFacingRight;
    public bool dash;
    public bool sprint;
    public bool wallcling;
    public bool can_hit;
    public bool isHitting;

    public KeyCode JumpKey;
    public KeyCode SprintKey;
    public KeyCode RightKey;
    public KeyCode LeftKey;
    public KeyCode UpKey;
    public KeyCode DownKey;
    public KeyCode DashKey;
    public KeyCode HitKey;

    public GameObject spawnpoint;
    public GameObject swordstraight;
    public GameObject swordup;
    public GameObject sworddown;
    public GameObject swordwall;

    // ----------------------

    private float FixedSpeed;
    private float Timer;
    private float InteruptionJumpTime;
    private float InteruptionDashTime;
    private float movebreak_t;
    private float ForceFactor;
    private float SmoothWalkTimer;
    private float SmoothFactor;
    private float smoothWalkFactor;
    private float dashrelax_t;
    private float hit_t;

    private int horizontal;
    private int dashdirection;

    private bool walltouch;
    private bool dashing;
    private bool doublejump_f;
    private bool dash_f;
    private bool Grounded;
    private bool CheckJump;
    private bool CheckDash;

    private Rigidbody2D body;
    private BoxCollider2D col;
    Animator anim;

    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.transform.tag == "Ground")
        {
            Grounded = true;
            doublejump_f = true;
            dash_f = true;
            anim.SetInteger("vertDir", 0);
        }
    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.transform.tag == "Ceiling")
        {
            ForceFactor = 0;
            CheckJump = false;
            InteruptionDashTime = 0;
            body.velocity = new Vector2(body.velocity.x, 0);
        }
        if ((coll.transform.tag == "SmoothWall") && dashing) InteruptionDashTime = 0;
        if (coll.transform.tag == "Wall")
        {
            if (wallcling)
            {
                walltouch = true; doublejump_f = true; dash_f = true;
                CheckJump = false;
                body.velocity = new Vector2(0, 0);
            }
            if (dashing) InteruptionDashTime = 0;
        }
        if (coll.transform.tag == "Ground")
            SmoothWalkTimer = smoothWalkFactor;
        if (coll.transform.tag == "Spike")
            transform.position = spawnpoint.transform.position;
    }
    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.transform.tag == "Ground") Grounded = false;
        if (coll.transform.tag == "Wall") walltouch = false;
    }
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
        transform.position = spawnpoint.transform.position;
    }
    void FixedUpdate()
    {

        if (walltouch) body.AddForce(new Vector3(0, 1, 0) * body.mass * clingForce);
        body.AddForce(new Vector3(0, ForceFactor, 0) * body.mass * JumpForce);
    }
    void Update()
    {
        anim.SetBool("isRun", false);
        if (!walltouch && !Grounded)
        {
            if (body.velocity.y > 0) anim.SetInteger("vertDir", 1);
            else anim.SetInteger("vertDir", -1);
        }
        if (!dashing)
        {
            if (walltouch)
            {
                anim.SetBool("isCling", true);
                if (Input.GetKeyDown(JumpKey))
                {
                    if (isFacingRight)
                        body.velocity =
                            new Vector2(-wallJumpMomentum, JumpMomentum);
                    else body.velocity =
                        new Vector2(wallJumpMomentum, JumpMomentum);
                    movebreak_t = Timer + movementblock;
                    { horizontal *= -1; Flip(); }
                    SmoothWalkTimer = smoothWalkFactor - movementblock;
                    InteruptionJumpTime = Timer + JumpDur;
                    CheckJump = true;
                }
            }
            else anim.SetBool("isCling", false);
            if (Grounded)
            {
                if (sprint)
                {
                    if (Input.GetKey(SprintKey))
                    {
                        FixedSpeed = Speed * SprintFactor;
                        smoothWalkFactor = SmoothWalkFactor / 1.5f;
                        anim.speed = SprintFactor;
                    }
                    else
                    {
                        FixedSpeed = Speed;
                        smoothWalkFactor = SmoothWalkFactor;
                    }
                }
                else
                {
                    FixedSpeed = Speed;
                    smoothWalkFactor = SmoothWalkFactor;
                }
                if (Input.GetKeyDown(JumpKey))
                {
                    body.velocity = new Vector2(body.velocity.x, JumpMomentum);
                    InteruptionJumpTime = Timer + JumpDur;
                    CheckJump = true;
                }
            }
            else
            {
                FixedSpeed = Speed / AirMovingFactor;
                if (Input.GetKeyDown(JumpKey) && doublejump_f &&
                    doublejump && !walltouch)
                {
                    body.velocity =
                        new Vector2(body.velocity.x, JumpMomentum);
                    InteruptionJumpTime = Timer + JumpDur;
                    doublejump_f = false;
                    CheckJump = true;
                }
            }
            if (InteruptionJumpTime > Timer) if (Input.GetKey(JumpKey) && CheckJump)
                    ForceFactor = (InteruptionJumpTime - Timer) / JumpDur;
                else ForceFactor = 0;
            if (Input.GetKeyUp(JumpKey)) CheckJump = false;

            SmoothFactor = (SmoothWalkTimer - smoothWalkFactor) / SmoothWalkTimer;
            if (movebreak_t < Timer)
            {
                if (Input.GetKeyDown(LeftKey) || Input.GetKeyDown(RightKey))
                {
                    SmoothWalkTimer = smoothWalkFactor;
                }
                if (Input.GetKey(LeftKey))
                {
                    horizontal = -1; body.velocity =
                        new Vector2(-FixedSpeed * SmoothFactor + platformspeed, body.velocity.y);
                    if (Grounded) anim.SetBool("isRun", true);
                }
                else if (Input.GetKey(RightKey))
                {
                    horizontal = 1; body.velocity =
                        new Vector2(FixedSpeed * SmoothFactor + platformspeed, body.velocity.y);
                    if (Grounded) anim.SetBool("isRun", true);
                }
            }
            else horizontal = 0;
            if (horizontal > 0 && !isFacingRight) Flip();
            else if (horizontal < 0 && isFacingRight) Flip();
        }
        if (dash)
        {
            if (Input.GetKeyDown(DashKey) && (dash_f) &&
                (dashrelax_t < Timer))
            {
                col.size = new Vector2(0.5f, 0.7f);
                anim.SetBool("isDash", true);
                if (walltouch) { horizontal *= -1; Flip(); }
                dashrelax_t = Timer + DashRelaxTime;
                InteruptionDashTime = Timer + DashDur; CheckDash = true;
                dash_f = false;
                body.gravityScale = 0;
                if (isFacingRight) dashdirection = 1;
                else dashdirection = -1;

            }
            if (InteruptionDashTime > Timer) if (Input.GetKey(DashKey) && CheckDash)
                {
                    ForceFactor = 0;
                    body.velocity = new Vector2(DashSpeed * dashdirection, 0);
                    dashing = true;
                }
            if ((InteruptionDashTime < Timer) && dashing ||
                ((Input.GetKeyUp(DashKey)) && dashing))
            {
                col.size = new Vector2(0.5f, 1.35f);
                anim.SetBool("isDash", false);
                CheckDash = false; dashing = false;
                body.velocity = new Vector2(0, 0);
                body.gravityScale = 5;
            }
        }
        if (!dashing && can_hit && !Input.GetKey(SprintKey) &&
            (hit_t + hit_reload < Timer) && Input.GetKeyDown(HitKey))
        {
            hit_t = Timer + hit_reload;
            anim.speed = 0.25f / hit_reload;
            anim.SetBool("isHit", true);
            isHitting = true;
            if (Input.GetKey(UpKey)) swordup.SetActive(true);
            else if (Input.GetKey(DownKey) && !Grounded) sworddown.SetActive(true);
            else if (walltouch) swordwall.SetActive(true);
            else if (!Input.GetKey(UpKey)) swordstraight.SetActive(true);
        }
        if (isHitting && (hit_t < Timer))
        {
            anim.speed = 1;
            anim.SetBool("isHit", false);
            isHitting = false;
            sworddown.SetActive(false);
            swordup.SetActive(false);
            swordstraight.SetActive(false);
        }

        SmoothWalkTimer += Time.deltaTime;
        Timer += Time.deltaTime;
    }
}