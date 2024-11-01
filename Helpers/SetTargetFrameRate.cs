using UnityEngine;

public class SetTargetFrameRate : MonoBehaviour
{
    public int targetFPS = 60;
    public bool printValue = false;

    void Start()
    {
        if (printValue)
        {
            print($"Initial Target FPS: {targetFPS}");
        }
    }

    void OnValidate()
    {        
        Application.targetFrameRate = targetFPS;
        
        if (printValue)
        {
            print($"Changed Target FPS: {targetFPS}");
        }
    }
}