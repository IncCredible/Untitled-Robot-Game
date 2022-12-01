using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyHealth : MonoBehaviour
{
    public float Energy;
    public float MaxEnergy;
    public float MaxCurrent;
    public float RegenSpeed;
    public float RegenCooldown;
    public float ExtraRegen;
    public float ExtraSpeed;
    public float Defence;
    public Slider Maxbar;
    public Slider Currbar;
    public bool dead = false;
    public float timer = 0;
    public float cooldown = 0;

    void Damage(float val)
    {
        MaxCurrent -= val / Defence;
    }

    void Exhausing(float val)
    {
        Energy -= val;
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.transform.tag == "Spike")
        {
            Damage(10 * Time.deltaTime);
        }
    }

    void Update()
    {

        if (MaxCurrent > MaxEnergy) MaxCurrent = MaxEnergy;
        if (Energy > MaxCurrent) Energy = MaxCurrent;
        if (MaxCurrent < ExtraRegen) MaxCurrent += ExtraSpeed * Time.deltaTime;
        if ((Energy < MaxCurrent) && (timer > cooldown)) Energy += RegenSpeed * Time.deltaTime;
        if (MaxCurrent <= 0) dead = true;
        if (Energy < 0) Energy = 0;
        if (MaxCurrent <= 0)
        {
            transform.position = new Vector2(0, 0);
            MaxCurrent = MaxEnergy;
        }

        Currbar.value = Energy / MaxEnergy;
        Maxbar.value = MaxCurrent / MaxEnergy;
        timer += Time.deltaTime;
    }
}
