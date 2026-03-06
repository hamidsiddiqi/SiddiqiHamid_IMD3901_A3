using Unity.Netcode;
using UnityEngine;

public class TaskButton : MonoBehaviour
{
    public Color pressedColor = Color.green; // Setting any pressed button already to green
    private MeshRenderer meshRenderer;
    private bool isPressed = false;

    // Button Audio
    [Header("Audio Settings")]
    public AudioClip clickSound;
    private AudioSource audioSource;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f;
    }


    public void Interact()
    {
        // Checking to see if Player 2 joins
        if (GameManager.Instance != null && !GameManager.Instance.gameActive.Value)
        {
            return; // Stop the function here
        }

        // Playing button pressed audio when player presses red button
        if (!isPressed)
        {
            isPressed = true;
            meshRenderer.material.color = pressedColor;
            if (clickSound != null) audioSource.PlayOneShot(clickSound);

            // Tell the GameManager a task with this tag is done
            GameManager.Instance.TaskCompletedServerRpc(gameObject.tag);
        }
    }
}


