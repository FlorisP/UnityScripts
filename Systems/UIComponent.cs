using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class UISystem : MonoBehaviour
{
    public void Activate(GameObject go, bool setActive)
    {
        go.SetActive(setActive);
    }

    // TODO Fade / transform with dynamics
}
