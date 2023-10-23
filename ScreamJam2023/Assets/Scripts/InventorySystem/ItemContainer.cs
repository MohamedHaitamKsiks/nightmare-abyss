using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemContainer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    // fields
    [SerializeField] private ItemIcon icon; 
    [SerializeField] private Text descriptionText;
    
    [SerializeField] private HoverPannel buttonMenu;

    [SerializeField] private Button equipPrimaryButton;
    [SerializeField] private Button equipSecondaryButton;
    [SerializeField] private Button useButton;


    // data
    // needed to be provided to manage equipement
    public EquipmentController PlayerEquipmentController = null;
    public string ItemID {get; private set;} = "";

    // Start is called before the first frame update
    void Start()
    {
        equipPrimaryButton.onClick.AddListener(() => 
        {
            PlayerEquipmentController.Equip(EquipmentController.PRIMARY_SLOT, ItemID);
        });

        equipSecondaryButton.onClick.AddListener(() =>
        {
            PlayerEquipmentController.Equip(EquipmentController.SECONDARY_SLOT, ItemID);
        });

        useButton.onClick.AddListener(() =>
        {

        });
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    // set itemID
    public void SetItemID(string itemID, int count)
    {
        ItemID = itemID;

        Item item = ItemManager.GetItem(itemID);
        icon.SetItemID(itemID, count);
        descriptionText.text = String.Format(@"<b>{0}</b>
<size=4><i>{1}</i></size>", item.Name, item.Description);
    }

    // on mouse hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ItemID == "") return;
        
        // config menu
        var item = ItemManager.GetItem(ItemID);
        equipPrimaryButton.gameObject.SetActive(item is Weapon);
        equipSecondaryButton.gameObject.SetActive(item is Weapon);
        useButton.gameObject.SetActive(item is not Weapon);

        buttonMenu.Show();

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ItemID == "") return;
        buttonMenu.Hide();
    }

}
