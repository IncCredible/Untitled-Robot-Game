using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeCube : MonoBehaviour
{
    public LayerMask IgnorePlayers;
    public float JumpRate;
    public Vector2 JumpVector;
    public int Direction = 1;
    private float Timer;
    private float NextJump_t;
    private Rigidbody2D body;
    public float Dist;
    public float Damag;
    private PlayerMove pm;
    public float ShieldingPlayer;
    public GameObject player;
    public float PushY;
    public float PushX;
    private bool Grounded;
    public bool Agressive;
    public bool Active;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnTriggerStay2D(Collider2D coll)
    {
        pm = player.GetComponent<PlayerMove>();
        if (pm.shieldBreakTime < pm.timer)
        {
            if (coll.transform.tag == "Player" && Agressive)
            {
                pm.dashContiniusFlag = false;
                pm.dashBreakTime = 0;
                pm.jumpContiniusFlag = false;
                pm.shieldBreakTime = pm.timer + ShieldingPlayer;
                pm.ChangeHP(Damag, true, false);
                if (player.transform.position.x > transform.position.x)
                    pm.GetPunch(new Vector2(PushX, PushY));
                else pm.GetPunch(new Vector2(-PushX, PushY));
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.transform.tag == "Ground")
            Grounded = true;
    }
    private void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.transform.tag == "Ground")
            Grounded = false;

    }
    private void Update()
    {
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        if (Active)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(Direction * 2f, 0, 0),
                    Vector2.down, 1000f, ~IgnorePlayers);
            RaycastHit2D wall = Physics2D.Raycast(transform.position + new Vector3(Direction * 0.6f, 0, 0),
                    new Vector2(Direction, 0), 1000f, ~IgnorePlayers);
            if (NextJump_t < Timer && Grounded)
            {
                NextJump_t += JumpRate + Random.Range(0f, 1f);
                if ((hit.distance > Dist) || (wall.distance < 1f)) Direction *= -1;
                body.velocity += new Vector2(JumpVector.x * Direction, JumpVector.y);
            }
            Timer += Time.deltaTime;
        }
    }
}
