using UnityEngine;

public class Player : CharacterScript
{

    private UIManager _uiManager;
    private void Awake()
    {

        _uiManager = FindObjectOfType<UIManager>();

        if (_characterStats != null)
        {
            _health = _characterStats.Health;
            _maxHealth = _characterStats.MaxHealth;
        }
        else
            Debug.LogError("CharacterStats is not Found");

        UpdateUI();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        UpdateUI();
    }

    public override void KillCharacter()
    {
        base.KillCharacter();
        EventBus.CharacterDied();
    }

    private void UpdateUI()
    {
        if (_uiManager != null)
        {
            _uiManager.UpdateHealthDisplay(_health, _maxHealth);
        }
    }
}