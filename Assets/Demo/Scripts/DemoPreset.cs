using System;
using UnityEngine;

public class DemoPreset : MonoBehaviour
{
    [SerializeField] private AudioPreset preset;

    private void Start()
    {
        AudioManager.Instance.Play(preset);
    }
}
