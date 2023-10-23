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
    public float Damage;
    public float Range;
    public float Recoil;
    public float Cooldown;
    public GameObject WeaponPrefab;
}

// bullet
[Serializable]
public class Bullet: Item
{
    public string WeaponID;
}

public class ItemManager : MonoBehaviour
{
    // register items
    [SerializeField] private Weapon[] weapons;
    [SerializeField] private Consumable[] consumables;

    // registered items
    private readonly Dictionary<string, Item> items = new();

    public static ItemManager Instance {get; private set;} = null;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

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
