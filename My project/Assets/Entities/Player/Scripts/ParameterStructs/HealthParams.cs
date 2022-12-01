using UnityEngine;

[System.Serializable]
public struct HealthParams
{
    public GameObject bar;
    public GameObject energyBar;
    public float maxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = Mathf.Clamp(value, 1, float.MaxValue); }
    }
    public float health
    {
        get { return _health; }
        set { _health = Mathf.Clamp(value, 0, _maxHealth); }
    }
    public float defence
    {
        get { return _defence; }
        set { _defence = Mathf.Clamp(value, 1, float.MaxValue); }
    }
    public float maxEnergy
    {
        get { return _maxEnergy; }
        set { _maxEnergy =  Mathf.Clamp(value, 0, float.MaxValue); }
    }
    public float energy
    {
        get { return _energy; }
        set { _energy = Mathf.Clamp(value, 0, _maxEnergy); }
    }
    public float regenSpeed
    {
        get { return _regenSpeed; }
        set { _regenSpeed = Mathf.Clamp(value, 0, float.MaxValue); }
    }
    public float regenCooldown
    {
        get { return _regenCooldown; }
        set { _regenCooldown = Mathf.Clamp(value, 0, float.MaxValue); }
    }

    [SerializeField] private float _health;
    [SerializeField] private float _defence;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _maxEnergy;
    [SerializeField] private float _energy;
    [SerializeField] private float _regenCooldown;
    [SerializeField] private float _regenSpeed;

    public void RegenerateEnergy(float _timer, float _regenStartTime)
    {
        if (_regenStartTime < _timer) 
            energy += regenSpeed * (health / maxHealth + (maxHealth - health) / (2 * maxHealth)) 
            * Time.deltaTime;
    }

    public void BarHandler()
    {
        bar.transform.localScale = new Vector2(health / maxHealth * 2, 0.1f);
        energyBar.transform.localScale = new Vector2(energy / maxEnergy * 2, 0.1f);
    }
}
