//using UnityEngine;
//using Unity.Netcode;
//using TMPro;

//public class GameManager : NetworkBehaviour
//{
//    public static GameManager Instance;

//    [Header("Game Settings")]
//    public NetworkVariable<float> timer = new NetworkVariable<float>(60f);
//    public NetworkVariable<int> p1Remaining = new NetworkVariable<int>(0);
//    public NetworkVariable<int> p2Remaining = new NetworkVariable<int>(0);

//    // Changing this to a NetworkVariable so the Client knows when to stop seeing "Waiting..."
//    //public NetworkVariable<bool> gameActive = new NetworkVariable<bool>(false);
//    public NetworkVariable<bool> gameActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

//    [Header("UI References")]
//    public TextMeshProUGUI timerText;
//    public TextMeshProUGUI p1Text;
//    public TextMeshProUGUI p2Text;
//    public TextMeshProUGUI winStatusText;

//    private void Awake() { Instance = this; }

//    public override void OnNetworkSpawn()
//    {
//        winStatusText.gameObject.SetActive(false);

//        if (IsServer)
//        {
//            gameActive.Value = false;

//            p1Remaining.Value = GameObject.FindGameObjectsWithTag("Player 1 Buttons").Length;
//            p2Remaining.Value = GameObject.FindGameObjectsWithTag("Player 2 Buttons").Length;
//        }
//    }



//    void Update()
//    {
//        // Always update these counts
//        p1Text.text = "P1 Tasks Left: " + p1Remaining.Value;
//        p2Text.text = "P2 Tasks Left: " + p2Remaining.Value;

//        if (IsServer)
//        {
//            // 1. Only start if 2 players are in
//            if (!gameActive.Value && NetworkManager.Singleton.ConnectedClients.Count >= 2)
//            {
//                gameActive.Value = true;
//            }

//            // 2. THE FIX: Only count down if the game is active AND nobody has won yet
//            if (gameActive.Value && !winStatusText.gameObject.activeSelf)
//            {
//                if (timer.Value > 0)
//                {
//                    timer.Value -= Time.deltaTime;
//                }
//                else
//                {
//                    // Timer hit zero naturally
//                    EndGame("GAME OVER!");
//                }
//            }
//        }

//        if (winStatusText.gameObject.activeSelf || gameActive.Value)
//        {
//            timerText.text = "Time: " + FormatTime(timer.Value);
//        }
//        else
//        {
//            timerText.text = "Waiting for P2 to join...";
//        }

//    }

//    // Helper function to turn float seconds into MM:SS string
//    string FormatTime(float timeToDisplay)
//    {
//        if (timeToDisplay < 0) timeToDisplay = 0;

//        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
//        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

//        return string.Format("{0:00}:{1:00}", minutes, seconds);
//    }


//    [ServerRpc(RequireOwnership = false)]
//    public void TaskCompletedServerRpc(string buttonTag)
//    {
//        // If the game is already over, don't process more button presses
//        if (!gameActive.Value) return;

//        if (buttonTag == "Player 1 Buttons") p1Remaining.Value--;
//        else if (buttonTag == "Player 2 Buttons") p2Remaining.Value--;

//        // CHECK FOR WINNER IMMEDIATELY
//        if (p1Remaining.Value <= 0)
//        {
//            EndGame("PLAYER 1 WINS!");
//        }
//        else if (p2Remaining.Value <= 0)
//        {
//            EndGame("PLAYER 2 WINS!");
//        }
//    }

//    private void EndGame(string message)
//    {
//        // This is the "Kill Switch" for the timer
//        gameActive.Value = false;

//        // Tell all clients to show the final message
//        UpdateWinTextClientRpc(message);

//        Debug.Log("Game Ended: " + message);
//    }





//    [ClientRpc]
//    private void UpdateWinTextClientRpc(string message)
//    {
//        winStatusText.text = message;
//        winStatusText.gameObject.SetActive(true);
//    }
//}


