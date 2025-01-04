using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public Button level1;
    public Button level2;
    public Texture lockedIcon;
    public Texture unlockedIcon1;
    public Texture unlockedIcon2;
    public RawImage level1Image;
    public RawImage level2Image;

    void Start() {
        level1.interactable = false;
        level1Image.texture = lockedIcon;
        level2.interactable = false;
        level2Image.texture = lockedIcon;
    }

    void Update()
    {
        if (SettingsManager.progress >= 1) {
            level1.interactable = true;
            level1Image.texture = unlockedIcon1;
        }
        if (SettingsManager.progress == 2) {
            level2.interactable = true;
            level2Image.texture = unlockedIcon2;
        }
    }
}
