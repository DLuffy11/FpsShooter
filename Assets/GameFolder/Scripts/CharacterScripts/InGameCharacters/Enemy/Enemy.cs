using UnityEngine;

public class Enemy : CharacterScript
{
    public EnemySO EnemyStats => _characterStats as EnemySO;
    private HealthBar _healthBar; 

    private void Awake()
    {

        _healthBar = GetComponent<HealthBar>();

        EnemySO enemyStats = _characterStats as EnemySO;

        if (enemyStats != null)
        {
            _health = enemyStats.Health;
            _maxHealth = enemyStats.MaxHealth;
            if (_healthBar != null)
            {
                _healthBar.SetHealth(_health, _maxHealth);
            }
        }
        else
            Debug.LogError("CharacterStats is not of type EnemySO.");
    }

    public override void KillCharacter()
    {
        base.KillCharacter();
        EventBus.EnemyDied();
    }

    private void UpdateHealthBar()
    {
        if (_healthBar != null)
        {
            _healthBar.SetHealth(_health, _maxHealth);
        }
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        UpdateHealthBar(); 
    }
}
