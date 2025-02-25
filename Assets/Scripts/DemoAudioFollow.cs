using System;
using UnityEngine;

public class DemoAudioFollow : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private Transform followTransform;

    private void Start()
    {
        AudioManager.Instance.SetClip(clip).FollowTransform(followTransform).SetLoop(true).SetSpacialBlend(1).Play();
    }
}
