using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sword : MonoBehaviour
{
    public GameObject Owner;
    public float wide;
    public float bounce;
    public float horbounce;
    public float range;
    public int type;
    private PlayerMove pm;
    Animator anim;
    private float h_rel;
    private float h_scale;
    private bool h_f;
    Color col;
    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<SpriteRenderer>().color;
        pm = Owner.GetComponent<PlayerMove>();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (((coll.transform.tag == "Spike") || (coll.transform.tag == "KingSlime")) && (type == 1))
        {
            pm.VerticalRecoil(bounce);
            pm.dashFlag = true;
            pm.doublejumpFlag = true;
        }
        if ((coll.transform.tag == "Wall") || (coll.transform.tag == "Spike") || (coll.transform.tag == "KingSlime")
            && (type == 2))
                    pm.HorizontakRecoil(horbounce);

    }

    void Update()
    {
        pm = Owner.GetComponent<PlayerMove>();
        h_rel = pm.combatParams.hitReload;
        h_scale = pm.combatParams.hitScale;
        h_f = pm.states.isHitting;
        transform.localScale = new Vector2(h_scale * wide, h_scale * range);
        if (!h_f) { anim.speed = 0; col.a = 0; }
        else { col.a = 0.9f; anim.speed = 0.25f / h_rel; }
    }
}
