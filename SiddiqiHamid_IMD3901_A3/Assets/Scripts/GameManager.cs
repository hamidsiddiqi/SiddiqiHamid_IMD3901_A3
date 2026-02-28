using UnityEngine;
using Unity.Netcode;
using TMPro; // Ensure you have TextMeshPro installed

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public NetworkVariable<float> timer = new NetworkVariable<float>(60f);
    public NetworkVariable<int> p1Remaining = new NetworkVariable<int>(0);
    public NetworkVariable<int> p2Remaining = new NetworkVariable<int>(0);
    public NetworkVariable<bool> gameActive = new NetworkVariable<bool>(true);

    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI p1Text;
    public TextMeshProUGUI p2Text;
    public TextMeshProUGUI winStatusText;

    private void Awake() { Instance = this; }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Count buttons based on tags at the start
            p1Remaining.Value = GameObject.FindGameObjectsWithTag("Player 1 Buttons").Length;
            p2Remaining.Value = GameObject.FindGameObjectsWithTag("Player 2 Buttons").Length;
        }
    }

    void Update()
    {
        // Update UI for everyone
        timerText.text = "Time: " + Mathf.Ceil(timer.Value).ToString();
        p1Text.text = "P1 Tasks Left: " + p1Remaining.Value;
        p2Text.text = "P2 Tasks Left: " + p2Remaining.Value;

        if (!IsServer || !gameActive.Value) return;

        // Count down the timer
        if (timer.Value > 0)
        {
            timer.Value -= Time.deltaTime;
        }
        else
        {
            gameActive.Value = false;
            winStatusText.text = "TIME UP - NO ONE WINS!";
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TaskCompletedServerRpc(string buttonTag, ulong clientId)
    {
        if (!gameActive.Value) return;

        if (buttonTag == "Player 1 Buttons") p1Remaining.Value--;
        else if (buttonTag == "Player 2 Buttons") p2Remaining.Value--;

        // Check for Winner
        if (p1Remaining.Value <= 0)
        {
            gameActive.Value = false;
            winStatusText.text = "PLAYER 1 WINS!";
        }
        else if (p2Remaining.Value <= 0)
        {
            gameActive.Value = false;
            winStatusText.text = "PLAYER 2 WINS!";
        }
    }
}