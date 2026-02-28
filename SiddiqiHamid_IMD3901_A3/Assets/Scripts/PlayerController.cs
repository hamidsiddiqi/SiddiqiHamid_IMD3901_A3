using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 2f;

    public CharacterController controller;
    public Transform cameraTransform;

    public Camera playerCamera;

    float xRotation = 0f;


    public override void OnNetworkSpawn()
    {

        if (!IsOwner)
        {
            playerCamera.enabled = false;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }


    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        // --- LOCK CONTROLS UNTIL P2 JOINS ---
        // If GameManager doesn't exist yet or game isn't active, stop here
        if (GameManager.Instance == null || !GameManager.Instance.gameActive.Value)
        {
            // Keep the cursor locked so they don't click out of the window while waiting
            Cursor.lockState = CursorLockMode.Locked;
            return;
        }

        // Everything below only runs once gameActive is true
        Vector2 moveInput = Keyboard.current != null ? new Vector2
            (
                (Keyboard.current.aKey.isPressed ? -1 : 0) + (Keyboard.current.dKey.isPressed ? 1 : 0),
                (Keyboard.current.sKey.isPressed ? -1 : 0) + (Keyboard.current.wKey.isPressed ? 1 : 0)
            ) : Vector2.zero;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.deltaTime);

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 3f))
            {
                if (hit.collider.TryGetComponent(out TaskButton button))
                {
                    button.Interact();
                }
            }
        }
    }


}
