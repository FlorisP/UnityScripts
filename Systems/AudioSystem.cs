using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Scripts;

public class AudioSystem : MonoBehaviour
{
    public static AudioSystem A;

    AudioSource ambientAudioSource;
    [OnValueChanged("PlayAmbient")] public AudioClip ambient;

    void Awake()
    {
        if (A == null)
        {
            A = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1.0f)
    {
        GameObject soundGameObject = new GameObject("Sound_" + clip.name);
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(soundGameObject, clip.length);
                print("sound");

    }

    public void PlayAmbient(AudioClip newAmbientClip = null)
    {
        if (newAmbientClip != null)
            ambient = newAmbientClip;

        if (ambient == null)
            return;

        if (ambientAudioSource==null)
        {
            ambientAudioSource = gameObject.AddComponent<AudioSource>();
            ambientAudioSource.loop = true;
        }

        ambientAudioSource.clip = ambient;
        ambientAudioSource.volume = 1.0f;
        ambientAudioSource.Play();
        print("play");
    }

}