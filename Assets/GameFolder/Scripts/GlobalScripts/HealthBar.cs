using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;

    private Transform _playerTransform;

    #region Unity Methods
    private void Start()
    {
        InitializeHealthSlider();
        _playerTransform = Camera.main.transform;
    }

    private void Update()
    {
        OrientHealthBarTowardsPlayer();
    }
    #endregion

    public void SetHealth(float currentHealth, float maxHealth)
    {
        if (_healthSlider != null)
        {
            _healthSlider.value = currentHealth / maxHealth;
        }
    }

    private void InitializeHealthSlider()
    {
        _healthSlider = GetComponentInChildren<Canvas>()?.GetComponentInChildren<Slider>();
        if (_healthSlider == null)
        {
            Debug.LogError("HealthSlider is not assigned.");
        }
    }

    private void OrientHealthBarTowardsPlayer()
    {
        if (_playerTransform != null)
        {
            _healthSlider.transform.LookAt(_playerTransform);
            _healthSlider.transform.forward = -_healthSlider.transform.forward;
        }
    }

}
