using UnityEngine;
using Sirenix.OdinInspector;

public class OldAudioSystem : MonoBehaviour
{
    static OldAudioSystem _instance;
    public static OldAudioSystem A
    {
        get { 
            if (_instance == null) {
                _instance = FindAnyObjectByType<OldAudioSystem>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;            
        }
    }

    public AudioSource ambientSource;
    [OnValueChanged("PlayAmbient")] public AudioClip ambientClip;

    void OnEnable()
    {
        if (_instance == null)
        {
            _instance = this;
            PlayAmbient();
        }
        else if (_instance != this)        
            Destroy(gameObject);
    }
    
    public void PlaySound(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        GameObject soundGameObject = new GameObject("Sound: " + clip.name);
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume; // 0-1
        audioSource.pitch = pitch; // 0-3
        audioSource.Play();
        Destroy(soundGameObject, clip.length);
    }

    public void PlayAmbient(AudioClip newAmbientClip = null)
    {
        if (newAmbientClip != null)
            ambientClip = newAmbientClip;
        if (ambientClip == null)
            return;

        ambientSource.clip = ambientClip;
        ambientSource.loop = true;
        ambientSource.Play();
    }

}