using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ItemObject : MonoBehaviour
{
    // consts
    private const float ITEM_COLLECTION_ANIMATION_SPEED = 20.0f; 
    private const float ITEM_ROTATION_SPEED = -130.0f;
    private const float ITEM_OSCIL_FREQ = 3.0f;
    private const float ITEM_OSCIL_AMP = 0.3f;

    // fields
    [SerializeField] private int count = 1;
    [SerializeField] private string itemID;
    [SerializeField] private Transform itemContainerTransform;
    
    
    // data
    private bool collected = false;
    private InventoryManager inventoryManager = null;
    private float animationTime = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // animate item if collected
        if (collected)
        {
            var targetPosition = Vector3.up * 0.5f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * ITEM_COLLECTION_ANIMATION_SPEED);
            //transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * ITEM_COLLECTION_ANIMATION_SPEED);

            // destroy when too close
            const float precision = 0.2f; 
            if ((transform.localPosition - targetPosition).magnitude < precision)
            {
                // add item to inventory and destroy
                inventoryManager.CollectItem(itemID, count);
                Destroy(gameObject);
            }
        }
        // animate idle
        else
        {
            itemContainerTransform.Rotate(transform.up, ITEM_ROTATION_SPEED * Time.deltaTime);
            itemContainerTransform.localPosition = Vector3.up * (Mathf.Sin(animationTime * ITEM_OSCIL_FREQ) * ITEM_OSCIL_AMP);
            animationTime += Time.deltaTime;
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
