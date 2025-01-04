using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonHighlighting : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text buttonText; // Reference to the TextMeshPro component
    public int id;

    // Called when the mouse pointer enters the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null && SettingsManager.progress >= this.id)
        {
            buttonText.fontStyle = FontStyles.Bold;
        }
    }

    // Called when the mouse pointer exits the button
    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.fontStyle = FontStyles.Normal;
        }
    }
}
