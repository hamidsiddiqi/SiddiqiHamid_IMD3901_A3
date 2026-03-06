using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerNameTag : NetworkBehaviour
{
    // Text that goes above players's head to show which is host and which is client
    public TextMeshProUGUI nameTagText; 

    public override void OnNetworkSpawn()
    {
        // For Host
        if (OwnerClientId == 0)
        {
            nameTagText.text = "P1 (Host)";
            nameTagText.color = Color.red; 
        }
        // For Client
        else
        {
            nameTagText.text = "P2 (Client)";
            nameTagText.color = Color.green; 
        }
    }

    void Update()
    {
        // Making the text face the player's camera
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                             Camera.main.transform.rotation * Vector3.up);
        }
    }
}
