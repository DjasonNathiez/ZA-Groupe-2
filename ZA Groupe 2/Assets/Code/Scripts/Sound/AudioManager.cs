using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    public Sounds[] playerSounds;
    public Sounds[] lionSounds;
    public Sounds[] rabbitSounds;
    public Sounds[] bearSounds;
    public Sounds[] bossSounds;
    public Sounds[] environment;
    public Sounds[] musics;

    public bool secretSoundActivated;
    public GameObject secretImageUI;
    
    [Serializable]public struct Sounds
    {
        public string soundName;
        public AudioClip clip;
        public bool loop;
        [Range(0,1)] public float volume;
    }

    private void Awake()
    {
        instance = this;

        secretSoundActivated = false;
    }

    public void SetMusic(string musicName)
    {
        foreach (Sounds s in musics)
        {
            if (s.soundName == musicName)
            {
                SoundManager.PlayMusic(s.clip, s.volume, s.loop);
            }
        }
    }

    public void PlayEnvironment(string enviroName)
    {
        foreach (Sounds s in environment)
        {
            if (s.soundName == enviroName)
            {
                SoundManager.PlayEffect(s.clip, s.volume, s.loop);
            }
        }
    }
}
