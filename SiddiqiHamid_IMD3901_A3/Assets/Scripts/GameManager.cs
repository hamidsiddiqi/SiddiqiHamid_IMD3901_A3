using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public NetworkVariable<float> timer = new NetworkVariable<float>(60f); // 60 second limit
    public NetworkVariable<int> p1Score = new NetworkVariable<int>(0);
    public NetworkVariable<int> p2Score = new NetworkVariable<int>(0);
    public NetworkVariable<int> totalPressed = new NetworkVariable<int>(0);

    public int totalButtonsInRoom = 20;

    void Awake() { Instance = this; }

    void Update()
    {
        if (IsServer && timer.Value > 0 && totalPressed.Value < totalButtonsInRoom)
        {
            timer.Value -= Time.deltaTime;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ButtonPressedServerRpc(ulong clientId)
    {
        totalPressed.Value++;
        if (clientId == 0) p1Score.Value++; // Host/P1
        else p2Score.Value++; // Client/P2

        if (totalPressed.Value >= totalButtonsInRoom)
        {
            Debug.Log("Collaboration Successful! Room Cleared.");
            DetermineWinner();
        }
    }

    void DetermineWinner()
    {
        if (p1Score.Value > p2Score.Value) Debug.Log("Player 1 wins the scramble!");
        else if (p2Score.Value > p1Score.Value) Debug.Log("Player 2 wins the scramble!");
        else Debug.Log("It's a tie!");
    }
}