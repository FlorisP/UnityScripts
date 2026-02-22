using UnityEngine;

public class Cache : MonoBehaviour
{    
    public static readonly WaitForSeconds waitForSeconds_0_15 = new(0.15f);
    public static readonly WaitForSeconds waitForSeconds_0_25 = new(0.25f);

    public static readonly WaitForSecondsRealtime waitForSecondsRealtime_0_1 = new(0.1f);
    public static readonly WaitForSecondsRealtime waitForSecondsRealtime_1 = new(1f);
    public static readonly WaitForSecondsRealtime waitForSecondsRealtime_2 = new(2);
    public static readonly WaitForSecondsRealtime waitForSecondsRealtime_3 = new(3);
}
