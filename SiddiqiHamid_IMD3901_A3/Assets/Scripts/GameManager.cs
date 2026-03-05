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
    public NetworkVariable<bool> gameActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI p1Text;
    public TextMeshProUGUI p2Text;
    public TextMeshProUGUI winStatusText;

    [Header("Background Music")]
    public AudioSource backgroundMusic;

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
        gameActive.OnValueChanged += OnGameActiveChanged;
    }




    public override void OnNetworkDespawn()
    {
        // Always clean up listeners
        gameActive.OnValueChanged -= OnGameActiveChanged;
    }

    private void OnGameActiveChanged(bool previous, bool current)
    {
        if (current && backgroundMusic != null)
        {
            backgroundMusic.Play(); // Starts music on all clients simultaneously
        }
        else if (!current && backgroundMusic != null)
        {
            backgroundMusic.Stop(); // Stops music once the game ends
        }
    }



    void Update()
    {
        p1Text.text = "P1 Tasks Left: " + p1Remaining.Value;
        p2Text.text = "P2 Tasks Left: " + p2Remaining.Value;

        if (IsServer)
        {
            if (!gameActive.Value && NetworkManager.Singleton.ConnectedClients.Count >= 2)
            {
                gameActive.Value = true;
            }

            if (gameActive.Value && !winStatusText.gameObject.activeSelf)
            {
                if (timer.Value > 0)
                {
                    timer.Value -= Time.deltaTime;
                }
                else
                {
                    EndGame("GAME OVER!");
                }
            }
        }

        // --- NEW FORMATTING LOGIC ---
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
        Debug.Log("Game Ended: " + message);
    }

    [ClientRpc]
    private void UpdateWinTextClientRpc(string message)
    {
        winStatusText.text = message;
        winStatusText.gameObject.SetActive(true);
    }
}
