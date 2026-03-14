using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    [SerializeField]
    private GameObject door;
    private bool doorOpen;
    [SerializeField]
    private float closeDelay = 3f;
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip doorCloseSound;

    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // interaction function
    protected override void Interact()
    {
        doorOpen = !doorOpen;
        door.GetComponent<Animator>().SetBool("IsOpen", door);
        Debug.Log("Interacted with " + gameObject.name);

        if (doorOpen)
        {
            audioSource.PlayOneShot(doorOpenSound);
            StartCoroutine(CloseDoorAfterDelay());
        }

        IEnumerator CloseDoorAfterDelay()
        {
            yield return new WaitForSeconds(closeDelay);

            doorOpen =false;
            door.GetComponent<Animator>().SetBool("IsOpen", false);
            audioSource.PlayOneShot(doorCloseSound);
        }

    }
}
