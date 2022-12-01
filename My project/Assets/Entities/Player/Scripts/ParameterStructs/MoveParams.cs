using UnityEngine;

[System.Serializable]
public struct MoveParams
{
    public KeyCode sprintKey;
    public KeyCode rightKey;
    public KeyCode leftKey;
    public KeyCode upKey;
    public KeyCode downKey;
    public float maxSpeed;
    public float airMovingResist;
    public float force;
    public bool canSprint;
    public float sprintFactor;
    public float sprintEnergyUsage;
}
