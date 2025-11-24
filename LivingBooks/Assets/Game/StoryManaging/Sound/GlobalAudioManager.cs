using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class GlobalAudioManager : MonoBehaviour
{
    public static GlobalAudioManager Instance { get; private set; }

    public AudioSource ambientSource;
    public AudioSource narratorSource;
    public AudioSource sfxSource;

    [System.Serializable]
    public class SoundEntry
    {
        public string Key;       
        public AudioClip Clip;  
    }
    public List<SoundEntry> soundList = new List<SoundEntry>();

    private Dictionary<string, AudioClip> sounds;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sounds = new Dictionary<string, AudioClip>();
        foreach (var s in soundList)
        {
            if (!string.IsNullOrEmpty(s.Key) && s.Clip)
                sounds[s.Key] = s.Clip;
        }
    }

    public void PlayAmbient(string key, float volume = 1f)
    {
        if (!sounds.TryGetValue(key, out var clip)) 
            return;

        ambientSource.clip = clip;
        ambientSource.volume = volume;
        ambientSource.loop = true;
        ambientSource.Play();
    }

    public void StopAmbient() => ambientSource.Stop();

    public void PlayNarrator(string key, float volume = 1f)
    {
        if (!sounds.TryGetValue(key, out var clip)) return;

        narratorSource.clip = clip;
        narratorSource.volume = volume;
        narratorSource.loop = false;
        narratorSource.Play();
    }

    public void StopNarrator() => narratorSource.Stop();

    public void PlaySmallSound(string key, float volume = 1f)
    {
        if (!sounds.TryGetValue(key, out var clip))
        {
            Debug.LogWarning($"[GlobalAudioManager] Sound with a : {key} not exist");
            return;
        }

        sfxSource.PlayOneShot(clip, volume);
    }
}
