using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    #region Inspector Fields
    [Header("General Settings")]
    [SerializeField] private int _poolSize = 10;
    [SerializeField] private bool _autoExpandPoolIfNeeded = true;
    [SerializeField] private bool _showWarnings = true;

    [Header("Volume Settings")]
    [Range(0, 1)] [SerializeField] private float _masterVolume = 1;
    [Range(0, 1)] [SerializeField] private float _sfxVolume = 1;
    [Range(0, 1)] [SerializeField] private float _musicVolume = 1;

    [Header("Pitch Settings")]
    [Range(0.1f, 3f)] [SerializeField] private float _masterPitch = 1f;

    [Header("Music Source")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioClip _defaultMusic;
    #endregion

    #region Private Fields
    private List<AudioSource> _sfxPool;
    private Transform _poolParent;
    private Dictionary<AudioSource,Transform> activeFollowSources = new Dictionary<AudioSource, Transform>();
    #endregion

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializePool();
        UpdateAudioProperties();
    }

    private void Start()
    {
        if (_defaultMusic != null)
            PlayMusic(_defaultMusic, loop: true, fadeDuration: 5f);
    }

    private void Update()
    {
        for(int i = 0; i < activeFollowSources.Count; i++)
        {
            var source = activeFollowSources.ElementAt(i);
            if (!source.Key.isPlaying || source.Key.clip == null || source.Value == null)
            {
                activeFollowSources.Remove(source.Key);
                continue;
            }
            source.Key.transform.position = source.Value.position;
        }
        
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    #endregion

    #region SFX Methods

    /// <summary>
    /// Plays a clip using default parameters.
    /// </summary>
    public void PlayClipSimple(AudioClip clip)
    {
        SetClip(clip)?.Play();
    }

    /// <summary>
    /// Plays a clip using a randomized melodic pitch.
    /// </summary>
    public void PlayClipSimpleMelodic(AudioClip clip)
    {
        var handle = SetClip(clip);
        handle?.RandomizeMelodicPitch().Play();
    }

    /// <summary>
    /// Returns an AudioHandle for chaining further options.
    /// </summary>
    public AudioHandle SetClip(AudioClip clip)
    {
        return new AudioHandle(this, clip);
    }

    #endregion

    #region Music Methods

    /// <summary>
    /// Plays music with optional looping and fade-in.
    /// </summary>
    public void PlayMusic(AudioClip clip, bool loop = true, float fadeDuration = 0f)
    {
        if (_musicSource == null)
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
        }

        _musicSource.clip = clip;
        _musicSource.loop = loop;
        _musicSource.pitch = _masterPitch;

        if (fadeDuration > 0)
        {
            FadeInSource(_musicSource, fadeDuration, _masterVolume * _musicVolume);
        }
        else
        {
            _musicSource.volume = _masterVolume * _musicVolume;
            _musicSource.Play();
        }
    }

    /// <summary>
    /// Stops music playback, with an optional fade-out.
    /// </summary>
    public void StopMusic(float fadeDuration = 0f)
    {
        if (_musicSource == null || !_musicSource.isPlaying)
            return;

        if (fadeDuration > 0)
            FadeOutSource(_musicSource, fadeDuration);
        else
            _musicSource.Stop();
    }

    /// <summary>
    /// Applies a slow-motion effect by tweening the pitch.
    /// </summary>
    public void ApplySlowMoEffect(float factor)
    {
        float newPitch = Mathf.Clamp01(factor * 2f + 0.15f);
        float duration = 0.35f;
        foreach (var source in GetPlayingSources())
        {
            AudioUtility.TweenPitch(source, newPitch, duration, this);
        }
        if (_musicSource != null && _musicSource.isPlaying)
            AudioUtility.TweenPitch(_musicSource, newPitch, duration, this);
    }

    /// <summary>
    /// Resets the pitch to the master pitch value.
    /// </summary>
    public void ResetPitch()
    {
        float duration = 0.25f;
        foreach (var source in GetPlayingSources())
        {
            AudioUtility.TweenPitch(source, _masterPitch, duration, this);
        }
        if (_musicSource != null && _musicSource.isPlaying)
            AudioUtility.TweenPitch(_musicSource, _masterPitch, duration, this);
    }

    #endregion

    #region Internal Audio Playback

    /// <summary>
    /// Plays an audio clip with various parameters such as volume scaling, pitch, fade duration, looping, and spatial position.
    /// </summary>
    internal void PlayAudioInternal(AudioClip clip, float volumeScale, float pitch, float fadeDuration, bool loop,Vector3 position,float spatialBlend,Transform followTransform,float dopperLevel)
    {
        if (clip == null)
            return;

        AudioSource source = GetPooledSource();
        if (source == null)
        {
            if (_autoExpandPoolIfNeeded)
            {
                source = CreatePooledSource();
                _sfxPool.Add(source);
            }
            else
            {
                Debug.LogError("No available audio sources in the pool, consider expanding the pool size or enabling auto-expansion.");
                return;
            }
        }

        float finalVolume = _masterVolume * _sfxVolume * volumeScale;
        float finalPitch = Mathf.Abs(pitch - 1f) > 0.01f ? pitch : _masterPitch;

        source.clip = clip;
        source.loop = loop;
        source.pitch = finalPitch;

        if (followTransform != null)
        {
            source.transform.position = followTransform.position;
            if (spatialBlend == 0)
            {
                AudioUtility.ShowWarning("Audio Follow transform is set but spatial blend is 2D. To use 3D, add SetSpatialBlend() with value > 0", _showWarnings);
            }
            else if (spatialBlend == 1 && followTransform is RectTransform)
            {
                AudioUtility.ShowWarning("For UI elements follow,it is recommended to use spacial blend with value less than 1", _showWarnings);
            }
            if(followTransform is RectTransform) dopperLevel = 0; // disable doppler effect for UI elements
            activeFollowSources.Add(source,followTransform);
        }
        if (position != Vector3.zero)
        {
            source.transform.position = position;
            if (spatialBlend == 0)
            {
                AudioUtility.ShowWarning("Audio Position is set but spatial blend is 2D. To use 3D, add SetSpatialBlend() with value > 0", _showWarnings);
            }
        }
        else
        {
            source.transform.position = transform.position;
            source.spatialBlend = spatialBlend;
        }
        source.dopplerLevel = dopperLevel;
        
        

        if (fadeDuration > 0)
            FadeInSource(source, fadeDuration, finalVolume);
        else
        {
            source.volume = finalVolume;
            source.Play();
        }
    }

    /// <summary>
    /// Fades in an AudioSource
    /// </summary>
    private void FadeInSource(AudioSource source, float duration, float targetVolume)
    {
        source.volume = 0;
        source.Play();
        AudioUtility.FadeIn(source, duration, targetVolume, this);
    }

    /// <summary>
    /// Fades out an AudioSource
    /// </summary>
    private void FadeOutSource(AudioSource source, float duration)
    {
        AudioUtility.FadeOut(source, duration, this);
    }

    #endregion

    #region Volume & Pool Management

    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
        UpdateAudioProperties();
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
        UpdateAudioProperties();
    }

    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        UpdateAudioProperties();
    }

    /// <summary>
    /// Updates the volume and pitch for both music and active SFX sources.
    /// </summary>
    private void UpdateAudioProperties()
    {
        if (_musicSource)
        {
            _musicSource.volume = _masterVolume * _musicVolume;
            _musicSource.pitch = _masterPitch;
        }

        if (_sfxPool == null)
            return;

        foreach (var source in _sfxPool)
        {
            if (source.isPlaying)
            {
                float currentVolumeScale = source.clip ? (source.volume / (_masterVolume * _sfxVolume)) : 1f;
                source.volume = _masterVolume * _sfxVolume * currentVolumeScale;
                source.pitch = _masterPitch;
            }
        }
    }

    /// <summary>
    /// Creates and initializes the audio source pool.
    /// </summary>
    private void InitializePool()
    {
        _poolParent = new GameObject("SFX Pool").transform;
        _poolParent.SetParent(transform);
        _sfxPool = new List<AudioSource>();

        for (int i = 0; i < _poolSize; i++)
            _sfxPool.Add(CreatePooledSource());
    }

    /// <summary>
    /// Creates a single AudioSource for the SFX pool.
    /// </summary>
    private AudioSource CreatePooledSource()
    {
        GameObject sourceGO = new GameObject("SFX_Source");
        sourceGO.transform.SetParent(_poolParent);
        AudioSource audioSource = sourceGO.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        return audioSource;
    }

    /// <summary>
    /// Returns an available AudioSource from the pool.
    /// </summary>
    private AudioSource GetPooledSource()
    {
        foreach (var source in _sfxPool)
        {
            if (!source.isPlaying)
                return source;
        }
        return null;
    }

    /// <summary>
    /// Returns a list of currently playing SFX sources.
    /// </summary>
    private List<AudioSource> GetPlayingSources()
    {
        List<AudioSource> playingSources = new List<AudioSource>();
        foreach (var source in _sfxPool)
        {
            if (source.isPlaying)
                playingSources.Add(source);
        }
        return playingSources;
    }

    #endregion
}
