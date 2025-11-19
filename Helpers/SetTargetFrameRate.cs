using UnityEngine;

public class SetTargetFrameRate : MonoBehaviour
{
    public bool isActive = true;
    public int targetFPS = 60;
    public bool printValue = false;

    void Start()
    {
        if (printValue && isActive)
        {
            print($"Initial Target FPS: {targetFPS}");
        }
    }

    void OnValidate()
    {      
        Application.targetFrameRate = isActive ? targetFPS : -1;
        
        if (printValue && isActive)
        {
            print($"Changed Target FPS: {targetFPS}");
        }
    }
}