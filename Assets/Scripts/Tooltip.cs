using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    private static Tooltip _instance;
    private Text _tooltipText;
    private RectTransform _backgroundRectTransform;
    [SerializeField]
    private Camera uiCamera;

    void Awake()
    {
        _instance = this;
        _tooltipText = gameObject.GetComponentInChildren<Text>();
        _backgroundRectTransform = GameObject.Find("Background").GetComponent<RectTransform>();
        
    }

    void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, uiCamera,
            out localPoint);

        transform.localPosition = localPoint;
    }
    
    void ShowTooltip(string tooltipString)
    {
        gameObject.SetActive(true);
        _tooltipText.text = tooltipString;
        float textPaddingSize = 4f;
        Vector2 bacVector2 = new Vector2(_tooltipText.preferredWidth + textPaddingSize * 2f, _tooltipText.preferredHeight + textPaddingSize * 2f);
        _backgroundRectTransform.sizeDelta = bacVector2;



    }

    void HideTooltip()
    {
        gameObject.SetActive(false);
    }


    public static void ShowTooltip_Static(string tooltipString)
    {
        _instance.ShowTooltip(tooltipString);
    }

    public static void HideTooltip_Static()
    {
        _instance.HideTooltip();
    }
}
