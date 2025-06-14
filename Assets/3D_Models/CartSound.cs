using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class CartSound : MonoBehaviour
{
    private Rigidbody rb;
    private AudioSource audioSource;
    public float velocityThreshold = 0.1f; // Adjust this value to determine what constitutes movement

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Check if the cart's velocity exceeds the threshold
        if (rb.velocity.magnitude > velocityThreshold)
        {
            // If the audio is not already playing, play it
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            // If the cart stops moving, stop the sound
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
