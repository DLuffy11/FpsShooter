using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public bool IsDead => _isDead;

    [SerializeField] protected CharacterStatsSO _characterStats;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected CharacterController _characterController; // Add this line
    protected bool _isDead;

    [ReadOnly, SerializeField] protected int _health;
    [ReadOnly, SerializeField] protected int _maxHealth;

    private void OnEnable()
    {
        _isDead = false;
    }

    public virtual void TakeDamage(int damage)
    {
        if ((_health - damage) <= 0)
        {
            _health = 0;
            KillCharacter();
            return;
        }

        _health -= damage;
        // Animation for taking damage (optional)
        /*if (_animator != null)
        {
            _animator.SetTrigger("Hurt");
        }*/
    }

    public virtual void KillCharacter()
    {
        if (_animator != null)
            _animator.SetTrigger("Die");

        _isDead = true;

        if (_characterController != null)
            _characterController.enabled = false;

        foreach (var script in GetComponents<MonoBehaviour>())
        {
            if (script != this)
            {
                script.enabled = false;
            }
        }
    }



    public void CharacterDies()
    {
        Destroy(gameObject, 1f);
    }
}
