using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupUI_Manager : MonoBehaviour
{
    public TextMeshProUGUI popupText;
    public RectTransform popupRect;
    public static Action<string, Vector2> OnMouseHover;
    public static Action OnMouseNotHover;

    private void OnEnable()
    {
        //subscribes the methods to the actions
        OnMouseHover += ShowPopup;
        OnMouseNotHover += HidePopup;
    }

    private void OnDisable()
    {
        //unsubscribes the methods from the actions
        OnMouseHover -= ShowPopup;
        OnMouseNotHover -= HidePopup;
    }

    // Start is called before the first frame update
    void Start()
    {
        //makes sure that the popups aren't showing before the action
        HidePopup();
    }

    /// <summary>
    /// Takes all the values needed for positioning the popup
    /// it gets positioned underneath the cursor
    /// and putting the text from the PopupUI on the popup object
    /// </summary>
    /// <param name="description"></param>
    /// <param name="mousePos"></param>
    private void ShowPopup(string description, Vector2 mousePos)
    {
        popupText.text = description;
        popupRect.sizeDelta = new Vector2(popupText.preferredWidth > 500 ? 500 : popupText.preferredWidth, popupText.preferredHeight);

        popupRect.gameObject.SetActive(true);
        popupRect.transform.position = new Vector2(mousePos.x + popupRect.sizeDelta.x * 0.5f, mousePos.y + popupRect.sizeDelta.y * -0.5f);
    }

    /// <summary>
    /// resets the text
    /// disables the object
    /// </summary>
    private void HidePopup()
    {
        popupText.text = default;
        popupRect.gameObject.SetActive(false);
    }
}
