using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAtEvent : MonoBehaviour
{
    public void PlaySound(string soundName)
    {
        AudioManager.instance.PlayEnvironment(soundName);
    }
}
