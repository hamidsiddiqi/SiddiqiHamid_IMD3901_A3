using UnityEngine;

public class TaskButton : MonoBehaviour
{
    public Color pressedColor = Color.green;
    private MeshRenderer meshRenderer;
    private bool isPressed = false;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Interact()
    {
        if (!isPressed)
        {
            isPressed = true;
            meshRenderer.material.color = pressedColor; 
            Debug.Log(gameObject.name + " Pressed!");

            // This is where you'll eventually trigger the "Win" logic
        }
    }
}


