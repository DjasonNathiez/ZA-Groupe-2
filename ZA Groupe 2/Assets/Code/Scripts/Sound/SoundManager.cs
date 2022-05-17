using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public Sound[] soundList;
    public List<AudioSource> sources;

    IEnumerator WaitUntilEndClip(AudioSource source)
    {
        yield return new WaitUntil(() => source.isPlaying);
        yield return new WaitWhile(() => source.isPlaying);
        sources.Remove(source);
        Destroy(source);
    }

    private void Start()
    {
        PlaySound("Hit");
        PlaySound("Hit");
    }

    private void Update()
    {
        // if (sources.Count != 0)
        // {
        //     foreach (AudioSource s in sources)
        //     {
        //         if (!s.isPlaying)
        //         {
        //             sources.Remove(s);
        //             Destroy(s);
        //         }
        //     }
        // }
    }

    public void PlaySound(string soundName)
    {
        
        foreach (Sound s in soundList)
        {
            if (s.soundName == soundName)
            {
                
                AudioSource source = gameObject.AddComponent<AudioSource>();

                sources.Add(source); //Add this to list;
                
                source.clip = s.clip;
                source.volume = s.volume;
                source.pitch = s.pitch;
                source.loop = s.loop;
                source.outputAudioMixerGroup = s.mixerGroup;
    
                source.Play();
                Debug.Log(source + "Instantiate");
                
                StartCoroutine(WaitUntilEndClip(source));
            }
        }
    }

    public void StopSound(string soundName)
    {
        foreach (Sound sound in soundList)
        {
            if (sound.soundName == soundName)
            {
                foreach (AudioSource source in sources)
                {
                    if (source.clip == sound.clip)
                    {
                        source.Stop();
                        sources.Remove(source);
                        Destroy(source);
                    }
                }
            }
        }
        
    }
    
}

[Serializable] public class Sound
{
    public string soundName;
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;
    [Range(0,1)] public float volume;
    [Range(0,1)] public float pitch;
    public bool loop;
    public bool unique;
}
