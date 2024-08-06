using System;

public static class EventBus
{
    public static event Action OnAimPressed;
    public static event Action OnAimReleased;
    public static event Action OnCharacterDied;
    public static event Action OnEnemyDied;

    public static void AimPressed() => OnAimPressed?.Invoke();
    public static void AimReleased() => OnAimReleased?.Invoke();
    public static void CharacterDied() => OnCharacterDied?.Invoke();
    public static void EnemyDied() => OnEnemyDied?.Invoke();
}
