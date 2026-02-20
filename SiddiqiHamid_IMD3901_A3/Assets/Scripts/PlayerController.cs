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


    /*
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Debug.Log("Scene has started!");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    } */

    // Update is called once per frame
    void Update()
    {

        if (!IsOwner)
        {
            return;
        }

        //Debug.Log("Scene is updating!");

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



        //if (IsOwner && Mouse.current.leftButton.wasPressedThisFrame)
        //{
        //    // 1. Define the ray starting from the center of the player's camera
        //    Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        //    RaycastHit hit;

        //    // 2. Check if the ray hits an object within 3 meters
        //    if (Physics.Raycast(ray, out hit, 3f))
        //    {
        //        // 3. Try to find the TaskButton script on the object we hit
        //        if (hit.collider.TryGetComponent(out TaskButton button))
        //        {
        //            // 4. Trigger the interaction (Visual/Aural feedback)
        //            button.InteractServerRpc();
        //        }
        //    }
        //}


    }
}
