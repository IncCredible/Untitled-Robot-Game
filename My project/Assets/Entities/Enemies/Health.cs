using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float max_health;
    public float health;
    private PlayerMove pm;
    public GameObject player;
    private Rigidbody2D body;
    private float timer;
    public bool Invulnarable;

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.transform.tag == "PlayerHitbox" && !Invulnarable) {
            if (pm.isFacingRight) body.velocity +=
                    new Vector2(pm.combatParams.knockBack / body.mass * pm.hitDirection.x, 
                pm.combatParams.knockBack / body.mass * pm.hitDirection.y);
            else body.velocity += new Vector2(-pm.combatParams.knockBack / body.mass * pm.hitDirection.x, 
                pm.combatParams.knockBack / body.mass * pm.hitDirection.y);
            health -= pm.combatParams.damage / 2;
        }
    }
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        body = GetComponent<Rigidbody2D>();
    }
    public void Update()
    {
        pm = player.GetComponent<PlayerMove>();
        if (health <= 0) Destroy(gameObject);
    }
}
