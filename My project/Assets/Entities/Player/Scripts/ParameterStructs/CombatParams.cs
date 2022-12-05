using UnityEngine;

[System.Serializable]
public struct CombatParams
{
    public KeyCode hitKey;
    public KeyCode defKey;

    public GameObject swordStraight;
    public GameObject swordUp;
    public GameObject swordDown;
    public GameObject swordWall;

    public bool canHit;
    public string weapon;
    public float knockback;
    public float damage;
    public float hitReload;
    public float hitScale;
    public float hitAccel;
    public float energyUsage;
}