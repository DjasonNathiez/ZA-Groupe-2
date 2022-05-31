﻿using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class SmConfigurator : MonoBehaviour
{
    [FormerlySerializedAs("effectAudioMixer")]
    [SerializeField]
    private AudioMixerGroup _effectAudioMixer = null;

    [FormerlySerializedAs("fxAudioMixer")]
    [SerializeField]
    private AudioMixerGroup _fxAudioMixer = null;

    [FormerlySerializedAs("mainAudioMixer")]
    [SerializeField]
    private AudioMixerGroup _mainAudioMixer = null;

    [FormerlySerializedAs("musicAudioMixer")]
    [SerializeField]
    private AudioMixerGroup _musicAudioMixer = null;

    [FormerlySerializedAs("soundLevel")]
    [SerializeField]
    [Range(0, 1f)]
    private float _soundLevel = 1f;

    private void Start()
    {
        SoundManager.MainVolume = _soundLevel;

        if (_mainAudioMixer == null) SoundManager.AudioMixer = _mainAudioMixer;

        if (_effectAudioMixer == null) SoundManager.EffectAudioMixer = _effectAudioMixer;

        if (_fxAudioMixer == null) SoundManager.FxAudioMixer = _fxAudioMixer;

        if (_musicAudioMixer == null) SoundManager.MusicAudioMixer = _musicAudioMixer;
    }
}