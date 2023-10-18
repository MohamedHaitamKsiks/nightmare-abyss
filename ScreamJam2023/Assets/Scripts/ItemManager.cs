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
}

// resource item can only be used to craft / make other objects
[Serializable]
public class Resource: Item
{

}

// consumable item (food, medicine)
[Serializable]
public class Consumable: Item
{
    public float Health;
    public float Hunger;
}

// weapons

[Serializable]
public class Weapon: Item
{
    public float Damage;
    public float Range;
}

// light source
[Serializable]
public class LightSource: Item
{
    public float Range;
    public float Life;
}

public class ItemManager : MonoBehaviour
{
    // register items
    [SerializeField] private Weapon[] weapons;
    [SerializeField] private Resource[] resources;
    [SerializeField] private Consumable[] consumables;
    [SerializeField] private LightSource[] lightSources;

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

        foreach(var resource in resources)
        {
            items[resource.ID] = resource;
        }

        foreach (var consumable in consumables)
        {
            items[consumable.ID] = consumable;
        }

        foreach (var lightSource in lightSources)
        {
            items[lightSource.ID] = lightSource;
        }
    
    }

    // get item
    public static Item GetItem(string ID)
    {
        if (Instance == null) return null;
        return Instance.items[ID];
    }
}
