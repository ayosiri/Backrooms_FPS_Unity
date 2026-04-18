using UnityEngine;

// Simulates flickering emission lighting across multiple renderer tiles
public class RoofLightFlicker : MonoBehaviour
{
    Renderer[] tiles;

    public float minIntensity = 0.6f;
    public float maxIntensity = 1.2f;

    public float minFlickerTime = 0.05f;
    public float maxFlickerTime = 0.2f;

    float timer;

    void Start()
    {
        // Cache all child renderers that will be affected by flickering
        tiles = GetComponentsInChildren<Renderer>();

        // Initialize timer with a random interval
        timer = Random.Range(minFlickerTime, maxFlickerTime);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            // Occasionally drop to very low intensity to simulate a flicker "dip"
            float intensity = Random.value < 0.1f ? 0.05f : Random.Range(minIntensity, maxIntensity);

            // Apply emission intensity to all tiles
            foreach (Renderer r in tiles)
            {
                Material mat = r.material;
                mat.SetColor("_EmissionColor", Color.white * intensity);
            }

            // Reset timer for next flicker interval
            timer = Random.Range(minFlickerTime, maxFlickerTime);
        }
    }
}
