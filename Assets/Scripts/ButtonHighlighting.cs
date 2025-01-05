using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonHighlighting : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text buttonText; // Level text
    public int id;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Bold text when button is highlighted - only if level is unlocked
        if (buttonText != null && SettingsManager.progress >= this.id)
        {
            buttonText.fontStyle = FontStyles.Bold;
        }
    }

    // Unbold text when button no longer highlighted
    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.fontStyle = FontStyles.Normal;
        }
    }
}
