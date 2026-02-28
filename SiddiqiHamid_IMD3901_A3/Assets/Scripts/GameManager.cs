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
//    public NetworkVariable<bool> gameActive = new NetworkVariable<bool>(false); // Start as false

//    [Header("UI References")]
//    public TextMeshProUGUI timerText;
//    public TextMeshProUGUI p1Text;
//    public TextMeshProUGUI p2Text;
//    public TextMeshProUGUI winStatusText;

//    private void Awake() { Instance = this; }

//    public override void OnNetworkSpawn()
//    {
//        // Hide win text at start
//        winStatusText.gameObject.SetActive(false);

//        if (IsServer)
//        {
//            p1Remaining.Value = GameObject.FindGameObjectsWithTag("Player 1 Buttons").Length;
//            p2Remaining.Value = GameObject.FindGameObjectsWithTag("Player 2 Buttons").Length;
//        }
//    }

//    //void Update()
//    //{
//    //    // UI Updates
//    //    timerText.text = "Time: " + Mathf.Ceil(timer.Value).ToString();
//    //    p1Text.text = "P1 Tasks Left: " + p1Remaining.Value;
//    //    p2Text.text = "P2 Tasks Left: " + p2Remaining.Value;

//    //    if (!IsServer) return;

//    //    // Only start game when 2 players are connected
//    //    if (!gameActive.Value && NetworkManager.Singleton.ConnectedClients.Count >= 2)
//    //    {
//    //        gameActive.Value = true;
//    //    }

//    //    if (!gameActive.Value)
//    //    {
//    //        timerText.text = "Waiting for P2 to join...";
//    //        return;
//    //    }

//    //    // Countdown Logic
//    //    if (timer.Value > 0 && gameActive.Value)
//    //    {
//    //        timer.Value -= Time.deltaTime;
//    //    }
//    //    else if (timer.Value <= 0 && gameActive.Value)
//    //    {
//    //        EndGame("GAME OVER - NO ONE WINS!");
//    //    }
//    //}

//    void Update()
//    {
//        // 1. Logic for Server ONLY
//        if (IsServer)
//        {
//            // Only activate game when 2 players are in the session
//            if (!gameActive.Value && NetworkManager.Singleton.ConnectedClients.Count >= 2)
//            {
//                gameActive.Value = true;
//            }

//            // If game is active, count down
//            if (gameActive.Value && timer.Value > 0)
//            {
//                timer.Value -= Time.deltaTime;
//            }
//            else if (gameActive.Value && timer.Value <= 0)
//            {
//                EndGame("GAME OVER - NO ONE WINS!");
//            }
//        }

//        // 2. UI Updates for EVERYONE (Host and Client)
//        p1Text.text = "P1 Tasks Left: " + p1Remaining.Value;
//        p2Text.text = "P2 Tasks Left: " + p2Remaining.Value;

//        // Only show the timer if the game has actually started
//        if (gameActive.Value)
//        {
//            timerText.text = "Time: " + Mathf.Ceil(timer.Value).ToString();
//        }
//        else
//        {
//            timerText.text = "Waiting for P2 to join...";
//        }
//    }

//    [ServerRpc(RequireOwnership = false)]
//    public void TaskCompletedServerRpc(string buttonTag)
//    {
//        if (!gameActive.Value) return;

//        if (buttonTag == "Player 1 Buttons") p1Remaining.Value--;
//        else if (buttonTag == "Player 2 Buttons") p2Remaining.Value--;

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
//        winStatusText.gameObject.SetActive(true); // Show the text
//    }
//}

