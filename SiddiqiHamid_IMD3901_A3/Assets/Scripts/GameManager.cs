using UnityEngine;
using Unity.Netcode;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    // For changing whats changing at the same time in Network on both screens
    [Header("Game Settings")]
    public NetworkVariable<float> timer = new NetworkVariable<float>(600f);
    public NetworkVariable<int> p1Remaining = new NetworkVariable<int>(0);
    public NetworkVariable<int> p2Remaining = new NetworkVariable<int>(0);
    public NetworkVariable<bool> gameActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("UI Texts")]
    public TextMeshProUGUI timerText; // Timer text 
    public TextMeshProUGUI p1Text; // Player 1 Buttons Remaining
    public TextMeshProUGUI p2Text; // Player 2 Buttons Remaining
    public TextMeshProUGUI winStatusText;

    // Background Music
    [Header("Background Music")]
    public AudioSource backgroundMusic;

    private void Awake() { Instance = this; }

    public override void OnNetworkSpawn()
    {
        // By default keep Win Status text hidden start of the game
        winStatusText.gameObject.SetActive(false);

        if (IsServer)
        {
        // Checks to see how many buttons each player as based how many are assigned Tags
            gameActive.Value = false;
            p1Remaining.Value = GameObject.FindGameObjectsWithTag("Player 1 Buttons").Length;
            p2Remaining.Value = GameObject.FindGameObjectsWithTag("Player 2 Buttons").Length;
        }
        gameActive.OnValueChanged += OnGameActiveChanged;
    }

    // Only playing Background Audio once game as started
    public override void OnNetworkDespawn()
    {
        gameActive.OnValueChanged -= OnGameActiveChanged;
    }

    private void OnGameActiveChanged(bool previous, bool current)
    {
        if (current && backgroundMusic != null) backgroundMusic.Play();
        else if (!current && backgroundMusic != null) backgroundMusic.Stop();
    }

    // Displays Buttons on both players's screens
    void Update()
    {
        p1Text.text = "P1 Buttons Left: " + p1Remaining.Value;
        p2Text.text = "P2 Buttons Left: " + p2Remaining.Value;

        if (IsServer)
        {
            // Checks to start game once when 2 players join the game
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

        // For Timer Display
        if (winStatusText.gameObject.activeSelf || gameActive.Value)
        {
            timerText.text = "Time: " + FormatTime(timer.Value);
        }
        else
        {
            // Displays this text if only one player has joined the game
            timerText.text = "Waiting for P2 to join...";
        }
    }

    // Fomatting timer to show up on screen as MM:SS
    string FormatTime(float timeToDisplay)
    {
        if (timeToDisplay < 0) timeToDisplay = 0;
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Checks to see if player pressed button then decrement the reaming text by one on both screens
    [ServerRpc(RequireOwnership = false)]
    public void TaskCompletedServerRpc(string buttonTag)
    {
        if (!gameActive.Value) return;

        if (buttonTag == "Player 1 Buttons") p1Remaining.Value--;
        else if (buttonTag == "Player 2 Buttons") p2Remaining.Value--;

        // Passing the Winner ID
        if (p1Remaining.Value <= 0) EndGame(1);
        else if (p2Remaining.Value <= 0) EndGame(2);
    }

    // Fucnction for ending game on both screens once something happens
    private void EndGame(int winnerId)
    {
        gameActive.Value = false;
        UpdateWinTextClientRpc(winnerId);
    }

    // Win Display Texts
    [ClientRpc]
    private void UpdateWinTextClientRpc(int winnerId)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Checks to see if it was Game Over
        if (winnerId == 0)
        {
            winStatusText.text = "GAME OVER!";
            winStatusText.color = Color.black;
        }
        else
        {
            // Checks to see which player won or lost and displays text for those
            bool isHost = NetworkManager.Singleton.IsHost;

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

        // Display texts on players's screens
        winStatusText.gameObject.SetActive(true);
    }
}

