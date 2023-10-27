using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EquipmentController : MonoBehaviour
{
    // conts
    public const int PRIMARY_SLOT = 0;
    public const int SECONDARY_SLOT = 1;

    // fields
    [SerializeField] private GunHolder gunHolder;
    [SerializeField] private InventoryMenu inventoryMenu;


    // components
    private InventoryManager inventoryManager;

    //data
    private readonly string[] equipedWeaponsID = { "", "" };
    private int currentWeapon = PRIMARY_SLOT;

    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(Input.mouseScrollDelta.y) < 0.01f) return;

        int newWeapon = currentWeapon - (int) Mathf.Sign(Input.mouseScrollDelta.y);
        newWeapon = Mathf.Clamp(newWeapon, 0, equipedWeaponsID.Length - 1);
        Debug.Log(newWeapon);

        if (!gunHolder.IsChangingGun && equipedWeaponsID[newWeapon] != InventoryManager.ITEM_NULL_ID && newWeapon != currentWeapon )
        {
            currentWeapon = newWeapon;
            gunHolder.ChangeWeaponTo(equipedWeaponsID[newWeapon]);
        }

    }

    // get current weapon (returns null if nothing is equiped)
    public Weapon GetCurrentWeapon()
    {
        return GetWeaponAt(currentWeapon);
    }

    // get weapon at slot
    public Weapon GetWeaponAt(int slot)
    {
        var currentWeaponID = equipedWeaponsID[slot];
        Weapon weapon = (currentWeaponID == "") ? null : (Weapon)ItemManager.GetItem(currentWeaponID);
        return weapon;
    }

    // equip weapon
    public void Equip(int slot, string weaponID)
    {
        if (slot == currentWeapon && equipedWeaponsID[currentWeapon] != weaponID)
        {
            gunHolder.ChangeWeaponTo(weaponID);
        }

        equipedWeaponsID[slot] = weaponID;
        inventoryMenu.UpdateEquipments();
    }
}
