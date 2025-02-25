using System;
using UnityEngine;

public class DemoPreset : MonoBehaviour
{
    [SerializeField] private AudioPreset preset;

    private void Start()
    {
        // Simple preset usage
       // AudioManager.Instance.Play(preset);
        
        
        // Customizing audio settings
        
        // var audio = AudioManager.Instance.SetPreset(preset);
        // audio.FollowTransform(transform);
        // audio.SetDopplerLevel(1);
        // audio.Play();
        
            // OR 
       // AudioManager.Instance.SetPreset(preset).FollowTransform(transform).SetDopplerLevel(1).Play();
       
       
    }
}