using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public NetworkVariable<float> timer = new NetworkVariable<float>(60f);
    public NetworkVariable<int> p1Remaining = new NetworkVariable<int>(0);
    public NetworkVariable<int> p2Remaining = new NetworkVariable<int>(0);
    public NetworkVariable<bool> gameActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI p1Text;
    public TextMeshProUGUI p2Text;
    public TextMeshProUGUI winStatusText;
    public GameObject selectionCanvas; // Your Startup Screen

    [Header("Player Spawning")]
    public GameObject desktopPlayerPrefab;
    public GameObject vrPlayerPrefab;
    private bool localPlayerHasSpawned = false;

    private void Awake() { Instance = this; }

    //public override void OnNetworkSpawn()
    //{
    //    winStatusText.gameObject.SetActive(false);

    //    // Reset game state on server
    //    if (IsServer)
    //    {
    //        gameActive.Value = false;
    //        p1Remaining.Value = GameObject.FindGameObjectsWithTag("Player 1 Buttons").Length;
    //        p2Remaining.Value = GameObject.FindGameObjectsWithTag("Player 2 Buttons").Length;
    //    }

    //    // Show the startup graphic for everyone joining
    //    if (selectionCanvas != null)
    //    {
    //        selectionCanvas.SetActive(true);
    //        Cursor.lockState = CursorLockMode.None;
    //        Cursor.visible = true;
    //    }
    //}

    public override void OnNetworkSpawn()
    {
        // Hide win text at start
        winStatusText.gameObject.SetActive(false);

        if (IsServer)
        {
            gameActive.Value = false;
            p1Remaining.Value = GameObject.FindGameObjectsWithTag("Player 1 Buttons").Length;
            p2Remaining.Value = GameObject.FindGameObjectsWithTag("Player 2 Buttons").Length;
        }

        // FORCE reveal graphic for the player who just joined
        if (IsOwner || IsClient)
        {
            if (selectionCanvas != null)
            {
                selectionCanvas.SetActive(true);
                Debug.Log("Graphic Revealed: Choose your control!");
            }

            // Ensure cursor is free to pick a mode
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        // Update task counts UI
        p1Text.text = "P1 Tasks Left: " + p1Remaining.Value;
        p2Text.text = "P2 Tasks Left: " + p2Remaining.Value;

        // --- HANDLE PLAYER SELECTION ---
        if (!localPlayerHasSpawned && selectionCanvas != null && selectionCanvas.activeSelf)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SpawnPlayer(0); // Desktop
            else if (Keyboard.current.digit2Key.wasPressedThisFrame) SpawnPlayer(1); // VR
        }

        // --- SERVER TIMER LOGIC ---
        if (IsServer && gameActive.Value && !winStatusText.gameObject.activeSelf)
        {
            if (timer.Value > 0) timer.Value -= Time.deltaTime;
            else EndGame("GAME OVER!");
        }

        // --- UI TIMER DISPLAY (MM:SS) ---
        if (winStatusText.gameObject.activeSelf || gameActive.Value)
        {
            timerText.text = "Time: " + FormatTime(timer.Value);
        }
        else
        {
            timerText.text = "Waiting for players...";
        }
    }

    // Spawning logic integrated from PlayerSpawner
    void SpawnPlayer(int type)
    {
        localPlayerHasSpawned = true;
        selectionCanvas.SetActive(false);

        if (type == 0) // Desktop-specific cursor lock
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

        // Only activate game when both players have chosen a character
        if (NetworkManager.Singleton.ConnectedClients.Count >= 2)
        {
            gameActive.Value = true;
        }
    }

    string FormatTime(float timeToDisplay)
    {
        if (timeToDisplay < 0) timeToDisplay = 0;
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    [ServerRpc(RequireOwnership = false)]
    public void TaskCompletedServerRpc(string buttonTag)
    {
        if (!gameActive.Value) return;

        if (buttonTag == "Player 1 Buttons") p1Remaining.Value--;
        else if (buttonTag == "Player 2 Buttons") p2Remaining.Value--;

        if (p1Remaining.Value <= 0) EndGame("PLAYER 1 WINS!");
        else if (p2Remaining.Value <= 0) EndGame("PLAYER 2 WINS!");
    }

    private void EndGame(string message)
    {
        gameActive.Value = false;
        UpdateWinTextClientRpc(message);
    }

    [ClientRpc]
    private void UpdateWinTextClientRpc(string message)
    {
        winStatusText.text = message;
        winStatusText.gameObject.SetActive(true);
    }
}


//using UnityEngine;
//using Unity.Netcode;
//using TMPro;
//using UnityEngine.InputSystem;

//public class GameManager : NetworkBehaviour
//{
//    public static GameManager Instance;

//    [Header("Game Settings")]
//    public NetworkVariable<float> timer = new NetworkVariable<float>(600f);
//    public NetworkVariable<int> p1Remaining = new NetworkVariable<int>(0);
//    public NetworkVariable<int> p2Remaining = new NetworkVariable<int>(0);
//    public NetworkVariable<bool> gameActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

