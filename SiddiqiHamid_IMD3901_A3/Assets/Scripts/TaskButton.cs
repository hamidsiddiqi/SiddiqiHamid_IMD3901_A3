using Unity.Netcode;
using UnityEngine;

public class TaskButton : MonoBehaviour
{
    public Color pressedColor = Color.green;
    private MeshRenderer meshRenderer;
    private bool isPressed = false;

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

        if (GameManager.Instance != null && !GameManager.Instance.gameActive.Value)
        {
            Debug.Log("Game hasn't started yet! Waiting for P2...");
            return; // Stop the function here
        }


        if (!isPressed)
        {
            isPressed = true;
            meshRenderer.material.color = pressedColor;
            if (clickSound != null) audioSource.PlayOneShot(clickSound);

            // Tell the GameManager a task with this tag is done
            //GameManager.Instance.TaskCompletedServerRpc(gameObject.tag, NetworkManager.Singleton.LocalClientId);\
            GameManager.Instance.TaskCompletedServerRpc(gameObject.tag);
        }
    }
}


