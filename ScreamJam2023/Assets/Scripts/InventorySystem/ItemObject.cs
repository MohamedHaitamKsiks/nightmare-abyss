using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ItemObject : MonoBehaviour
{
    // consts
    private const float ITEM_COLLECTION_ANIMATION_SPEED = 10.0f; 

    // fields
    [SerializeField] private int count = 1;
    [SerializeField] private string itemID;
    
    // components
    private BoxCollider boxCollider;
    
    // data
    private bool collected = false;
    private InventoryManager inventoryManager = null;


    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        // animate item if collected
        if (collected)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * ITEM_COLLECTION_ANIMATION_SPEED);
            
            // destroy when too close
            const float precision = 0.1f; 
            if (transform.localPosition.magnitude < precision)
            {
                // add item to inventory and destroy
                inventoryManager.CollectItem(itemID, count);
                Destroy(gameObject);
            }
        }
    }

    // add item to inventory manager
    public void Collect(InventoryManager manager)
    {
        if (collected) return;

        // get inventory manager
        inventoryManager = manager;

        // animate item collection
        collected = true;
        transform.SetParent(manager.gameObject.transform, true);

    }
}
