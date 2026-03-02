https://assetstore.unity.com/packages/3d/environments/sci-fi/megapoly-art-vintage-control-room-190538

https://www.freepik.com/free-photo/gray-tiled-wall_4102545.htm#fromView=search&page=1&position=7&uuid=6775f2ae-2dfe-49d0-a9e3-293bb42f4df6&query=space+wall+texture

https://www.freepik.com/free-photo/grunge-style-metallic-texture-background_8210786.htm#fromView=search&page=1&position=17&uuid=10a605cc-9503-4f4a-8436-c0677723724f&query=space+station+wall+texture

https://www.freepik.com/free-photo/metallic-wall-design-element-textured-wallpaper-concept_2971618.htm#fromView=search&page=1&position=41&uuid=10a605cc-9503-4f4a-8436-c0677723724f&query=space+station+wall+texture

https://www.freepik.com/free-photo/photo-wood-texture-pattern_210126230.htm#fromView=search&page=2&position=4&uuid=10a605cc-9503-4f4a-8436-c0677723724f&query=space+station+wall+texture


https://pixabay.com/music/adventure-war-epic-background-music-480183/

https://pixabay.com/sound-effects/film-special-effects-button-press-382713/


//using UnityEngine;
//using Unity.Netcode;

//public class PlayerSpawner : NetworkBehaviour
//{
//    [Header("Player Prefabs")]
//    public GameObject desktopPlayerPrefab;
//    public GameObject vrPlayerPrefab;

//    [Header("UI Reference")]
//    public GameObject selectionCanvas; // Drag your Startup Screen Canvas here

//    void Start()
//    {
//        // Ensure the selection screen is visible when the game starts
//        if (selectionCanvas != null) selectionCanvas.SetActive(true);
//    }

//    //void Update()
//    //{
//    //    // Only allow spawning if the local player hasn't already spawned a character
//    //    if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
//    //    {
//    //        // If the UI is already hidden, we've already spawned
//    //        if (!selectionCanvas.activeSelf) return;

//    //        if (Input.GetKeyDown(KeyCode.Alpha1))
//    //        {
//    //            SpawnPlayer(0); // Desktop
//    //        }
//    //        else if (Input.GetKeyDown(KeyCode.Alpha2))
//    //        {
//    //            SpawnPlayer(1); // VR
//    //        }
//    //    }
//    //}

//    void Update()
//    {
//        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
//        {
//            if (selectionCanvas == null || !selectionCanvas.activeSelf) return;

//            // NEW INPUT SYSTEM CODE
//            if (UnityEngine.InputSystem.Keyboard.current.digit1Key.wasPressedThisFrame)
//            {
//                SpawnPlayer(0); // Desktop
//            }
//            else if (UnityEngine.InputSystem.Keyboard.current.digit2Key.wasPressedThisFrame)
//            {
//                SpawnPlayer(1); // VR
//            }
//        }
//    }

//    void SpawnPlayer(int type)
//    {
//        // Hide the selection screen immediately for the local user
//        selectionCanvas.SetActive(false);

//        // Request the server to spawn the specific prefab
//        RequestSpawnServerRpc(type, NetworkManager.Singleton.LocalClientId);
//    }

//    [ServerRpc(RequireOwnership = false)]
//    void RequestSpawnServerRpc(int type, ulong clientId)
//    {
//        GameObject prefabToSpawn = (type == 0) ? desktopPlayerPrefab : vrPlayerPrefab;

//        // Create the player instance on the server
//        GameObject playerInstance = Instantiate(prefabToSpawn);

//        // Use SpawnAsPlayerObject to give this client ownership of their chosen character
//        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
//    }
//}


//using UnityEngine;
//using Unity.Netcode;
//using UnityEngine.InputSystem;

//public class PlayerSpawner : NetworkBehaviour
//{
//    [Header("Player Prefabs")]
//    public GameObject desktopPlayerPrefab;
//    public GameObject vrPlayerPrefab;

//    [Header("Start Menu Graphic")]
//    public GameObject selectionCanvas;

//    private bool gameStarted = false;

//    void Start()
//    {
//        // Setup the initial state like your past assignment
//        if (selectionCanvas != null)
//        {
//            selectionCanvas.SetActive(true);
//        }

//        // Unlock cursor for the menu
//        Cursor.lockState = CursorLockMode.None;
//        Cursor.visible = true;
//    }

//    void Update()
//    {
//        // Only allow selection if the game hasn't started for the local player
//        if (gameStarted) return;

//        // Use the New Input System from your example
//        if (Keyboard.current.digit1Key.wasPressedThisFrame)
//        {
//            EnableDesktop();
//        }

//        if (Keyboard.current.digit2Key.wasPressedThisFrame)
//        {
//            EnableHMD();
//        }
//    }