//    [Header("UI References")]
//    public TextMeshProUGUI timerText;
//    public TextMeshProUGUI p1Text;
//    public TextMeshProUGUI p2Text;
//    public TextMeshProUGUI winStatusText;
//    public GameObject selectionCanvas; // Your Startup Graphic

//    [Header("Prefabs")]
//    public GameObject desktopPlayerPrefab;
//    public GameObject vrPlayerPrefab;
//    private bool hasSpawned = false;

//    private void Awake() { Instance = this; }

//    public override void OnNetworkSpawn()
//    {
//        winStatusText.gameObject.SetActive(false);

//        if (IsServer)
//        {
//            gameActive.Value = false;
//            // Counts buttons based on tags
//            p1Remaining.Value = GameObject.FindGameObjectsWithTag("Player 1 Buttons").Length;
//            p2Remaining.Value = GameObject.FindGameObjectsWithTag("Player 2 Buttons").Length;
//        }

//        // Always show the selection screen for the local player when they join
//        if (selectionCanvas != null)
//        {
//            selectionCanvas.SetActive(true);
//            Cursor.lockState = CursorLockMode.None;
//            Cursor.visible = true;
//        }
//    }

//    void Update()
//    {
//        p1Text.text = "P1 Tasks Left: " + p1Remaining.Value;
//        p2Text.text = "P2 Tasks Left: " + p2Remaining.Value;

//        // LOCAL SELECTION: Handles Desktop (1) or VR (2)
//        if (!hasSpawned && selectionCanvas != null && selectionCanvas.activeSelf)
//        {
//            if (Keyboard.current.digit1Key.wasPressedThisFrame) SpawnPlayer(0);
//            else if (Keyboard.current.digit2Key.wasPressedThisFrame) SpawnPlayer(1);
//        }

//        // Server-side timer countdown
//        if (IsServer && gameActive.Value && !winStatusText.gameObject.activeSelf)
//        {
//            if (timer.Value > 0) timer.Value -= Time.deltaTime;
//            else EndGame("GAME OVER!");
//        }

//        // UI DISPLAY with MM:SS formatting
//        if (winStatusText.gameObject.activeSelf || gameActive.Value)
//            timerText.text = "Time: " + FormatTime(timer.Value);
//        else
//            timerText.text = "Waiting for players...";
//    }

//    void SpawnPlayer(int type)
//    {
//        hasSpawned = true;
//        selectionCanvas.SetActive(false);

//        // Lock cursor ONLY for Desktop mode
//        if (type == 0) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }

//        RequestSpawnServerRpc(type, NetworkManager.Singleton.LocalClientId);
//    }

//    [ServerRpc(RequireOwnership = false)]
//    void RequestSpawnServerRpc(int type, ulong clientId)
//    {
//        GameObject prefabToSpawn = (type == 0) ? desktopPlayerPrefab : vrPlayerPrefab;
//        GameObject playerInstance = Instantiate(prefabToSpawn);

//        // Spawns and assigns ownership
//        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

//        // Only start game when both players have joined
//        if (NetworkManager.Singleton.ConnectedClients.Count >= 2) gameActive.Value = true;
//    }

//    // --- HELPER FUNCTIONS ---

//    string FormatTime(float timeToDisplay)
//    {
//        if (timeToDisplay < 0) timeToDisplay = 0;
//        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
//        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
//        return string.Format("{0:00}:{1:00}", minutes, seconds);
//    }

//    [ServerRpc(RequireOwnership = false)]
//    public void TaskCompletedServerRpc(string buttonTag)
//    {
//        if (!gameActive.Value) return;

//        if (buttonTag == "Player 1 Buttons") p1Remaining.Value--;
//        else if (buttonTag == "Player 2 Buttons") p2Remaining.Value--;

//        // Check for victory conditions
//        if (p1Remaining.Value <= 0) EndGame("PLAYER 1 WINS!");
//        else if (p2Remaining.Value <= 0) EndGame("PLAYER 2 WINS!");
//    }

//    private void EndGame(string message)
//    {
//        gameActive.Value = false;
//        UpdateWinTextClientRpc(message);
//    }

//    [ClientRpc]
//    private void UpdateWinTextClientRpc(string message)
//    {
//        winStatusText.text = message;
//        winStatusText.gameObject.SetActive(true);
//    }
//}