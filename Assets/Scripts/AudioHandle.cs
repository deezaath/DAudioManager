using UnityEngine;
using System;

/// <summary>
///  Class to handle chaining audio settings for AudioManager
/// </summary>
public class AudioHandle
{
    private AudioManager _manager;
    
    private AudioClip _clip;
    private float _volumeScale = 1f;
    private float _pitch = 1f;
    private bool _loop = false;
    
    private float _fadeDuration = 0f;
    private float _delay = 0f;
    private bool _usePosition = false;
    private Vector3 _position;
    
    
    public AudioHandle(AudioManager manager, AudioClip clip)
    {
        _manager = manager;
        _clip = clip;
    }
    
    #region Chainable Methods
    
    public AudioHandle SetVolume(float volumeScale)
    {
        _volumeScale = volumeScale;
        return this;
    }

    public AudioHandle SetPitch(float pitch)
    {
        _pitch = pitch;
        return this;
    }
    public AudioHandle RandomizePitch(float min = 0.9f, float max = 1.2f)
    {
        _pitch = UnityEngine.Random.Range(min, max);
        return this;
    }

    public AudioHandle SetLoop(bool loop)
    {
        _loop = loop;
        return this;
    }
    
    public AudioHandle SetFade(float fadeDuration)
    {
        _fadeDuration = fadeDuration;
        return this;
    }

    public AudioHandle SetDelay(float delay)
    {
        _delay = delay;
        return this;
    }

    public AudioHandle AtPosition(Vector3 position)
    {
        _usePosition = true;
        _position = position;
        return this;
    }
    public AudioHandle RandomizeMelodicPitch()
    {
        int semitone = MelodyGenerator.GetNextNote();
        
        _pitch = Mathf.Pow(2f, semitone / 12f);
        return this;
    }
    #endregion
    
    public void Play()
    {
        if (_delay > 0f)
        {
            AudioUtility.DelayedCall(_delay, () =>
            {
                _manager.PlayAudioInternal(_clip,_volumeScale,_pitch,_fadeDuration,_loop,_usePosition,_position);
            },_manager);
        }
        else
        {
            _manager.PlayAudioInternal(_clip, _volumeScale, _pitch, _fadeDuration, _loop, _usePosition, _position);
        }
    }
}
