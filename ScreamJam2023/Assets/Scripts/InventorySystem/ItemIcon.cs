using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
    // fields
    [SerializeField] private Text textCounter;

    // components
    private Image image;


    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
        image.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        image.preserveAspect = true;

        textCounter.text = "";
    }

    public void SetItemID(string itemID, int count)
    {
        var item = ItemManager.GetItem(itemID);
        if (item == null)
        {
            image.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            textCounter.text = "";
        }
        else
        {
            image.color = Color.white;
            image.sprite = item.Icon;

            textCounter.text = string.Format("x{0}", count);
        }
    }


}
