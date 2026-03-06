using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerNameTag : NetworkBehaviour
{
    public TextMeshProUGUI nameTagText;

    public override void OnNetworkSpawn()
    {
        // OwnerClientId 0 is always the Host (Player 1)
        if (OwnerClientId == 0)
        {
            nameTagText.text = "P1 (Host)";
            nameTagText.color = Color.red; // Match your button theme!
        }
        else
        {
            nameTagText.text = "P2 (Client)";
            nameTagText.color = Color.green; // Match your button theme!
        }
    }

    void Update()
    {
        // Simple Billboard effect: Make the text face the local player's camera
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                             Camera.main.transform.rotation * Vector3.up);
        }
    }
}
