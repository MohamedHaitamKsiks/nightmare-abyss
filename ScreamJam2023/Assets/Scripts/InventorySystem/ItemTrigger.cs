using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTrigger : MonoBehaviour
{

    [SerializeField] private InventoryManager inventoryManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != gameObject.layer) return;

        var itemObject = other.gameObject.GetComponent<ItemObject>();
        itemObject.Collect(inventoryManager);
    }
}
