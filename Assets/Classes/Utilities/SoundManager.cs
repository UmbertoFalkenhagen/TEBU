using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [HideInInspector]
        public AudioSource source; // Ensure this line is correctly defined
    }

    public List<Sound> sounds;
    private Dictionary<string, AudioSource> playingSounds = new Dictionary<string, AudioSource>();

    public static SoundManager Instance { get; private set; }

    void Awake()
    {
        // Singleton pattern to ensure only one SoundManager exists
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize AudioSource for each sound
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.spatialBlend = 0; // This makes the sound global
        }
    }
    private void Start()
    {
        PlaySoundForever("MainSoundtrack");
        ChangeSoundVolume("EngineRunning", 0.5f);
    }

    public void PlaySoundOnce(string soundName)
    {
        Sound sound = sounds.Find(s => s.name == soundName);
        if (sound != null && sound.clip != null)
        {
            sound.source.Play();
        }
    }

    public void PlaySoundForever(string soundName)
    {
        if (playingSounds.ContainsKey(soundName))
        {
            return; // Sound is already playing
        }

        Sound sound = sounds.Find(s => s.name == soundName);
        if (sound != null && sound.clip != null)
        {
            sound.source.loop = true;
            sound.source.Play();
            playingSounds[soundName] = sound.source;
        }
    }

    public void StopSound(string soundName)
    {
        if (playingSounds.TryGetValue(soundName, out AudioSource source))
        {
            source.Stop();
            playingSounds.Remove(soundName);
        }
    }

    public float GetSoundLength(string soundName)
    {
        Sound sound = sounds.Find(s => s.name == soundName);
        return sound != null ? sound.clip.length : 0f;
    }

    public void StopAllSounds()
    {
        foreach (var kvp in playingSounds)
        {
            kvp.Value.Stop();
        }
        playingSounds.Clear();
    }

    public void ChangeSoundVolume(string soundName, float newVolume)
    {
        Sound sound = sounds.Find(s => s.name == soundName);
        if (sound != null && sound.source != null)
        {
            sound.source.volume = Mathf.Clamp(newVolume, 0f, 1f); // Clamp the volume between 0 and 1
        }
    }

    public void ChangeSoundVolumeOverTime(string soundName, float targetVolume, float duration)
    {
        Sound sound = sounds.Find(s => s.name == soundName);
        if (sound != null && sound.source != null)
        {
            StartCoroutine(ChangeVolume(sound.source, targetVolume, duration));
        }
    }

    private IEnumerator ChangeVolume(AudioSource source, float targetVolume, float duration)
    {
        float currentTime = 0;
        float startVolume = source.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            source.volume = Mathf.Clamp(newVolume, 0f, 1f);
            yield return null;
        }

        source.volume = targetVolume; // Ensure the final volume is exactly the target volume
    }

}