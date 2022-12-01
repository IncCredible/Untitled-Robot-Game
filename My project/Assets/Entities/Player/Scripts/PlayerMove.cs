using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Main Parameters")]

    [SerializeField] public JumpParams jumpParams;
    [SerializeField] public DashParams dashParams;
    [SerializeField] public WallclingParams clingParams;
    [SerializeField] public MoveParams moveParams;
    [SerializeField] public CombatParams combatParams;
    [SerializeField] public HealthParams healthParams;

    [Header("Extra Parameters")]

    public bool isFacingRight;
    public States states;

    //related to movement

    private float moveBlockTime;
    public float moveBlockCooldown;
    public int horizontal;
    private Vector3 moveDirection;

    //related to combat

    public Vector2 hitDirection;
    private float hitBlockTime;

    //related to dash

    [HideInInspector] public bool dashFlag;
    [HideInInspector] public float dashBreakTime;
    [HideInInspector] public bool dashContiniusFlag;
    private float dashCooldown;
    private int dashDirectionX;

    //related to jump

    [HideInInspector] public bool doublejumpFlag;
    private float jumpBreakTime;
    [HideInInspector] public bool jumpContiniusFlag;
    private float jumpCurrentForce;

    // other

    [HideInInspector] public float timer;
    [HideInInspector] public float shieldBreakTime;
    private float regenStartTime;
    private Rigidbody2D body;
    public GameObject spawnpoint;
    Animator anim;

    private void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.transform.tag == "Ground") 
        {
            states.isGrounded = true;
            doublejumpFlag = true;
            dashFlag = true;
            anim.SetInteger("vertDir", 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.transform.tag == "Ceiling")
        {
            jumpCurrentForce = 0;
            jumpContiniusFlag = false;
            dashBreakTime = 0;
            body.velocity = new Vector2(body.velocity.x, 0);
        } 

        if ((coll.transform.tag == "SmoothWall") && states.isDashing) dashBreakTime = 0;

        if (coll.transform.tag == "Wall")
        {
            if (clingParams.canWallCling && !states.isGrounded) 
            {
                states.isClinging = true; 
                doublejumpFlag = true; 
                dashFlag = true;
                jumpContiniusFlag = false;
                body.velocity = new Vector2(0, 0);
            }
            if (states.isDashing) dashBreakTime = 0;
        }

        if (coll.transform.tag == "Ground") anim.SetTrigger("Ground");
    }

    private void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.transform.tag == "Ground") states.isGrounded = false;
        if (coll.transform.tag == "Wall") states.isClinging = false;
    }

    private void GetDamage(float value)
    {
        ChangeEnergy(0, false);
        healthParams.health -= value / healthParams.defence;
        moveBlockTime = timer + moveBlockCooldown;
        dashContiniusFlag = false;
        dashBreakTime = 0;
        jumpContiniusFlag = false;
    }

    public void ChangeHP(float value, bool isHit, bool isBodytouchDamage)
    {
        if (isHit)
        {
            if (isBodytouchDamage) GetDamage(value);
            else
            {
                if (states.isDefencing)  ChangeEnergy(value / healthParams.defence, false);
                else  GetDamage(value);          
            }
        }
        else healthParams.health += value;
    }

    public void ChangeEnergy(float value, bool durable=false)
    {
        healthParams.energy += value;
        if (!durable) regenStartTime = timer + healthParams.regenCooldown;
    }

    public void GetPunch(Vector2 dir)
    {
        if (states.isDefencing) body.velocity += new Vector2(dir.x / 2, 0);
        else body.velocity = dir;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void VerticalRecoil(float recoil)
    {
        body.velocity = new Vector2(body.velocity.x, recoil);
    }

    public void HorizontakRecoil(float recoil)
    {
        body.velocity -= new Vector2(horizontal * recoil, 0);
    }

    private void Death()
    {
        if (healthParams.health <= 0)
        {
            healthParams.health = healthParams.maxHealth;
            transform.position = spawnpoint.transform.position;
        }
    }

    private bool canSprint
    {
        get
        {
            return moveParams.canSprint
                && Input.GetKey(moveParams.sprintKey)
                && (moveParams.sprintEnergyUsage < healthParams.energy);
        }
    }

    private void doJump(Vector2 _jumpVector)
    {
        anim.SetTrigger("Jump");
        ChangeEnergy(-jumpParams.energyUsage, false);
        body.velocity = _jumpVector;
        jumpBreakTime = timer + jumpParams.duration;
        jumpContiniusFlag = true;
    }

    private void Jump()
    {
        var canJump = !states.isDashing
                && jumpParams.energyUsage < healthParams.energy
                && Input.GetKeyDown(jumpParams.key);

        if (canJump)
        {
            if (states.isClinging)
            {
                horizontal *= -1;
                Flip();
                moveBlockTime = timer + moveBlockCooldown;
                doJump(new Vector2(Convert.ToInt16(isFacingRight) * clingParams.momentum, 
                    jumpParams.momentum));
            }
            else if (states.isGrounded)
            {
                doJump(new Vector2(body.velocity.x, jumpParams.momentum));
            }
            else if (doublejumpFlag)
            {
                doublejumpFlag = false;
                doJump(new Vector2(body.velocity.x, jumpParams.momentum));
            }
        }
        if (jumpBreakTime > timer)
        {
            if (Input.GetKey(jumpParams.key) && jumpContiniusFlag)
            {
                jumpCurrentForce = (jumpBreakTime - timer) / jumpParams.duration;
            }
            else jumpCurrentForce = 0;
        }
        if (Input.GetKeyUp(jumpParams.key)) jumpContiniusFlag = false;
    }
    
    private void Run()
    {
        var canRun = !states.isDashing
                && timer >= moveBlockTime;

        if (canRun)
        {
            anim.SetBool("isRun", true);
            if (Input.GetKey(moveParams.leftKey)) horizontal = -1;
            else if (Input.GetKey(moveParams.rightKey)) horizontal = 1;
            else
            {
                horizontal = 0;
                anim.SetBool("isRun", false);
            }
            moveDirection = new Vector2(horizontal, 0);

            if (horizontal > 0 && !isFacingRight) Flip();
            else if (horizontal < 0 && isFacingRight) Flip();
        }
        else
        {
            horizontal = 0;
            anim.SetBool("isRun", false);
        }

        if (!states.isDashing)
            body.velocity = new Vector2(
                Mathf.Clamp(body.velocity.x, -moveParams.maxSpeed, moveParams.maxSpeed),
                body.velocity.y);
    }

    private void Dash()
    {
        var canDash = dashParams.canDash
                && Input.GetKeyDown(dashParams.key)
                && (dashParams.energyUsage * dashParams.duration < healthParams.energy)
                && (dashCooldown < timer)
                && (dashFlag);

        if (canDash)
        {
            anim.SetBool("isDash", true);
            if (states.isClinging) { horizontal *= -1; Flip(); }
            dashCooldown = timer + dashParams.cooldown;
            dashBreakTime = timer + dashParams.duration; dashContiniusFlag = true;
            dashFlag = false;
            body.gravityScale = 0;
            if (isFacingRight) dashDirectionX = 1;
            else dashDirectionX = -1;
        }

        if (dashBreakTime >= timer) if (Input.GetKey(dashParams.key) && dashContiniusFlag)
        {
            ChangeEnergy(-dashParams.energyUsage * Time.deltaTime, false);
            jumpCurrentForce = 0;
            body.velocity = new Vector2(dashParams.speed * dashDirectionX, 0);
            states.isDashing = true;
        }

        if ((dashBreakTime < timer || Input.GetKeyUp(dashParams.key)) && states.isDashing)
        {
            anim.SetBool("isDash", false);
            dashContiniusFlag = false; states.isDashing = false;
            body.velocity = new Vector2(0, 0);
            body.gravityScale = 5;
        }
    }

    private void DoHit()
    {
        if (Input.GetKey(moveParams.upKey))
        {
            combatParams.swordUp.SetActive(true);
            hitDirection = Vector2.up;
        }
        else if (states.isClinging && !Input.GetKey(moveParams.upKey))
        {
            combatParams.swordWall.SetActive(true);
            hitDirection = Vector2.right;
        }
        else if (Input.GetKey(moveParams.downKey) && !states.isGrounded)
        {
            combatParams.swordDown.SetActive(true);
            hitDirection = Vector2.down;
        }
        else if (!Input.GetKey(moveParams.upKey))
        {
            combatParams.swordStraight.SetActive(true);
            hitDirection = Vector2.right;
        }
    }

    private void Hit()
    {
        var canHit = combatParams.canHit
                && Input.GetKeyDown(combatParams.hitKey)
                && (hitBlockTime + combatParams.hitReload * combatParams.hitAccel < timer)
                && !states.isDashing
                && (combatParams.energyUsage < healthParams.energy);

        if (canHit)
        {
            anim.speed = 0.25f / combatParams.hitReload;
            anim.SetBool("isHit", true);

            ChangeEnergy(-combatParams.energyUsage, false);
            hitBlockTime = timer + combatParams.hitReload;
            states.isHitting = true;
            DoHit();
        }
        if (states.isHitting && (hitBlockTime < timer))
        {
            anim.speed = 1;
            anim.SetBool("isHit", false);

            states.isHitting = false;
            combatParams.swordDown.SetActive(false);
            combatParams.swordUp.SetActive(false);
            combatParams.swordWall.SetActive(false);
            combatParams.swordStraight.SetActive(false);
        }
        if (!states.isHitting && Input.GetKey(combatParams.defKey))
        {
            states.isDefencing = true;
        }
        else states.isDefencing = false;
    }

    private void AnimateFlying()
    {
        if (!states.isClinging && !states.isGrounded)
        {
            if (body.velocity.y > 0.05f) anim.SetInteger("vertDir", 1);
            else if (body.velocity.y < -0.05f) anim.SetInteger("vertDir", -1);
            anim.SetBool("isRun", false);
        }
        else anim.SetInteger("vertDir", 0);
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        transform.position = spawnpoint.transform.position;
    }

    void FixedUpdate()
    {
        if (states.isClinging) 
            body.AddForce(new Vector3(0, 1, 0) * body.mass * clingParams.force);

        body.AddForce(new Vector3(0, jumpCurrentForce, 0) * body.mass * jumpParams.force);

        if (canSprint)
        {
            body.AddForce(moveDirection * body.mass * moveParams.force * moveParams.sprintFactor
                * Time.deltaTime);
            ChangeEnergy(-moveParams.sprintEnergyUsage * Time.deltaTime, false);
        }
        else body.AddForce(moveDirection * body.mass * moveParams.force * Time.deltaTime);
    }

    void Update()
    {
        AnimateFlying();

        Hit();
        Dash();
        Jump();
        Run();

        healthParams.RegenerateEnergy(timer, regenStartTime);
        healthParams.BarHandler();
        Death();

        timer += Time.deltaTime;
    }
}

