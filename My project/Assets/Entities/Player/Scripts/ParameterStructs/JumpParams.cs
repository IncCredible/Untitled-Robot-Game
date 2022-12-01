using UnityEngine;

[System.Serializable]
public struct JumpParams
{
    public bool canDoubleJump;
    public float momentum;
    public float force;
    public float energyUsage;
    public float duration;
    public KeyCode key;
}


