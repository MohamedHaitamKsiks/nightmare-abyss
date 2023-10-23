using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverPannel : MonoBehaviour
{
    // consts
    private const float ANIMATION_SPEED = 15.0f;

    // fields
    [SerializeField] private Rect bounds;

    // components
    private RectTransform rectTransform;

    // data
    private float scale = 0.0f;
    private bool visible = false;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        var scaleTarget = visible? 1.0f: 0.0f;
        scale = Mathf.Lerp(scale, scaleTarget, Time.unscaledDeltaTime * ANIMATION_SPEED);
        
        // snap to target
        var precision = 0.01f;
        if (Mathf.Abs(scale - scaleTarget) < precision)
        {
            scale = scaleTarget;
            // disable 
            if (!visible) gameObject.SetActive(false);
        }

        // set scale
        rectTransform.localScale = Vector3.one * scale;

    }

    public void Show()
    {
        gameObject.SetActive(true);
        visible = true;

        // bound position
        Vector2 mousePosition = Input.mousePosition;
        mousePosition.x = Mathf.Sign(mousePosition.x);
        mousePosition.y = Mathf.Sign(mousePosition.y);

        rectTransform.anchoredPosition = new Vector2(
            mousePosition.x * bounds.width,
            mousePosition.y * bounds.height
        );
    }

    public void Hide()
    {
        visible = false;
    }
}
