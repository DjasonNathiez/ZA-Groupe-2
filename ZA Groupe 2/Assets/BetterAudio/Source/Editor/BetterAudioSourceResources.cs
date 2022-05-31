using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BetterAudio
{
    //[CreateAssetMenu(fileName = "Resources", menuName = "")]
    public partial class BetterAudioSourceResources : ScriptableObject
    {
        public Texture BetterAudioSourceLogo;
        public Texture BetterAudioSourceIcon;

        public Texture Light;
        [Header("Source icons")]
        public Texture IconTools;
        public Texture IconStart;
        public Texture IconPlay;
        public  Texture IconUpdate;
        public  Texture IconEnd;

        [Header("Main themes")]
        public BetterAudioStyle DarkThemeBackground;
        public BetterAudioStyle DarkThemeForeground;

        public BetterAudioStyle LightThemeBackground;
        public BetterAudioStyle LightThemeForeground;

        public BetterAudioStyle SliderBackground;
        public BetterAudioStyle SliderThumb;

        [Header("Source themes")]
        public BetterAudioStyle StartBackground;
        public BetterAudioStyle StartForeground;

        public BetterAudioStyle PlayBackground;
        public BetterAudioStyle PlayForeground;

        public BetterAudioStyle UpdateBackground;
        public BetterAudioStyle UpdateForeground;

        public BetterAudioStyle EndBackground;
        public BetterAudioStyle EndForeground;

    }
}