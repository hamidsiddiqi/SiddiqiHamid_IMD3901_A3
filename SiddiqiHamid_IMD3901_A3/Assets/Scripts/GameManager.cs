using UnityEngine;
using Unity.Netcode;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public NetworkVariable<float> timer = new NetworkVariable<float>(600f);
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
        // Start hidden
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
        gameActive.OnValueChanged -= OnGameActiveChanged;
    }

    private void OnGameActiveChanged(bool previous, bool current)
    {
        if (current && backgroundMusic != null) backgroundMusic.Play();
        else if (!current && backgroundMusic != null) backgroundMusic.Stop();
    }

    void Update()
    {
        p1Text.text = "P1 Buttons Left: " + p1Remaining.Value;
        p2Text.text = "P2 Buttons Left: " + p2Remaining.Value;

        if (IsServer)
        {
            // Auto-start when 2 players join
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
                    EndGame(0); // 0 = Timeout
                }
            }
        }

        // Timer Display
        if (winStatusText.gameObject.activeSelf || gameActive.Value)
        {
            timerText.text = "Time: " + FormatTime(timer.Value);
        }
        else
        {
            timerText.text = "Waiting for P2 to join...";
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

        // Pass the Winner ID
        if (p1Remaining.Value <= 0) EndGame(1);
        else if (p2Remaining.Value <= 0) EndGame(2);
    }

    private void EndGame(int winnerId)
    {
        gameActive.Value = false;
        UpdateWinTextClientRpc(winnerId);
    }

    [ClientRpc]
    private void UpdateWinTextClientRpc(int winnerId)
    {
        // Re-enable cursor so they can click buttons if you have them
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (winnerId == 0)
        {
            winStatusText.text = "GAME OVER!";
            winStatusText.color = Color.black;
        }
        else
        {
            bool isHost = NetworkManager.Singleton.IsHost;

            // Personalize the message based on who is looking at the screen
            if ((winnerId == 1 && isHost) || (winnerId == 2 && !isHost))
            {
                winStatusText.text = "YOU WIN!";
                winStatusText.color = Color.green;
            }
            else
            {
                winStatusText.text = "YOU LOST!";
                winStatusText.color = Color.red;
            }
        }

        winStatusText.gameObject.SetActive(true);
    }
}

