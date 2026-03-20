using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{

    public event EventHandler OnDead;
    public event EventHandler OnDamaged;


    [SerializeField] private int health = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] bool regenHealth;
    [SerializeField] bool hasCorruptionResist;
    public void InitCorruptionResist(bool value) { hasCorruptionResist = value; }
    public bool HasCorruptionResist() { return hasCorruptionResist; }


    private void Awake()
    {
        currentHealth = health;
    }

    public void InitHP(int hp)
    {
        health = hp;
        currentHealth = health;
    }

    public void InitRegenHealth(bool value)
    {
        regenHealth = value;
    }

    public void Damage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        OnDamaged?.Invoke(this, EventArgs.Empty);

        if (currentHealth == 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;

        if(currentHealth > 100) currentHealth = 100;
    }

    private void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return (float)currentHealth / health;
    }

}
