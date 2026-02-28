using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    // Track scores and timer across the network
    public NetworkVariable<int> p1Score = new NetworkVariable<int>(0);
    public NetworkVariable<int> p2Score = new NetworkVariable<int>(0);
    public NetworkVariable<float> timer = new NetworkVariable<float>(60f);

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // Only the server should manage the timer
        if (IsServer && timer.Value > 0)
        {
            timer.Value -= Time.deltaTime;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RegisterButtonPressServerRpc(string buttonTag)
    {
        if (buttonTag == "Player 1 Buttons")
        {
            p1Score.Value++;
        }
        else if (buttonTag == "Player 2 Buttons")
        {
            p2Score.Value++;
        }

        Debug.Log($"Score - P1: {p1Score.Value} | P2: {p2Score.Value}");
    }
}