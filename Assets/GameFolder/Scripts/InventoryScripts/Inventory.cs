using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<WeaponSO> Weapons => _weapons;

    private Dictionary<string, WeaponState> _weaponStates = new Dictionary<string, WeaponState>();
    [SerializeField] private List<WeaponSO> _weapons = new List<WeaponSO>(); // only 3 weapons


    public void SaveWeaponState(string weaponName, int currentBullets, int currentClips)
    {
        if (_weaponStates.ContainsKey(weaponName))
        {
            _weaponStates[weaponName] = new WeaponState(currentBullets, currentClips);
        }
        else
        {
            _weaponStates.Add(weaponName, new WeaponState(currentBullets, currentClips));
        }
    }

    public WeaponState LoadWeaponState(string weaponName)
    {
        if (_weaponStates.TryGetValue(weaponName, out WeaponState state))
        {
            return state;
        }
        return null;
    }

    public void AddWeapon(WeaponSO weapon)
    {
        if (!_weapons.Contains(weapon))
        {
            _weapons.Add(weapon);
        }
    }
}


public class WeaponState
{
    public int CurrentBullets { get; private set; }
    public int CurrentClips { get; private set; }

    public WeaponState(int currentBullets, int currentClips)
    {
        CurrentBullets = currentBullets;
        CurrentClips = currentClips;
    }
}
