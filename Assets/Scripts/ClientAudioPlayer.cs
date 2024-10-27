using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientAudioPlayer : Singleton<ClientAudioPlayer>
{
    [SerializeField] private AudioClip _audioClip;
    private AudioSource _audioSource;
    
    public override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudioClip()
    {
        _audioSource.clip = _audioClip;
        _audioSource.Play();
    }
}
