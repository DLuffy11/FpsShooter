using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon")]
public class WeaponSO : ScriptableObject
{
    public string Name => _name;
    public int Damage => _damage;
    public float FireRate => _fireRate;

    public int MaxBullets => _maxBullets;
    public int MaxClips => _maxClips;

    public AudioClip FireSound => _fireSound;
    public AudioClip ImpactSound => _impactSound;
    public GameObject WeaponPrefab => _weaponPrefab;
    public GameObject HitParticlePrefab => _hitParticlePrefab;
    public Sprite ImageUI => _imageUI;

    [SerializeField] private string _name;

    [SerializeField] private int _damage;
    [SerializeField] private float _fireRate;

    [SerializeField] private int _maxBullets;
    [SerializeField] private int _maxClips;

    [SerializeField] private AudioClip _fireSound;
    [SerializeField] private AudioClip _impactSound;

    [SerializeField] private GameObject _weaponPrefab;
    [SerializeField] private GameObject _hitParticlePrefab;

    [SerializeField] private Sprite _imageUI;

    [HideInInspector] public int CurrentBullets;
    [HideInInspector] public int CurrentClips;
}
