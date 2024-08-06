public class WeaponRuntimeData
{
    public WeaponSO WeaponSO { get; private set; }
    public int CurrentBullets { get; set; }
    public int CurrentClips { get; set; }

    public WeaponRuntimeData(WeaponSO weaponSO)
    {
        WeaponSO = weaponSO;
        CurrentBullets = weaponSO.MaxBullets;
        CurrentClips = weaponSO.MaxClips;
    }
}
