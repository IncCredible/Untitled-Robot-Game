using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeCopter : MonoBehaviour
{
    public LayerMask IgnorePlayers;
    private Rigidbody2D body;
    public float Damag;
    private PlayerMove pm;
    public float ShieldingPlayer;
    public GameObject player;
    public float PushY;
    public float PushX;
    public float LookDist;
    public float MaxSpeed;
    private bool Attack;
    private float change_t;
    private Vector2 dir_noAt;
    private float Timer;
    public float AttackLookDist;
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
                pm.ChangeHP(Damag, true, true);
                if (player.transform.position.x > transform.position.x)
                    pm.GetPunch(new Vector2(PushX, PushY));
                else pm.GetPunch(new Vector2(-PushX, PushY));
            }
        }
    }
    private Vector2 dir;
    private void FixedUpdate()
    {
        if (Attack)
        {
            dir = new Vector2(player.transform.position.x - transform.position.x,
            player.transform.position.y - transform.position.y);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1000f, ~IgnorePlayers);

            body.AddForce(new Vector2((player.transform.position.x - transform.position.x) /
                hit.distance * body.mass * MaxSpeed,
            (player.transform.position.y - transform.position.y) /
                hit.distance * body.mass * MaxSpeed)); ;
        }
        else body.AddForce(dir_noAt);
    }
    private void Update()
    {
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        if (Active)
        {
            dir = new Vector2(player.transform.position.x - transform.position.x,
                player.transform.position.y - transform.position.y);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1000f, ~IgnorePlayers);
            if ((hit.collider.transform.tag == "Player") &&
                (!Attack && (LookDist > hit.distance) || (Attack && (AttackLookDist > hit.distance))))
                Attack = true;
            else Attack = false;
            if (Timer > change_t)
            {
                change_t = Timer + 1 + Random.Range(0, 2);
                dir_noAt = new Vector2(Random.Range(0f, 0.1f), Random.Range(0f, 0.1f));
            }
            Timer += Time.deltaTime;
        }
        else dir_noAt = new Vector2(0, 0);
    }
}
