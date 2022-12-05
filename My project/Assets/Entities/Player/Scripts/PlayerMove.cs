using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    #region Public main fields
    [Header("Main Parameters")]

    [SerializeField] public JumpParams JumpParams;
    [SerializeField] public DashParams DashParams;
    [SerializeField] public WallclingParams ClingParams;
    [SerializeField] public MoveParams MoveParams;
    [SerializeField] public CombatParams CombatParams;
    [SerializeField] public HealthParams HealthParams;
    #endregion

    #region Public fields
    [Header("Extra Parameters")]

    public bool IsFacingRight;
    [HideInInspector] public bool DashFlag;
    [HideInInspector] public bool DashContiniousFlag;
    [HideInInspector] public bool DoubleJumpFlag;
    [HideInInspector] public bool JumpContiniusFlag;
    public float MoveBlockCooldown;
    [HideInInspector] public float DashBreakTime;
    [HideInInspector] public float Timer;
    [HideInInspector] public float ShieldBreakTime;
    [HideInInspector] public Vector2 HitDirection;
    public GameObject Spawnpoint;
    public States States;
    #endregion

    #region Private fields
    private bool _canSprint
    {
        get
        {
            return MoveParams.canSprint
                && Input.GetKey(MoveParams.sprintKey)
                && (MoveParams.sprintEnergyUsage < HealthParams.energy);
        }
    }
    private int _DashDirectionX;
    private int _Horizontal;
    private float _MoveBlockTime;
    private float _JumpBreakTime;
    private float _JumpCurrentForce;
    private float _RegenStartTime;
    private float _HitBlockTime;
    private float _DashCooldown;
    private Vector3 _MoveDirection;
    private Vector3 _ClingForce
    {
        get
        {
            var speedFactor = _Body.velocity.y * 0.1f;
            return new Vector3(0, -1, 0) * _Body.mass * ClingParams.force * speedFactor;
        }
    }
    private Rigidbody2D _Body;
    private Animator _Anim;
    #endregion

    #region Public methods
    public void ChangeHP(float value, bool isHit, bool isBodytouchDamage)
    {
        if (isHit)
        {
            if (isBodytouchDamage) GetDamage(value);
            else
            {
                if (States.isDefencing)
                {
                    ChangeEnergy(-value / HealthParams.defence, false);
                    GetDamage(Mathf.Clamp(value - HealthParams.energy, 0, float.MaxValue));
                }
                else GetDamage(value);          
            }
        }
        else HealthParams.health += value;
    }

    public void ChangeEnergy(float value, bool durable=false)
    {
        HealthParams.energy += value;
        if (!durable) _RegenStartTime = Timer + HealthParams.regenCooldown;
    }

    public void GetPunch(Vector2 dir)
    {
        if (States.isDefencing) _Body.velocity += new Vector2(dir.x / 2, 0);
        else _Body.velocity = dir;
    }

    public void VerticalRecoil(float recoil)
    {
        _Body.velocity = new Vector2(_Body.velocity.x, recoil);
    }

    public void HorizontakRecoil(float recoil)
    {
        _Body.velocity -= new Vector2(_Horizontal * recoil, 0);
    }
    #endregion

    #region Private methods
    private void OnCollisionStay2D(Collision2D coll)
    {
        if (string.Equals(coll.transform.tag, "Ground", System.StringComparison.CurrentCultureIgnoreCase)) 
        {
            States.isGrounded = true;
            DoubleJumpFlag = true;
            DashFlag = true;
            _Anim.SetInteger("vertDir", 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (string.Equals(coll.transform.tag, "Ceiling", System.StringComparison.CurrentCultureIgnoreCase))
        {
            _JumpCurrentForce = 0;
            JumpContiniusFlag = false;
            DashBreakTime = 0;
            _Body.velocity = new Vector2(_Body.velocity.x, 0);
        } 

        if (string.Equals(coll.transform.tag, "SmoothWall", System.StringComparison.CurrentCultureIgnoreCase) 
            && States.isDashing) DashBreakTime = 0;

        if (string.Equals(coll.transform.tag, "Wall", System.StringComparison.CurrentCultureIgnoreCase))
        {
            if (ClingParams.canWallCling && !States.isGrounded) 
            {
                States.isClinging = true; 
                DoubleJumpFlag = true; 
                DashFlag = true;
                JumpContiniusFlag = false;
                _Body.velocity = new Vector2(0, 0);
            }
            if (States.isDashing) DashBreakTime = 0;
        }

        if (string.Equals(coll.transform.tag, "Ground", System.StringComparison.CurrentCultureIgnoreCase)) 
            _Anim.SetTrigger("Ground");
    }

    private void OnCollisionExit2D(Collision2D coll)
    {
        if (string.Equals(coll.transform.tag, "Ground", System.StringComparison.CurrentCultureIgnoreCase)) 
            States.isGrounded = false;
        if (string.Equals(coll.transform.tag, "Wall", System.StringComparison.CurrentCultureIgnoreCase)) 
            States.isClinging = false;
    }

    private void GetDamage(float value)
    {
        ChangeEnergy(0, false);
        HealthParams.health -= value / HealthParams.defence;
        DashContiniousFlag = false;
        DashBreakTime = 0;
        JumpContiniusFlag = false;
        if (value > 0)
        {
            _MoveBlockTime = Timer + MoveBlockCooldown;
        }
    }

    private void Flip()
    {
        IsFacingRight = !IsFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void Death()
    {
        if (HealthParams.health <= 0)
        {
            HealthParams.health = HealthParams.maxHealth;
            transform.position = Spawnpoint.transform.position;
        }
    }

    private void DoJump(Vector2 jumpVector)
    {
        _Anim.SetTrigger("Jump");
        ChangeEnergy(-JumpParams.energyUsage, false);
        _Body.velocity = jumpVector;
        _JumpBreakTime = Timer + JumpParams.duration;
        JumpContiniusFlag = true;
    }

    private void Jump()
    {
        var canJump = !States.isDashing
                && JumpParams.energyUsage < HealthParams.energy
                && Timer >= _MoveBlockTime
                && Input.GetKeyDown(JumpParams.key);

        if (canJump)
        {
            if (States.isClinging)
            {
                _Horizontal *= -1;
                Flip();
                _MoveBlockTime = Timer + MoveBlockCooldown;
                var direction = IsFacingRight == true ? 1:-1;
                DoJump(new Vector2(direction * ClingParams.momentum, JumpParams.momentum));
            }
            else if (States.isGrounded)
            {
                DoJump(new Vector2(_Body.velocity.x, JumpParams.momentum));
            }
            else if (DoubleJumpFlag)
            {
                DoubleJumpFlag = false;
                DoJump(new Vector2(_Body.velocity.x, JumpParams.momentum));
            }
        }
        if (_JumpBreakTime > Timer)
        {
            if (Input.GetKey(JumpParams.key) && JumpContiniusFlag)
            {
                _JumpCurrentForce = (_JumpBreakTime - Timer) / JumpParams.duration;
            }
            else _JumpCurrentForce = 0;
        }
        if (Input.GetKeyUp(JumpParams.key)) JumpContiniusFlag = false;
    }
    
    private void Run()
    {
        var canRun = !States.isDashing
                && Timer >= _MoveBlockTime;

        if (canRun)
        {
            _Anim.SetBool("isRun", true);
            if (Input.GetKey(MoveParams.leftKey)) _Horizontal = -1;
            else if (Input.GetKey(MoveParams.rightKey)) _Horizontal = 1;
            else
            {
                _Horizontal = 0;
                _Anim.SetBool("isRun", false);
            }
            _MoveDirection = new Vector2(_Horizontal, 0);

            if (_Horizontal > 0 && !IsFacingRight) Flip();
            else if (_Horizontal < 0 && IsFacingRight) Flip();
        }
        else
        {
            _MoveDirection = new Vector2(0, 0);
            _Anim.SetBool("isRun", false);
        }

        if (!States.isDashing)
            _Body.velocity = new Vector2(
                Mathf.Clamp(_Body.velocity.x, -MoveParams.maxSpeed, MoveParams.maxSpeed),
                _Body.velocity.y);
    }

    private void Dash()
    {
        var canDash = Input.GetKeyDown(DashParams.key)
                && DashParams.canDash
                && DashFlag
                && Timer >= _MoveBlockTime
                && _DashCooldown < Timer
                && DashParams.energyUsage * DashParams.duration < HealthParams.energy;

        if (canDash)
        {
            _Anim.SetBool("isDash", true);
            if (States.isClinging) { _Horizontal *= -1; Flip(); }
            _DashCooldown = Timer + DashParams.cooldown;
            DashBreakTime = Timer + DashParams.duration; DashContiniousFlag = true;
            DashFlag = false;
            _Body.gravityScale = 0;
            if (IsFacingRight) _DashDirectionX = 1;
            else _DashDirectionX = -1;
        }

        if (DashBreakTime >= Timer && Input.GetKey(DashParams.key) && DashContiniousFlag)
        {
            ChangeEnergy(-DashParams.energyUsage * Time.deltaTime, false);
            _JumpCurrentForce = 0;
            _Body.velocity = new Vector2(DashParams.speed * _DashDirectionX, 0);
            States.isDashing = true;
        }

        if ((DashBreakTime < Timer || Input.GetKeyUp(DashParams.key)) && States.isDashing)
        {
            _Anim.SetBool("isDash", false);
            DashContiniousFlag = false;
            States.isDashing = false;
            _Body.velocity = new Vector2(0, 0);
            _Body.gravityScale = 5;
        }
    }

    private void DoHit()
    {
        if (Input.GetKey(MoveParams.upKey))
        {
            CombatParams.swordUp.SetActive(true);
            HitDirection = Vector2.up;
        }
        else if (States.isClinging && !Input.GetKey(MoveParams.upKey))
        {
            CombatParams.swordWall.SetActive(true);
            HitDirection = Vector2.right;
        }
        else if (Input.GetKey(MoveParams.downKey) && !States.isGrounded)
        {
            CombatParams.swordDown.SetActive(true);
            HitDirection = Vector2.down;
        }
        else if (!Input.GetKey(MoveParams.upKey))
        {
            CombatParams.swordStraight.SetActive(true);
            HitDirection = Vector2.right;
        }
    }

    private void Hit()
    {
        var canHit = Input.GetKeyDown(CombatParams.hitKey)
                && !States.isDashing
                && CombatParams.canHit
                && Timer >= _MoveBlockTime
                && (CombatParams.energyUsage < HealthParams.energy)
                && (_HitBlockTime + CombatParams.hitReload * CombatParams.hitAccel < Timer);

        if (canHit)
        {
            _Anim.speed = 0.25f / CombatParams.hitReload;
            _Anim.SetBool("isHit", true);

            ChangeEnergy(-CombatParams.energyUsage, false);
            _HitBlockTime = Timer + CombatParams.hitReload;
            States.isHitting = true;
            DoHit();
        }
        if (States.isHitting && (_HitBlockTime < Timer))
        {
            _Anim.speed = 1;
            _Anim.SetBool("isHit", false);

            States.isHitting = false;
            CombatParams.swordDown.SetActive(false);
            CombatParams.swordUp.SetActive(false);
            CombatParams.swordWall.SetActive(false);
            CombatParams.swordStraight.SetActive(false);
        }
        if (!States.isHitting && Input.GetKey(CombatParams.defKey))
        {
            States.isDefencing = true;
        }
        else States.isDefencing = false;
    }

    private void AnimateFlying()
    {
        if (!States.isClinging && !States.isGrounded)
        {
            if (_Body.velocity.y > 0.05f) _Anim.SetInteger("vertDir", 1);
            else if (_Body.velocity.y < -0.05f) _Anim.SetInteger("vertDir", -1);
            _Anim.SetBool("isRun", false);
        }
        else _Anim.SetInteger("vertDir", 0);
    }

    private void Start()
    {
        _Body = GetComponent<Rigidbody2D>();
        _Anim = GetComponent<Animator>();
        transform.position = Spawnpoint.transform.position;
    }

    private void FixedUpdate()
    {
        if (States.isClinging) 
            _Body.AddForce(_ClingForce);

        _Body.AddForce(new Vector3(0, _JumpCurrentForce, 0) * _Body.mass * JumpParams.force);

        if (_canSprint)
        {
            _Body.AddForce(_MoveDirection * _Body.mass * MoveParams.force * MoveParams.sprintFactor
                * Time.deltaTime);
            ChangeEnergy(-MoveParams.sprintEnergyUsage * Time.deltaTime, false);
        }
        else _Body.AddForce(_MoveDirection * _Body.mass * MoveParams.force * Time.deltaTime);
    }

    private void Update()
    {
        AnimateFlying();

        Hit();
        Dash();
        Jump();
        Run();

        HealthParams.RegenerateEnergy(Timer, _RegenStartTime);
        HealthParams.BarHandler();
        Death();

        Timer += Time.deltaTime;
    }
    #endregion
}