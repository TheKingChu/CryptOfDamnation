using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(5, 30)]
    public string description;
    private float timeToWait = 0.5f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //makes sure that no popups are available and if there are stop it
        StopAllCoroutines();
        //timer + show the popup
        StartCoroutine(StartTimer());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //stops the popup displaying
        StopAllCoroutines();
        PopupUI_Manager.OnMouseNotHover();
    }

    private void ShowMessage()
    {
        PopupUI_Manager.OnMouseHover(description, Input.mousePosition);
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(timeToWait);

        ShowMessage();
    }
}