//    void EnableDesktop()
//    {
//        StartGame();
//        // Request a Desktop spawn from the server
//        RequestSpawnServerRpc(0, NetworkManager.Singleton.LocalClientId);

//        // Lock cursor for Desktop play
//        Cursor.lockState = CursorLockMode.Locked;
//        Cursor.visible = false;
//    }

//    void EnableHMD()
//    {
//        StartGame();
//        // Request a VR spawn from the server
//        RequestSpawnServerRpc(1, NetworkManager.Singleton.LocalClientId);

//        // VR usually doesn't need a locked mouse
//        Cursor.lockState = CursorLockMode.None;
//        Cursor.visible = true;
//    }

//    void StartGame()
//    {
//        if (!gameStarted && selectionCanvas != null)
//        {
//            selectionCanvas.SetActive(false);
//            gameStarted = true;
//        }
//    }


//    [ServerRpc(RequireOwnership = false)]
//    void RequestSpawnServerRpc(int type, ulong clientId)
//    {
//        // Server chooses which prefab to instantiate
//        GameObject prefabToSpawn = (type == 0) ? desktopPlayerPrefab : vrPlayerPrefab;
//        GameObject playerInstance = Instantiate(prefabToSpawn);

//        // This makes the object appear for EVERYONE and gives control to the client
//        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
//    }
//}

//using UnityEngine;
//using Unity.Netcode;
//using UnityEngine.InputSystem;

//public class PlayerSpawner : NetworkBehaviour
//{
//    [Header("Player Prefabs")]
//    public GameObject desktopPlayerPrefab;
//    public GameObject vrPlayerPrefab;

//    [Header("Start Menu Graphic")]
//    public GameObject selectionCanvas;

//    private bool hasSpawned = false;

//    void Start()
//    {
//        // Ensure graphic is visible at the start
//        if (selectionCanvas != null) selectionCanvas.SetActive(true);

//        Cursor.lockState = CursorLockMode.None;
//        Cursor.visible = true;
//    }

//    void Update()
//    {
//        // Only allow spawning if the local player hasn't picked yet
//        if (hasSpawned) return;

//        // Use New Input System as seen in your past code
//        if (Keyboard.current.digit1Key.wasPressedThisFrame)
//        {
//            SpawnPlayer(0); // Desktop
//        }
//        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
//        {
//            SpawnPlayer(1); // VR
//        }
//    }

//    void SpawnPlayer(int type)
//    {
//        hasSpawned = true;
//        if (selectionCanvas != null) selectionCanvas.SetActive(false);

//        // Request the specific prefab from the server
//        RequestSpawnServerRpc(type, NetworkManager.Singleton.LocalClientId);
//    }

//    [ServerRpc(RequireOwnership = false)]
//    void RequestSpawnServerRpc(int type, ulong clientId)
//    {
//        GameObject prefabToSpawn = (type == 0) ? desktopPlayerPrefab : vrPlayerPrefab;
//        GameObject playerInstance = Instantiate(prefabToSpawn);

//        // Spawns the character for everyone
//        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

//        // --- THE TIMER TRIGGER ---
//        // Only start the game timer if the server sees at least 2 players have actually spawned
//        if (NetworkManager.Singleton.ConnectedClients.Count >= 2)
//        {
//            GameManager.Instance.gameActive.Value = true; //
//        }
//    }
//}


using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerSpawner : NetworkBehaviour
{
    [Header("Player Prefabs")]
    public GameObject desktopPlayerPrefab;
    public GameObject vrPlayerPrefab;

    [Header("Start Menu Graphic")]
    public GameObject selectionCanvas;

    private bool hasSpawned = false;

    // Use OnNetworkSpawn instead of Start for reliable UI activation
    public override void OnNetworkSpawn()
    {
        if (IsOwner || IsServer) // Ensure the local player sees the menu
        {
            if (selectionCanvas != null)
            {
                selectionCanvas.SetActive(true);
            }

            // Force cursor to be visible so you can interact
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        if (hasSpawned) return;

        // Ensure Keyboard is actually connected/active
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("Spawning Desktop Player...");
            SpawnPlayer(0);
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Debug.Log("Spawning VR Player...");
            SpawnPlayer(1);
        }
    }

    void SpawnPlayer(int type)
    {
        hasSpawned = true;
        if (selectionCanvas != null) selectionCanvas.SetActive(false);

        // LOCK cursor only for desktop mode
        if (type == 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        RequestSpawnServerRpc(type, NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestSpawnServerRpc(int type, ulong clientId)
    {
        GameObject prefabToSpawn = (type == 0) ? desktopPlayerPrefab : vrPlayerPrefab;
        GameObject playerInstance = Instantiate(prefabToSpawn);

        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

        if (NetworkManager.Singleton.ConnectedClients.Count >= 2)
        {
            GameManager.Instance.gameActive.Value = true;
        }
    }
}