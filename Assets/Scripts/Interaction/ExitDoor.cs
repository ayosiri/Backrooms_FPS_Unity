using UnityEngine;

public class ExitDoor : Interactable
{
    public GameEndManager gameEndManager;

    protected override void Interact()
    {
        Debug.Log("PLAYER ESCAPED");

        if (gameEndManager != null)
        {
            gameEndManager.PlayerWon();
        }

        PlayerMovement movement = FindFirstObjectByType<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        CameraController camController = FindFirstObjectByType<CameraController>();
        if (camController != null)
            camController.enabled = false;

        AudioSource bgAudio = FindFirstObjectByType<AudioSource>();
        if (bgAudio != null)
            bgAudio.Stop();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }
}