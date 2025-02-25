using System;
using System.Collections;
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
    
    public static void ShowWarning(string message,bool warningsEnabled)
    {
        if (warningsEnabled)
        {
            Debug.LogWarning($"<color=#FFF288><b>[DAudioManager]</b></color> <b>{message}</b>");
        }
    }
}
