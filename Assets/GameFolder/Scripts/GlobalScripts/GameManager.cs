using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Enemy> _enemyList = new List<Enemy>();

    private void Awake()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        _enemyList.AddRange(enemies);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        EventBus.OnEnemyDied += HandleEnemyDied;
        EventBus.OnCharacterDied += HandleCharacterDied;
    }

    private void OnDisable()
    {
        EventBus.OnEnemyDied -= HandleEnemyDied;
        EventBus.OnCharacterDied -= HandleCharacterDied;
    }

    private void HandleEnemyDied()
    {
        Enemy deadEnemy = _enemyList.Find(e => e.IsDead);
        if (deadEnemy != null)
        {
            _enemyList.Remove(deadEnemy);
            
            if (_enemyList.Count == 0)
            {
                IncrementWinScore();
            }
        }
    }

    private void HandleCharacterDied()
    {
        IncrementLoseScore();
    }

    private void IncrementWinScore()
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.IncrementWinScore();
        }
    }

    private void IncrementLoseScore()
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.IncrementLoseScore();
        }
    }
}
