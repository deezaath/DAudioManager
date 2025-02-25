using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioUtility
{
    /// <summary>
    /// Fades in the given AudioSource from volume 0 to targetVolume over fadeDuration (in seconds).
    /// </summary>
    public static void FadeIn(AudioSource audioSource, float fadeDuration, float targetVolume, MonoBehaviour runner)
    {
        runner.StartCoroutine(FadeInCoroutine(audioSource, fadeDuration, targetVolume));
    }

    private static IEnumerator FadeInCoroutine(AudioSource audioSource, float fadeDuration, float targetVolume)
    {
        audioSource.volume = 0f;
        audioSource.Play();
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / fadeDuration);
            yield return null;
        }
        audioSource.volume = targetVolume;
    }

    /// <summary>
    /// Fades out the AudioSource from its current volume to 0 over fadeDuration (in seconds) and then stops it.
    /// </summary>
    public static void FadeOut(AudioSource audioSource, float fadeDuration, MonoBehaviour runner)
    {
        runner.StartCoroutine(FadeOutCoroutine(audioSource, fadeDuration));
    }

    private static IEnumerator FadeOutCoroutine(AudioSource audioSource, float fadeDuration)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Stop();
    }

    /// <summary>
    /// Invokes an action after a delay.
    /// </summary>
    public static void DelayedCall(float delay, Action action, MonoBehaviour runner)
    {
        runner.StartCoroutine(DelayedCallCoroutine(delay, action));
    }

    private static IEnumerator DelayedCallCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    /// <summary>
    /// Tweens the pitch of an AudioSource to the targetPitch over duration (in seconds).
    /// </summary>
    public static void TweenPitch(AudioSource audioSource, float targetPitch, float duration, MonoBehaviour runner)
    {
        runner.StartCoroutine(TweenPitchCoroutine(audioSource, targetPitch, duration));
    }
    
    /// <summary>
    /// Tweens the volume of an AudioSource to the targetVolume over duration (in seconds).
    /// </summary>
    public static void TweenVolume(AudioSource audioSource, float targetVolume, float duration, MonoBehaviour runner)
    {
        runner.StartCoroutine(TweenVolumeCoroutine(audioSource, targetVolume, duration));
    }
    public static void ApplyAudioFilter(AudioSource source, AudioEffect effect)
    {
        switch (effect)
        {
            case AudioEffect.None:
                break;
            
            case AudioEffect.Muffled:
                AudioLowPassFilter lowPass = source.GetComponent<AudioLowPassFilter>();
                if (lowPass == null)
                    lowPass = source.gameObject.AddComponent<AudioLowPassFilter>();
                
                lowPass.enabled = true;
                lowPass.cutoffFrequency = 800f; 
                break;

            case AudioEffect.Robot:
                AudioDistortionFilter distortion = source.GetComponent<AudioDistortionFilter>();
                if (distortion == null)
                    distortion = source.gameObject.AddComponent<AudioDistortionFilter>();
                
                distortion.enabled = true;
                distortion.distortionLevel = 0.5f;
                break;
            
            case AudioEffect.Echo:
                AudioEchoFilter echo = source.GetComponent<AudioEchoFilter>();
                if (echo == null)
                    echo = source.gameObject.AddComponent<AudioEchoFilter>();
                
                echo.enabled = true;
                echo.delay = 500f;  
                echo.decayRatio = 0.5f;
                echo.wetMix = 1f;
                echo.dryMix = 1f;
                break;
            
            case AudioEffect.Cave:
                AudioReverbFilter reverb = source.GetComponent<AudioReverbFilter>();
                if (reverb == null)
                    reverb = source.gameObject.AddComponent<AudioReverbFilter>();
                
                reverb.enabled = true;
                reverb.reverbPreset = AudioReverbPreset.Cave;
                break;
            
            case AudioEffect.Chorus:
                AudioChorusFilter chorus = source.GetComponent<AudioChorusFilter>();
                if (chorus == null)
                    chorus = source.gameObject.AddComponent<AudioChorusFilter>();
                
                chorus.enabled = true;
                chorus.delay = 40f;
                chorus.rate = 0.8f;
                chorus.depth = 0.7f;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(effect), effect, null);
        }
    }
    public static void RemoveAudioFilters(AudioSource source)
    {
        AudioLowPassFilter lowPass = source.GetComponent<AudioLowPassFilter>();
        if (lowPass != null)
            lowPass.enabled = false;
    
        AudioReverbFilter reverb = source.GetComponent<AudioReverbFilter>();
        if (reverb != null)
            reverb.enabled = false;
    
        AudioDistortionFilter distortion = source.GetComponent<AudioDistortionFilter>();
        if (distortion != null)
            distortion.enabled = false;
    
        AudioEchoFilter echo = source.GetComponent<AudioEchoFilter>();
        if (echo != null)
            echo.enabled = false;
    
        AudioChorusFilter chorus = source.GetComponent<AudioChorusFilter>();
        if (chorus != null)
            chorus.enabled = false;
    }

    private static IEnumerator TweenPitchCoroutine(AudioSource audioSource, float targetPitch, float duration)
    {
        float startPitch = audioSource.pitch;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.pitch = Mathf.Lerp(startPitch, targetPitch, elapsed / duration);
            yield return null;
        }
        audioSource.pitch = targetPitch;
    }
    private static IEnumerator TweenVolumeCoroutine(AudioSource audioSource, float targetVolume, float duration)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }
        audioSource.volume = targetVolume;
    }
    public static void ShowWarning(string message,bool warningsEnabled)
    {
        if (warningsEnabled)
        {
            Debug.LogWarning($"<color=#FFF288><b>[DAudioManager]</b></color> <b>{message}</b>");
        }
    }
}
