using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InventoryMenu : MonoBehaviour
{
    // consts
    private const float ANIMATION_SPEED = 20.0f;
    private const int MAX_ITEM_NUMBER = 100;

    // fields
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private EquipmentController equipmentController;
    [SerializeField] private GameObject itemContainerPrefab;
    [SerializeField] private GameObject content;
    [SerializeField] private ItemIcon primaryIcon;
    [SerializeField] private ItemIcon secondaryIcon;

    // components
    private RectTransform rectTransform;
    private Image image;
    private RectTransform contentRect;
    private HorizontalLayoutGroup contentHorizontalLayoutGroup;

    // data
    private float height;
    private bool visible = false;
    private readonly DynamicArray<ItemContainer> itemContainers = new();
    private float itemContainerWidth = 0.0f;


    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        height = rectTransform.rect.height;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 0.0f);

        // get content rect
        contentRect = content.GetComponent<RectTransform>();
        contentHorizontalLayoutGroup = content.GetComponent<HorizontalLayoutGroup>();

    
        // object pooling item containers
        for (int i = 0; i < MAX_ITEM_NUMBER; i++)
        {
            var itemContainer = Instantiate(itemContainerPrefab, content.transform).GetComponent<ItemContainer>();
            itemContainer.PlayerEquipmentController = equipmentController;
            itemContainerWidth = itemContainer.GetComponent<RectTransform>().rect.width;
            itemContainers.Add(itemContainer);
        }

        UpdateInventoryList();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            visible = !visible;
            
            if (visible)
            {
                UpdateInventoryList();
                PauseManager.Pause();
            }
            else 
            {
                PauseManager.Resume();
            }
        }

        // animate height
        var heightTarget = visible ? height : 0.0f;
        rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, new Vector2(rectTransform.sizeDelta.x, heightTarget), Time.unscaledDeltaTime * ANIMATION_SPEED);
        
        // snap to target when close
        var precision = 4.0f;
        if (Mathf.Abs(rectTransform.sizeDelta.y - height) < precision)
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, heightTarget);
        
    }

    void UpdateInventoryList()
    {
        // update containers 
        foreach (var itemContainer in itemContainers)
        {
            itemContainer.gameObject.SetActive(false);
        }

        int i = 0;
        foreach (var itemID in inventoryManager.Items.Keys)
        {
            itemContainers[i].gameObject.SetActive(true);
            itemContainers[i].SetItemID(itemID, inventoryManager.Items[itemID]);
            i++;
        }

        // calculate width of content
        var contentWidth = (itemContainerWidth + contentHorizontalLayoutGroup.spacing) * i;
        contentRect.sizeDelta = new Vector2(contentWidth, contentRect.sizeDelta.y);
    }

    public void UpdateEquipments()
    {
        // update primary image
        var primaryWeapon = equipmentController.GetWeaponAt(EquipmentController.PRIMARY_SLOT);
        if (primaryWeapon != null)
            primaryIcon.SetItemID(primaryWeapon.ID, inventoryManager.Items[primaryWeapon.ID]);

        // update secondary image   
        var secondaryWeapon = equipmentController.GetWeaponAt(EquipmentController.SECONDARY_SLOT);
        if (secondaryWeapon != null)
            secondaryIcon.SetItemID(secondaryWeapon.ID, inventoryManager.Items[secondaryWeapon.ID]);
    }
}
