using UnityEngine;

// Handles player interaction with the exit door and triggers the win sequence
public class ExitDoor : Interactable
{
    public GameEndManager gameEndManager;

    protected override void Interact()
    {
        Debug.Log("PLAYER ESCAPED");

        // Notify game manager that the player has successfully escaped
        if (gameEndManager != null)
        {
            gameEndManager.PlayerWon();
        }

        // Disable player movement to prevent further input after winning
        PlayerMovement movement = FindFirstObjectByType<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        // Disable camera control to lock the final view
        CameraController camController = FindFirstObjectByType<CameraController>();
        if (camController != null)
            camController.enabled = false;

        // Stop any active background audio
        AudioSource bgAudio = FindFirstObjectByType<AudioSource>();
        if (bgAudio != null)
            bgAudio.Stop();

        // Restore cursor for UI interaction after gameplay ends
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pause the game
        Time.timeScale = 0f;
    }
}
