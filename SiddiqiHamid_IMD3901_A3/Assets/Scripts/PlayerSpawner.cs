using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : NetworkBehaviour
{
    [Header("Player Prefabs")]
    public GameObject desktopPlayerPrefab;
    public GameObject vrPlayerPrefab;

    [Header("UI Reference")]
    public GameObject selectionCanvas; // Drag your Startup Screen Canvas here

    void Start()
    {
        // Ensure the selection screen is visible when the game starts
        if (selectionCanvas != null) selectionCanvas.SetActive(true);
    }

    //void Update()
    //{
    //    // Only allow spawning if the local player hasn't already spawned a character
    //    if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
    //    {
    //        // If the UI is already hidden, we've already spawned
    //        if (!selectionCanvas.activeSelf) return;

    //        if (Input.GetKeyDown(KeyCode.Alpha1))
    //        {
    //            SpawnPlayer(0); // Desktop
    //        }
    //        else if (Input.GetKeyDown(KeyCode.Alpha2))
    //        {
    //            SpawnPlayer(1); // VR
    //        }
    //    }
    //}

    void Update()
    {
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
        {
            if (selectionCanvas == null || !selectionCanvas.activeSelf) return;

            // NEW INPUT SYSTEM CODE
            if (UnityEngine.InputSystem.Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                SpawnPlayer(0); // Desktop
            }
            else if (UnityEngine.InputSystem.Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                SpawnPlayer(1); // VR
            }
        }
    }

    void SpawnPlayer(int type)
    {
        // Hide the selection screen immediately for the local user
        selectionCanvas.SetActive(false);

        // Request the server to spawn the specific prefab
        RequestSpawnServerRpc(type, NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestSpawnServerRpc(int type, ulong clientId)
    {
        GameObject prefabToSpawn = (type == 0) ? desktopPlayerPrefab : vrPlayerPrefab;

        // Create the player instance on the server
        GameObject playerInstance = Instantiate(prefabToSpawn);

        // Use SpawnAsPlayerObject to give this client ownership of their chosen character
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }
}