using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages on-screen interaction prompts for the player
public class PlayerUI : MonoBehaviour
{
    public GameObject interactPrompt;
    public GameObject escapePrompt;

    [SerializeField]
    private RawImage promptImage;

    void Start()
    {
        // Intentionally left empty (can be used for future initialization if needed)
    }

    // Toggles visibility of the prompt image
    public void UpdateImage(bool showImage)
    {
        promptImage.enabled = showImage;
    }

    // Displays the standard interaction prompt
    public void ShowInteractPrompt()
    {
        interactPrompt.SetActive(true);
        escapePrompt.SetActive(false);
    }

    // Displays the escape-specific prompt
    public void ShowEscapePrompt()
    {
        interactPrompt.SetActive(false);
        escapePrompt.SetActive(true);
    }

    // Hides all interaction prompts
    public void HidePrompts()
    {
        interactPrompt.SetActive(false);
        escapePrompt.SetActive(false);
    }
}
