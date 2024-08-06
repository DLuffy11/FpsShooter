using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> _inventoryItems = new List<InventoryItem>(); // UI elements representing the inventory slots
    [SerializeField] private Inventory _inventory; 

    private void Start()
    {
        UpdateUI();
    }

    private void Awake()
    {
        _inventory = FindAnyObjectByType<Inventory>();
    }

    public void UpdateUI()
    {
        List<WeaponSO> weapons = _inventory.Weapons;

        for (int i = 0; i < _inventoryItems.Count; i++)
        {
            if (i < weapons.Count)
            {
                _inventoryItems[i].SetImage(weapons[i].ImageUI);
            }
        }

        //If i don't do that selected weapon is wrong
        SwapItems(1, 2);
    }


    private void SwapItems(int index1, int index2)
    {
        if (index1 < 0 || index1 >= _inventoryItems.Count || index2 < 0 || index2 >= _inventoryItems.Count)
        {
            Debug.LogError("Invalid index for swapping inventory items.");
            return;
        }

        InventoryItem temp = _inventoryItems[index1];
        _inventoryItems[index1] = _inventoryItems[index2];
        _inventoryItems[index2] = temp;
    }


    public void SetSelectedWeapon(WeaponSO selectedWeapon)
    {
        List<WeaponSO> weapons = _inventory.Weapons;

        for (int i = 0; i < _inventoryItems.Count; i++)
        {
            if (i < weapons.Count)
            {
                if (weapons[i] == selectedWeapon)
                {
                    _inventoryItems[i].SetSelected(true);
                }
                else
                {
                    _inventoryItems[i].SetSelected(false);
                }
            }
        }
    }

}
