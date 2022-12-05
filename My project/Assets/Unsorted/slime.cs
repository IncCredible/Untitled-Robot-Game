using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slime : MonoBehaviour
{
    public bool active;
    public GameObject player;
    private int locate_f;
    private float Timer;
    private float T_act;
    private float xspd;
    private float yspd;
    public float MaxHor;
    public float Speed;
    public float MaxVert;
    public float Damag;
    public float Min;
    public float vertFactor;
    public float ShieldingPlayer;
    public bool King;
    public float PushY;
    public float PushX;
    private Rigidbody2D body;
    private bool Grounded;
    public GameObject Mini;
    private PlayerMove pm;
    private float tick;
    public bool Agressive;
    public bool Active;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.transform.tag == "Ground")
            Grounded = true;
    }
    private void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.transform.tag == "Ground") { 
        if (King) for (int i = 0; i < 1; i++)
                Instantiate(Mini, transform.position, Quaternion.identity);
        Grounded = false;
        }
    }
    private void OnTriggerStay2D(Collider2D coll)
    {
        pm = player.GetComponent<PlayerMove>();
        if (pm.ShieldBreakTime < pm.Timer)
        {
            if (coll.transform.tag == "Player" && Agressive)
            {
                pm.ShieldBreakTime = pm.Timer + ShieldingPlayer;
                pm.ChangeHP(Damag, true, true);
                if (player.transform.position.x > transform.position.x)
                    pm.GetPunch(new Vector2(PushX, PushY));
                else pm.GetPunch(new Vector2(-PushX, PushY));
            }
        }
    }
    void Update()
    {
        pm = player.GetComponent<PlayerMove>();
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        
        if (T_act < Timer && active && (King || Grounded))
        {
            T_act = Timer + Speed;
            xspd = Mathf.Clamp((player.transform.position.x - transform.position.x) * Min,
                -MaxHor, MaxHor);
            yspd = Mathf.Clamp((player.transform.position.y - transform.position.y) * vertFactor + 8,
                3, MaxVert);
            body.velocity += new Vector2(xspd, yspd);
        }
        Timer += Time.deltaTime;
    }
}
