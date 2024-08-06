using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "ScriptableObjects/Character")]
public class CharacterStatsSO : ScriptableObject
{
    public int Health => _health;
    public int MaxHealth => _maxHealth;
    public int MovementSpeed => _movementSpeed;

    [SerializeField] private int _health;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _movementSpeed;

}
