using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
    public RawImage backgroundImage;  // Reference to the background image
    public float scrollSpeedX = 0.01f;  // Horizontal scroll speed
    public float scrollSpeedY = 0.01f;  // Vertical scroll speed

    void Update()
    {
        // Get the current UV offset of the texture
        Vector2 offset = backgroundImage.uvRect.position;

        // Update the offset based on scroll speeds
        offset.x += scrollSpeedX * Time.deltaTime;
        offset.y += scrollSpeedY * Time.deltaTime;

        // Apply the offset back to the UV rect
        backgroundImage.uvRect = new Rect(offset, backgroundImage.uvRect.size);
    }
}
