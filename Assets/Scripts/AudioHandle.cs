using UnityEngine;
using System;

/// <summary>
///  Class to handle chaining audio settings for AudioManager
/// </summary>
public class AudioHandle
{
    private AudioManager manager;
    
    private AudioClip clip;
    private float volumeScale = 1f;
    private float pitch = 1f;
    private bool useLoop = false;
    
    private float fadeDuration = 0f;
    private float delay = 0f;
    
    private Vector3 position;
    
    private float spacialBlend = 0f;
    private Transform followTransform = null;
    
    private float dopplerLevel = 0f;
    
    private AudioEffect effect = AudioEffect.None;
    
    
    public AudioHandle(AudioManager manager, AudioClip clip)
    {
        this.manager = manager;
        this.clip = clip;
    }
    
    #region Chainable Methods
    
    /// <summary>
    ///  Set the volume of the audio source
    /// </summary>
    public AudioHandle SetVolume(float volumeScale)
    {
        this.volumeScale = volumeScale;
        return this;
    }

    /// <summary>
    ///  Set the pitch of the audio source
    /// </summary>
    public AudioHandle SetPitch(float pitch)
    {
        this.pitch = pitch;
        return this;
    }
    
    /// <summary>
    ///  Set the audio effect for the audio source
    /// </summary>
    public AudioHandle SetEffect(AudioEffect effect)
    {
        this.effect = effect;
        return this;
    }
    
    /// <summary>
    ///  Randomizes the pitch of the audio source within a range
    /// </summary>
    public AudioHandle RandomizePitch(float min = 0.9f, float max = 1.2f)
    {
        pitch = UnityEngine.Random.Range(min, max);
        return this;
    }
    
    /// <summary>
    ///  Spacial Blend changes the 3D effect of the audio source (0 - 1)
    /// </summary>
    public AudioHandle SetSpacialBlend(float spacialBlend = 1)
    {
        this.spacialBlend = Mathf.Clamp(spacialBlend, 0, 1);
        return this;
    }
    
    /// <summary>
    /// Doppler Level changes the pitch based on the velocity of audio source (0 - 5)
    /// Applies only to 3D audio sources (SpacialBlend > 0)
    /// </summary>
    public AudioHandle SetDopplerLevel(float dopplerLevel = 0)
    {
        this.dopplerLevel = Mathf.Clamp(dopplerLevel, 0, 5);
        return this;
    }
    
    /// <summary>
    ///  Follows the transform in 3D space.
    /// </summary>
    public AudioHandle FollowTransform(Transform followTransform)
    {
        this.followTransform = followTransform;
        return this;
    }

    /// <summary>
    ///  Set if the audio should loop
    /// </summary>
    public AudioHandle SetLoop(bool loop)
    {
        useLoop = loop;
        return this;
    }
    
    /// <summary>
    /// Set the fade duration of the audio source (default is 0)
    /// </summary>
    public AudioHandle SetFade(float fadeDuration)
    {
        this.fadeDuration = fadeDuration;
        return this;
    }

    /// <summary>
    /// Set the delay before playing the audio source (default is 0)
    /// </summary>
    public AudioHandle SetDelay(float delay)
    {
        this.delay = delay;
        return this;
    }

    /// <summary>
    ///  Set the position of the audio source in 3D space
    /// </summary>
    public AudioHandle AtPosition(Vector3 position)
    {
        this.position = position;
        return this;
    }
    
    /// <summary>
    ///  Randomizes the pitch of the audio source using the MelodyGenerator
    /// </summary>
    public AudioHandle RandomizeMelodicPitch()
    {
        int semitone = MelodyGenerator.GetNextNote();
        
        pitch = Mathf.Pow(2f, semitone / 12f);
        return this;
    }
    
    #endregion
    
    public void Play()
    {
        if (delay > 0f)
        {
            AudioUtility.DelayedCall(delay, () =>
            {
                manager.PlayAudioInternal(clip,volumeScale,pitch,fadeDuration,useLoop,position,spacialBlend,followTransform,dopplerLevel,effect);
            },manager);
        }
        else
        {
            manager.PlayAudioInternal(clip, volumeScale, pitch, fadeDuration, useLoop,position, spacialBlend, followTransform,dopplerLevel,effect);
        }
    }
}
public enum AudioEffect { None, Muffled, Robot,Echo,Cave,Chorus}
