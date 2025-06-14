using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickeringLight : MonoBehaviour
{
    public AudioClip alarmClip;  // Reference to the AudioClip of the alarm sound
    public float intensity1 = 0.5f; // First intensity value
    public float intensity2 = 2.0f; // Second intensity value
    public float flickerSpeed = 0.1f; // Speed of the flickering effect

    private Light lightSource;
    private float timeSinceLastFlicker = 0f;
    private float alarmLength = 0f;
    private float alarmTimer = 0f;
    private bool useFirstIntensity = true;

    void Start()
    {
        lightSource = GetComponent<Light>();

        if (alarmClip == null)
        {
            Debug.LogError("No AudioClip assigned to FlickeringLight script.");
        }
        else
        {
            alarmLength = alarmClip.length;
        }
    }

    void Update()
    {
        if (alarmClip != null)
        {
            alarmTimer += Time.deltaTime;

            if (alarmTimer >= alarmLength)
            {
                alarmTimer = 0f;
            }

            timeSinceLastFlicker += Time.deltaTime;

            if (timeSinceLastFlicker >= flickerSpeed)
            {
                Flicker();
                timeSinceLastFlicker = 0f;
            }
        }
    }

    void Flicker()
    {
        lightSource.intensity = useFirstIntensity ? intensity1 : intensity2;
        useFirstIntensity = !useFirstIntensity; // Toggle between the two intensity values
    }
}
