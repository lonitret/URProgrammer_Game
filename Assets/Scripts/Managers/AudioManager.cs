using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public struct SoundData
    {
        public SoundType type;
        public AudioClip clip;
    }

    [Header("Настройки микшера")]
    [SerializeField] private AudioMixer mainMixer;

    [Header("Список всех звуков в игре")]
    [SerializeField] private List<SoundData> soundList;

    [Header("Громкость")]
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.3f;
    [SerializeField, Range(0f, 1f)] private float ambientVolume = 0.2f;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float voiceVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float minigameTickVolume = 0.45f;

    private AudioSource musicSource;
    private AudioSource ambientSource;
    private AudioSource sfxSource;
    private AudioSource voiceSource;
    private AudioSource heartbeatSource;
    private AudioSource rumbleSource;
    private AudioSource minigameTickSource;

    private Dictionary<SoundType, AudioClip> soundDictionary;
    private float angerHeartbeatVolume;
    private float angerRumbleVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        //musicSource.playOnAwake = false;
        //musicSource.loop = true;
        //musicSource.volume = 0.3f;

        ambientSource = gameObject.AddComponent<AudioSource>();
        //ambientSource.playOnAwake = false;
        //ambientSource.loop = true;
        //ambientSource.volume = 0.2f;

        sfxSource = gameObject.AddComponent<AudioSource>();
        //sfxSource.playOnAwake = false;
        //sfxSource.loop = false;
        //sfxSource.volume = 0.8f;

        voiceSource = gameObject.AddComponent<AudioSource>();

        minigameTickSource = gameObject.AddComponent<AudioSource>();
        minigameTickSource.loop = true;

        heartbeatSource = gameObject.AddComponent<AudioSource>();
        heartbeatSource.loop = true;
        heartbeatSource.volume = 0f;

        rumbleSource = gameObject.AddComponent<AudioSource>();
        rumbleSource.loop = true;
        rumbleSource.volume = 0f;

        if (mainMixer != null)
        {
            musicSource.outputAudioMixerGroup = mainMixer.FindMatchingGroups("Music")[0];
            ambientSource.outputAudioMixerGroup = mainMixer.FindMatchingGroups("Ambient")[0];
            sfxSource.outputAudioMixerGroup = mainMixer.FindMatchingGroups("SFX")[0];
            voiceSource.outputAudioMixerGroup = mainMixer.FindMatchingGroups("SFX")[0];
            minigameTickSource.outputAudioMixerGroup = mainMixer.FindMatchingGroups("SFX")[0];

            heartbeatSource.outputAudioMixerGroup = mainMixer.FindMatchingGroups("Master")[0];
            rumbleSource.outputAudioMixerGroup = mainMixer.FindMatchingGroups("Master")[0];
        }

        LoadVolumeSettings();
        musicSource.loop = true;
        ambientSource.loop = true;
        ApplyVolumeSettings();

        soundDictionary = new Dictionary<SoundType, AudioClip>();
        foreach (var sound in soundList)
            if (!soundDictionary.ContainsKey(sound.type))
                soundDictionary.Add(sound.type, sound.clip);

        StartBackgroundSounds();
        SetupAngerSounds();
    }

    private void StartBackgroundSounds()
    {
        if (soundDictionary.TryGetValue(SoundType.BackgroundMusic, out AudioClip music))
        {
            musicSource.clip = music;
            musicSource.Play();
        }

        if (soundDictionary.TryGetValue(SoundType.OfficeAmbient, out AudioClip ambient))
        {
            ambientSource.clip = ambient;
            ambientSource.Play();
        }
    }

    public void PlaySFX(SoundType type)
    {
        if (soundDictionary.TryGetValue(type, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Звук {type} не найден в AudioManager");
        }
    }

    public void PlayVoice()
    {
        if (soundDictionary.TryGetValue(SoundType.NPCVoice, out AudioClip clip))
        {
            voiceSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Звук {SoundType.NPCVoice} не найден в AudioManager");
        }
    }

    public void StartMinigameTick()
    {
        if (soundDictionary.TryGetValue(SoundType.MinigameTick, out AudioClip tick))
        {
            if (minigameTickSource.clip != tick)
            {
                minigameTickSource.clip = tick;
            }

            if (!minigameTickSource.isPlaying)
            {
                minigameTickSource.Play();
            }
        }
    }

    public void StopMinigameTick()
    {
        if (minigameTickSource != null && minigameTickSource.isPlaying)
        {
            minigameTickSource.Stop();
        }
    }

    public float MusicVolume => musicVolume;
    public float SfxVolume => sfxVolume;
    public float VoiceVolume => voiceVolume;

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("Audio.MusicVolume", musicVolume);
        ApplyVolumeSettings();
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("Audio.SfxVolume", sfxVolume);
        ApplyVolumeSettings();
    }

    public void SetVoiceVolume(float value)
    {
        voiceVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("Audio.VoiceVolume", voiceVolume);
        ApplyVolumeSettings();
    }

    private void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("Audio.MusicVolume", musicVolume);
        sfxVolume = PlayerPrefs.GetFloat("Audio.SfxVolume", sfxVolume);
        voiceVolume = PlayerPrefs.GetFloat("Audio.VoiceVolume", voiceVolume);
    }

    private void ApplyVolumeSettings()
    {
        if (musicSource != null) musicSource.volume = musicVolume;
        if (ambientSource != null) ambientSource.volume = ambientVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
        if (voiceSource != null) voiceSource.volume = voiceVolume;
        if (minigameTickSource != null) minigameTickSource.volume = minigameTickVolume * sfxVolume;
        if (heartbeatSource != null) heartbeatSource.volume = angerHeartbeatVolume * sfxVolume;
        if (rumbleSource != null) rumbleSource.volume = angerRumbleVolume * sfxVolume;
    }

    private void SetupAngerSounds()
    {
        if (soundDictionary.TryGetValue(SoundType.Heartbeat, out AudioClip heart))
        {
            heartbeatSource.clip = heart;
            heartbeatSource.Play();
        }
        if (soundDictionary.TryGetValue(SoundType.LowRumble, out AudioClip rumble))
        {
            rumbleSource.clip = rumble;
            rumbleSource.Play();
        }
    }

    private void OnEnable()
    {
        StatsManager.OnAngerChanged += SetAngerEffects;
    }

    private void OnDisable()
    {
        StatsManager.OnAngerChanged -= SetAngerEffects;
    }

    public void SetAngerEffects(float current, float max)
    {
        if (max <= 0) return;
        float t = current / max;
        float smoothProgress = t * t * (3f - 2f * t);

        angerHeartbeatVolume = Mathf.Lerp(0f, 0.85f, smoothProgress);
        angerRumbleVolume = Mathf.Lerp(0f, 0.65f, smoothProgress);
        ApplyVolumeSettings();

        float cutoffValue = Mathf.Lerp(22000f, 650f, smoothProgress);
        if (mainMixer != null)
        {
            mainMixer.SetFloat("AmbientCutoff", cutoffValue);
        }
        musicSource.pitch = Mathf.Lerp(1.0f, 1.15f, smoothProgress);
    }
    public void PauseAllSounds()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }

    public void ResumeAllSounds()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.UnPause();
        }
    }
    public void StopAllSounds()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.Stop();
        }
    }
}
