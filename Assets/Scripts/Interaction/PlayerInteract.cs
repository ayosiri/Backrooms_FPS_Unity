using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles player interaction with objects using raycasting and input
public class PlayerInteract : MonoBehaviour
{
    private Camera cam;

    [SerializeField]
    private float distance = 3f;

    [SerializeField]
    private LayerMask mask;

    private PlayerUI playerUI;

    private bool interactionEnabled = true;

    void Start()
    {
        // Cache camera and UI references
        cam = GetComponentInChildren<Camera>();
        playerUI = GetComponent<PlayerUI>();
    }

    // Disables all interaction (used during end game states)
    public void DisableInteraction()
    {
        interactionEnabled = false;
        playerUI.HidePrompts();
    }

    void Update()
    {
        if (!interactionEnabled)
            return;

        // Reset UI prompts each frame
        playerUI.HidePrompts();

        // Create a forward ray from the player's camera
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);

        RaycastHit hitInfo;

        // Check for interactable objects within range and layer mask
        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                // Show appropriate prompt based on object type
                if (interactable is ExitDoor)
                {
                    playerUI.ShowEscapePrompt();
                }
                else
                {
                    playerUI.ShowInteractPrompt();
                }

                // Trigger interaction on input
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.BaseInteract();
                }
            }
        }
    }
}
