using System;
using System.Collections;
using UnityEngine;

// Handles weapon firing, including projectile spawning, aiming, audio, and visual effects
public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;

    public AudioClip gunShot;
    private AudioSource audioSource;

    public ParticleSystem muzzleFlash;

    void Start()
    {
        // Cache audio source for firing sound
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Fire weapon on left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            FireWeapon();
        }
    }

    // Handles full firing sequence: audio, VFX, aiming, and projectile launch
    private void FireWeapon()
    {
        audioSource.PlayOneShot(gunShot);

        // Play muzzle flash effect if assigned
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // Raycast from center of screen to determine target point
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            // Fallback target if nothing is hit
            targetPoint = ray.GetPoint(100);
        }

        // Spawn projectile at weapon muzzle
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        // Calculate direction from muzzle to target point
        Vector3 direction = (targetPoint - bulletSpawn.position).normalized;

        // Apply force to projectile
        bullet.GetComponent<Rigidbody>().AddForce(direction * bulletVelocity, ForceMode.Impulse);

        // Schedule projectile destruction
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
    }

    // Destroys projectile after a set lifetime
    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
