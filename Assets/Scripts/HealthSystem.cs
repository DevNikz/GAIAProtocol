using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{

    public event EventHandler OnDead;
    public event EventHandler OnDamaged;


    [SerializeField] private int health = 100;
    private int healthMax;
    bool regenHealth;
    bool hasCorruptionResist;
    public void InitCorruptionResist(bool value) { hasCorruptionResist = value; }


    private void Awake()
    {
        healthMax = health;
    }

    public void InitHP(int hp)
    {
        health = hp;
        healthMax = health;
    }

    public void InitRegenHealth(bool value)
    {
        regenHealth = value;
    }

    public void Damage(int damageAmount)
    {
        health -= damageAmount;

        if (health < 0)
        {
            health = 0;
        }

        OnDamaged?.Invoke(this, EventArgs.Empty);

        if (health == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return (float)health / healthMax;
    }

}
