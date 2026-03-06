using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton; // Start Host Button
    [SerializeField] private Button startClientButton; // Start Client Button
    [SerializeField] private GameObject menuPanel; // Menu Panel for Buttons

    private void Awake()
    {
        // Assigning Listeners to host and client to hide panel when button is pressed
        startHostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            HideMenu();
        });

        startClientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            HideMenu();
        });
    }

    private void HideMenu()
    {
        // Hiding the buttons when a connection is started
        menuPanel.SetActive(false);
    }
}