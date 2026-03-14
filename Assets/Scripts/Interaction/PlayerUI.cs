using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public GameObject interactPrompt;
    public GameObject escapePrompt;
    [SerializeField]
    private RawImage promptImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void UpdateImage(bool showImage)
    {
       promptImage.enabled = showImage;
    }

    public void ShowInteractPrompt()
    {
        interactPrompt.SetActive(true);
        escapePrompt.SetActive(false);
    }

    public void ShowEscapePrompt()
    {
        interactPrompt.SetActive(false);
        escapePrompt.SetActive(true);
    }

    public void HidePrompts()
    {
        interactPrompt.SetActive(false);
        escapePrompt.SetActive(false);
    }
}
