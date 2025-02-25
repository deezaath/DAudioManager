using UnityEngine;

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
    
    public enum ClipMode { Random }
    
}
