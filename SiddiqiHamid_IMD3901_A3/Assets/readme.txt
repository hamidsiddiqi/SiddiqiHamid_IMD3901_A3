https://assetstore.unity.com/packages/3d/environments/sci-fi/megapoly-art-vintage-control-room-190538

https://www.freepik.com/free-photo/gray-tiled-wall_4102545.htm#fromView=search&page=1&position=7&uuid=6775f2ae-2dfe-49d0-a9e3-293bb42f4df6&query=space+wall+texture

https://www.freepik.com/free-photo/grunge-style-metallic-texture-background_8210786.htm#fromView=search&page=1&position=17&uuid=10a605cc-9503-4f4a-8436-c0677723724f&query=space+station+wall+texture

https://www.freepik.com/free-photo/metallic-wall-design-element-textured-wallpaper-concept_2971618.htm#fromView=search&page=1&position=41&uuid=10a605cc-9503-4f4a-8436-c0677723724f&query=space+station+wall+texture

https://www.freepik.com/free-photo/photo-wood-texture-pattern_210126230.htm#fromView=search&page=2&position=4&uuid=10a605cc-9503-4f4a-8436-c0677723724f&query=space+station+wall+texture


//using UnityEngine;
//using Unity.Netcode;

//public class GameManager : NetworkBehaviour
//{
//    public static GameManager Instance;

//    // Track scores and timer across the network
//    public NetworkVariable<int> p1Score = new NetworkVariable<int>(0);
//    public NetworkVariable<int> p2Score = new NetworkVariable<int>(0);
//    public NetworkVariable<float> timer = new NetworkVariable<float>(60f);

//    private void Awake()
//    {
//        Instance = this;
//    }

//    void Update()
//    {
//        // Only the server should manage the timer
//        if (IsServer && timer.Value > 0)
//        {
//            timer.Value -= Time.deltaTime;
//        }
//    }

//    [ServerRpc(RequireOwnership = false)]
//    public void RegisterButtonPressServerRpc(string buttonTag)
//    {
//        if (buttonTag == "Player 1 Buttons")
//        {
//            p1Score.Value++;
//        }
//        else if (buttonTag == "Player 2 Buttons")
//        {
//            p2Score.Value++;
//        }

//        Debug.Log($"Score - P1: {p1Score.Value} | P2: {p2Score.Value}");
//    }
//}
