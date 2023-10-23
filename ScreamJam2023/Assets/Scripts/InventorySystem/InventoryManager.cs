using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // collected items with it's quantity
    public readonly Dictionary<string, int> Items = new();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // add item
    public void CollectItem(string itemID, int count = 1)
    {
        if (Items.ContainsKey(itemID) && ItemManager.GetItem(itemID) is not Weapon)
        {
            Items[itemID] += count;
        }
        else
        {
            Items[itemID] = count;
        }
    }
}
