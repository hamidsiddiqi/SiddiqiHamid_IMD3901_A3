using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;
    [SerializeField] private GameObject menuPanel; // The panel holding these buttons

    private void Awake()
    {
        // 1. Assign Button Listeners
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
        // Hide the buttons once a connection is started
        menuPanel.SetActive(false);
    }
}