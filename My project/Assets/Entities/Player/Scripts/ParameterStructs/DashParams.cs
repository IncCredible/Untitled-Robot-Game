using UnityEngine;

[System.Serializable]
public struct DashParams
{
    public bool canDash;
    public float speed;
    public float duration;
    public float cooldown;
    public float energyUsage;
    public KeyCode key;
}
