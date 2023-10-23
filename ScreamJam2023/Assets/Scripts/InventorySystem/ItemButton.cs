using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;
    private Color baseColor;
    private bool hovering = false;


    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        baseColor = image.color;
        image.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        var targetColor = hovering ? baseColor : new Color(0.0f, 0.0f, 0.0f, 0.0f);
        image.color = Color.Lerp(image.color, targetColor, Time.unscaledDeltaTime * 15.0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }
}
