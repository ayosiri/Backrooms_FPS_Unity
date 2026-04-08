using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        cam = GetComponentInChildren<Camera>();
        playerUI = GetComponent<PlayerUI>();
        // Debug.Log("Hit Check");
    }

    public void DisableInteraction()
    {
        interactionEnabled = false;
        playerUI.HidePrompts();
    }

    void Update()
    {
        if (!interactionEnabled)
            return;

        playerUI.HidePrompts();

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);

        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                if (interactable is ExitDoor)
                {
                    playerUI.ShowEscapePrompt();
                }
                else
                {
                    playerUI.ShowInteractPrompt();
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.BaseInteract();
                }
            }
        }
    }
}
