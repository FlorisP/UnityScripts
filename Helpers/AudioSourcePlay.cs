using UnityEngine;
using System;
using System.Reflection;
using Sirenix.OdinInspector;

public class AudioSourcePlay : MonoBehaviour
{
    #if UNITY_EDITOR

    public AudioSource source;

    static Type audioUtilType;
    static MethodInfo playMethod;
    static MethodInfo stopAllMethod;

    [Button]
    public void Play()
    {
        if (source == null) source = GetComponent<AudioSource>();
        if (source == null || source.clip == null) return;

        if (Application.isPlaying) { source.Play(); return; }

        EnsureAudioUtil();
        if (playMethod == null) return;

        ParameterInfo[] p = playMethod.GetParameters();
        object[] args =
            p.Length == 1 ? new object[] { source.clip } :
            p.Length == 2 ? new object[] { source.clip, 0 } :
            p.Length == 3 ? new object[] { source.clip, 0, false } :
                            new object[] { source.clip, 0, false, 1f };

        playMethod.Invoke(null, args);
    }

    [Button]
    public void Stop()
    {
        if (Application.isPlaying)
        {
            if (source == null) source = GetComponent<AudioSource>();
            if (source != null) source.Stop();
            return;
        }

        EnsureAudioUtil();
        if (stopAllMethod != null) stopAllMethod.Invoke(null, null);
    }

    static void EnsureAudioUtil()
    {
        if (audioUtilType == null)
            audioUtilType = Type.GetType("UnityEditor.AudioUtil, UnityEditor")
                             ?? Type.GetType("UnityEditorInternal.AudioUtil, UnityEditor");
        if (audioUtilType == null) return;

        if (playMethod == null)
        {
            MethodInfo[] methods = audioUtilType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i].Name != "PlayPreviewClip") continue;
                ParameterInfo[] ps = methods[i].GetParameters();
                if (ps.Length >= 1 && ps[0].ParameterType == typeof(AudioClip))
                {
                    playMethod = methods[i];
                    break;
                }
            }
        }
        if (stopAllMethod == null)
            stopAllMethod = audioUtilType.GetMethod("StopAllPreviewClips",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    }

    #endif
}
