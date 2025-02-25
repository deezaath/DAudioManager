using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "AudioPreset", menuName = "DAudioManager/Audio Preset")]
public class AudioPreset : ScriptableObject
{
    [Tooltip("Determines which clip from array will be played")]
    public ClipMode Mode = ClipMode.Random;
    
    public AudioClip[] Clips;
    
    [Header("Preset Settings")]
    [Range(0,1)] public float VolumeScale = 1f;
    public bool Looped = false;
    public float FadeDuration = 0f;
    public float Delay = 0f;
    
    [Header("Pitch Settings")]
    public float Pitch = 1f;
    
    [Space(5)]
    
    public bool RandomizePitch = false;
    public float MinPitch = 0.9f;
    public float MaxPitch = 1.2f;
    
    [Space(5)]
    
    [Tooltip("Melodic pitch will use MelodyGenerator to determine pitch")]
    public bool MelodicPitch = false;
    
    
    [HideInInspector] public AudioClip LastClipPlayed = null;
    public enum ClipMode { Random, Sequential, RandomNoRepeat }

    public AudioClip GetNextClip()
    {
        switch (Mode)
        {
            case ClipMode.Random:
                return GetClip(Clips[Random.Range(0, Clips.Length)]);
            case ClipMode.Sequential:
                return GetClip(Clips[(Array.IndexOf(Clips, LastClipPlayed) + 1) % Clips.Length]);
            case ClipMode.RandomNoRepeat:
                if(!LastClipPlayed) return GetClip(Clips[Random.Range(0, Clips.Length)]);
                List<AudioClip> clips = Clips.ToList();
                clips.Remove(LastClipPlayed);
                return GetClip(clips[Random.Range(0, clips.Count)]);
        }
        return null;
    }

    private AudioClip GetClip(AudioClip clip)
    {
        LastClipPlayed = clip;
        return clip;
    }
    
}
