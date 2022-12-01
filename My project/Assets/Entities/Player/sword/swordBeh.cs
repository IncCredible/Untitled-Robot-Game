using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swordBeh : MonoBehaviour
{
    public GameObject Owner;
    public float wide;
    public float range;
    public int type;
    private PlayerMove pm;
    Animator anim;
    private float h_rel;
    private float h_scale;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        pm = Owner.GetComponent<PlayerMove>();
        h_rel = pm.combatParams.hitReload;
        h_scale = pm.combatParams.hitScale;
        anim.speed = 0.25f / h_rel * 2;
        transform.localScale = new Vector2(h_scale * wide, h_scale * range);
    }
}
