using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class BulletCounter : MonoBehaviour
{
    private const float ANIMATION_SPEED = 8.0f;

    [SerializeField] private AnimationCurve updateAnimationCurve;
    [SerializeField] private RectTransform rectTransform;

    private Text text;
    private float animationTime = 10000.0f;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        animationTime += Time.deltaTime * ANIMATION_SPEED;
        if (animationTime > updateAnimationCurve[updateAnimationCurve.length - 1].time)
        {
            rectTransform.localScale = Vector3.one;
        }
        else
        {
            rectTransform.localScale = Vector3.one * updateAnimationCurve.Evaluate(animationTime);
        }
    }

    // void update bullet count
    public void UpdateValue(int newValue)
    {
        text.text = string.Format("x{0}", newValue);
    }

    // void update bullet animated
    public void UpdateValueAnimated(int newValue)
    {
        UpdateValue(newValue);
        animationTime = 0.0f;
    }
}
