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



    void Update()
    {
        // Always update these counts
        p1Text.text = "P1 Tasks Left: " + p1Remaining.Value;
        p2Text.text = "P2 Tasks Left: " + p2Remaining.Value;

        if (IsServer)
        {
            // 1. Only start if 2 players are in
            if (!gameActive.Value && NetworkManager.Singleton.ConnectedClients.Count >= 2)
            {
                gameActive.Value = true;
            }

            // 2. THE FIX: Only count down if the game is active AND nobody has won yet
            if (gameActive.Value && !winStatusText.gameObject.activeSelf)
            {
                if (timer.Value > 0)
                {
                    timer.Value -= Time.deltaTime;
                }
                else
                {
                    // Timer hit zero naturally
                    EndGame("GAME OVER!");
                }
            }
        }

        // 3. UI DISPLAY: Prioritize the Win Message over everything else
        //if (winStatusText.gameObject.activeSelf)
        //{
        //    // Freeze the timer display so it doesn't flicker or change
        //    timerText.text = "Time: " + Mathf.Ceil(timer.Value).ToString();
        //}
        //else if (gameActive.Value)
        //{
        //    timerText.text = "Time: " + Mathf.Ceil(timer.Value).ToString();
        //}
        //else
        //{
        //    timerText.text = "Waiting for P2 to join...";
        //}



        if (winStatusText.gameObject.activeSelf || gameActive.Value)
        {
            timerText.text = "Time: " + FormatTime(timer.Value);
        }
        else
        {
            timerText.text = "Waiting for P2 to join...";
        }

    }

    // Helper function to turn float seconds into MM:SS string
    string FormatTime(float timeToDisplay)
    {
        if (timeToDisplay < 0) timeToDisplay = 0;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    [ServerRpc(RequireOwnership = false)]
    public void TaskCompletedServerRpc(string buttonTag)
    {
        // If the game is already over, don't process more button presses
        if (!gameActive.Value) return;

        if (buttonTag == "Player 1 Buttons") p1Remaining.Value--;
        else if (buttonTag == "Player 2 Buttons") p2Remaining.Value--;

        // CHECK FOR WINNER IMMEDIATELY
        if (p1Remaining.Value <= 0)
        {
            EndGame("PLAYER 1 WINS!");
        }
        else if (p2Remaining.Value <= 0)
        {
            EndGame("PLAYER 2 WINS!");
        }
    }

    private void EndGame(string message)
    {
        // This is the "Kill Switch" for the timer
        gameActive.Value = false;

        // Tell all clients to show the final message
        UpdateWinTextClientRpc(message);

        Debug.Log("Game Ended: " + message);
    }





    [ClientRpc]
    private void UpdateWinTextClientRpc(string message)
    {
        winStatusText.text = message;
        winStatusText.gameObject.SetActive(true);
    }
}