using UnityEngine;
using Unity.Netcode;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public NetworkVariable<float> timer = new NetworkVariable<float>(60f);
    public NetworkVariable<int> p1Remaining = new NetworkVariable<int>(0);
    public NetworkVariable<int> p2Remaining = new NetworkVariable<int>(0);

    // Changing this to a NetworkVariable so the Client knows when to stop seeing "Waiting..."
    //public NetworkVariable<bool> gameActive = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> gameActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI p1Text;
    public TextMeshProUGUI p2Text;
    public TextMeshProUGUI winStatusText;

    private void Awake() { Instance = this; }

    public override void OnNetworkSpawn()
    {
        winStatusText.gameObject.SetActive(false);

        if (IsServer)
        {
            gameActive.Value = false;

            p1Remaining.Value = GameObject.FindGameObjectsWithTag("Player 1 Buttons").Length;
            p2Remaining.Value = GameObject.FindGameObjectsWithTag("Player 2 Buttons").Length;
        }
    }

    //void Update()
    //{
    //    // 1. Logic for SERVER ONLY (The Host)
    //    if (IsServer)
    //    {
    //        // Start game ONLY when 2 players are connected
    //        if (!gameActive.Value && NetworkManager.Singleton.ConnectedClients.Count >= 2)
    //        {
    //            gameActive.Value = true;
    //        }

    //        if (gameActive.Value && timer.Value > 0)
    //        {
    //            timer.Value -= Time.deltaTime;
    //        }
    //        else if (gameActive.Value && timer.Value <= 0)
    //        {
    //            EndGame("GAME OVER - NO ONE WINS!");
    //        }
    //    }

    //    // 2. UI logic for EVERYONE (The Client will now see this change!)
    //    p1Text.text = "P1 Tasks Left: " + p1Remaining.Value;
    //    p2Text.text = "P2 Tasks Left: " + p2Remaining.Value;

    //    if (gameActive.Value)
    //    {
    //        timerText.text = "Time: " + Mathf.Ceil(timer.Value).ToString();
    //    }
    //    else
    //    {
    //        timerText.text = "Waiting for P2 to join...";
    //    }
    //}

    //void Update()
    //{
    //    // 1. GLOBAL UI UPDATES (Runs for everyone)
    //    p1Text.text = "P1 Tasks Left: " + p1Remaining.Value;
    //    p2Text.text = "P2 Tasks Left: " + p2Remaining.Value;

    //    // Use the NetworkVariable to decide what to show on the clock
    //    if (gameActive.Value)
    //    {
    //        timerText.text = "Time: " + Mathf.Ceil(timer.Value).ToString();
    //    }
    //    else
    //    {
    //        timerText.text = "Waiting for P2 to join...";
    //    }

    //    // 2. SERVER-ONLY LOGIC (Only the Host runs this)
    //    if (!IsServer) return;

    //    // Strictly check if we have 2 or more players
    //    if (!gameActive.Value)
    //    {
    //        if (NetworkManager.Singleton.ConnectedClients.Count >= 2)
    //        {
    //            gameActive.Value = true; // This syncs to the client automatically
    //            Debug.Log("Second player joined! Starting timer.");
    //        }
    //        return; // Exit early so timer doesn't tick yet
    //    }

    //    // Timer countdown only happens when gameActive is true
    //    if (timer.Value > 0)
    //    {
    //        timer.Value -= Time.deltaTime;
    //    }
    //    else
    //    {
    //        EndGame("GAME OVER - NO ONE WINS!");
    //    }
    //}

    //void Update()
    //{
    //    // 1. VISUAL FEEDBACK: Update the UI for everyone based on game state
    //    p1Text.text = "P1 Tasks Left: " + p1Remaining.Value;
    //    p2Text.text = "P2 Tasks Left: " + p2Remaining.Value;

    //    if (gameActive.Value)
    //    {
    //        timerText.text = "Time: " + Mathf.Ceil(timer.Value).ToString();
    //    }
    //    else
    //    {
    //        timerText.text = "Waiting for P2 to join...";
    //        Debug.Log("Waiting for P2 to Join");

    //    }

    //    // 2. SERVER LOGIC: Only the Host (Server) manages the timer and player count
    //    if (!IsServer) return;

    //    // Check if both players are present to start the game
    //    if (!gameActive.Value)
    //    {
    //        // NetworkManager.Singleton.ConnectedClients.Count includes the Host
    //        if (NetworkManager.Singleton.ConnectedClients.Count >= 2)
    //        {
    //            gameActive.Value = true; // This syncs to the Client automatically
    //            Debug.Log("Both players joined. Timer starting now!");
    //        }
    //        return; // Don't run the countdown while still waiting
    //    }

    //    // 3. COUNTDOWN: Only runs if gameActive is true
    //    if (timer.Value > 0)
    //    {
    //        timer.Value -= Time.deltaTime;
    //    }
    //    else
    //    {
    //        EndGame("GAME OVER - NO ONE WINS!");
    //    }
    //}


    //void Update()
    //{
    //    // If the texts are still null, the script will crash here.
    //    // Make sure you dragged them into the slots!
    //    p1Text.text = "P1 Tasks Left: " + p1Remaining.Value;
    //    p2Text.text = "P2 Tasks Left: " + p2Remaining.Value;

    //    if (!IsServer)
    //    {
    //        // CLIENTS: Just show the time if the server says it's active
    //        if (gameActive.Value) timerText.text = "Time: " + Mathf.Ceil(timer.Value).ToString();
    //        else timerText.text = "Waiting for P2 to join...";
    //        return;
    //    }

    //    // SERVER (HOST) LOGIC:
    //    if (!gameActive.Value)
    //    {
    //        timerText.text = "Waiting for P2 to join...";
    //        Debug.Log("Waiting for P2");

    //        // The Host is always 1. We need 2.
    //        if (NetworkManager.Singleton.ConnectedClients.Count >= 2)
    //        {
    //            gameActive.Value = true;
    //            Debug.Log("P2 joined");
    //        }
    //    }
    //    else
    //    {
    //        timerText.text = "Time: " + Mathf.Ceil(timer.Value).ToString();
    //        if (timer.Value > 0) timer.Value -= Time.deltaTime;
    //        else EndGame("GAME OVER!");
    //    }
    //}











    void Update()
    {
        // Always update these counts so players see progress
        p1Text.text = "P1 Tasks Left: " + p1Remaining.Value;
        p2Text.text = "P2 Tasks Left: " + p2Remaining.Value;

        // The Logic Gate
        if (gameActive.Value == true)
        {
            timerText.text = "Time: " + Mathf.Ceil(timer.Value).ToString();

            if (IsServer)
            {
                if (timer.Value > 0) timer.Value -= Time.deltaTime;
                else EndGame("GAME OVER!");
            }
        }
        else
        {
            timerText.text = "Waiting for P2 to join...";

            if (IsServer && NetworkManager.Singleton.ConnectedClients.Count >= 2)
            {
                gameActive.Value = true; // This triggers the timer for everyone
            }
        }
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