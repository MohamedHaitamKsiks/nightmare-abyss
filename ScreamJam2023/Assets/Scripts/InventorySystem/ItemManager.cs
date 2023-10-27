using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Item
{
    public string ID;
    public string Name;
    public string Description;
    public Sprite Icon;
}

// consumable item (food, medicine)
[Serializable]
public class Consumable: Item
{
    public float Health;
}

// weapons

[Serializable]
public class Weapon: Item
{
    // 
    public bool UseBullets = true;
    // max damage by weapon
    public float Damage = 5.0f;
    public float Range = 100.0f;
    public int BulletCount = 1;
    // give the range of spread 
    public float BulletSpread = 0.0f;
    public float Recoil = 1.0f;
    // how much does the gun pushes the player (can be usefull with rocket jump)
    public float MovementRecoil = 0.0f;
    public float Delai = 0.1f;
    public float AnimationSpeed = 1.0f;
    public float Penetration = 2.0f;
    public Vector3 RotationEuler = Vector3.forward;
    public GameObject WeaponPrefab;
    public AnimationCurve ShootCurve;   
    public Sprite Crosshair;

}

// bullet
[Serializable]
public class Bullet: Item
{
    public string WeaponID;
}

public class ItemManager : Singleton<ItemManager>
{
    // register items
    [SerializeField] private Weapon[] weapons;
    [SerializeField] private Consumable[] consumables;

    // registered items
    private readonly Dictionary<string, Item> items = new();

    // Start is called before the first frame update
    public override void OnInit()
    {
        // register all type of items
        foreach(var weapon in weapons)
        {
            items[weapon.ID] =  weapon;
        }

        foreach (var consumable in consumables)
        {
            items[consumable.ID] = consumable;
        }
    
    }

    // get item
    public static Item GetItem(string ID)
    {
        if (Instance == null) return null;
        return Instance.items[ID];
    }
}
