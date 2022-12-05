using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int SwordType;
    public float Width;
    public float RecoilValueY;
    public float RecoilValueX;
    public float Length;
    public GameObject Owner;

    private bool _Flag;
    private float _Reload;
    private float _Scale;
    private Color _Col;
    private PlayerMove _PlayerMove;
    private Animator _Anim;

    private void Start()
    {
        _Anim = GetComponent<Animator>();
        _Col = GetComponent<SpriteRenderer>().color;
        _PlayerMove = Owner.GetComponent<PlayerMove>();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if ((string.Equals(coll.transform.tag, "Spike", System.StringComparison.CurrentCultureIgnoreCase)
            || string.Equals(coll.transform.tag, "KingSlime", System.StringComparison.CurrentCultureIgnoreCase)) 
            && (SwordType == 1))
        {
            _PlayerMove.VerticalRecoil(RecoilValueY);
            _PlayerMove.DashFlag = true;
            _PlayerMove.DoubleJumpFlag = true;
        }
        if (string.Equals(coll.transform.tag, "Spike", System.StringComparison.CurrentCultureIgnoreCase)
            || string.Equals(coll.transform.tag, "KingSlime", System.StringComparison.CurrentCultureIgnoreCase)
            || string.Equals(coll.transform.tag, "Wall", System.StringComparison.CurrentCultureIgnoreCase)
            && (SwordType == 2))
            _PlayerMove.HorizontakRecoil(RecoilValueX);

    }

    private void GetDataFromPlayerMove()
    {
        _PlayerMove = Owner.GetComponent<PlayerMove>();
        _Reload = _PlayerMove.CombatParams.hitReload;
        _Scale = _PlayerMove.CombatParams.hitScale;
        _Flag = _PlayerMove.States.isHitting;
    }

    private void Update()
    {
        GetDataFromPlayerMove();
        transform.localScale = new Vector2(_Scale * Width, _Scale * Length);
        if (_Flag) 
        { 
            _Col.a = 0.9f; 
            _Anim.speed = 0.25f / _Reload; 
        }
        else 
        { 
            _Anim.speed = 0;
            _Col.a = 0; 
        }
    }
}
