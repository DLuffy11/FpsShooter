using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public WeaponSO WeaponStats => _weaponStats;
    public Transform ShootingPos => _shootingPos;

    public int Bullets => _bullets;
    public int Clips => _clips;

    [ReadOnly, SerializeField] private int _bullets;
    [ReadOnly, SerializeField] private int _clips;
    [SerializeField] private WeaponSO _weaponStats;
    [SerializeField] private Transform _shootingPos;

    private void Awake()
    {
        _bullets = _weaponStats.MaxBullets;
        _clips = _weaponStats.MaxClips;
    }
}
