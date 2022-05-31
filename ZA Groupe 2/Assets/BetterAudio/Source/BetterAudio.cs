using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterAudio
{
    public static partial class BetterAudio
    {

        static Behaviour Fade = new Behaviour("Fade", typeof(Fade));
        static Behaviour FadeAt = new Behaviour("Fade At", typeof(FadeAt));
        static Behaviour RandomClip = new Behaviour("Random Clip", typeof(RandomClip));
        static Behaviour Delay = new Behaviour("Delay", typeof(Delay));
        static Behaviour SourceCurves = new Behaviour("Set source property (curve)", typeof(SourceCurves));
        static Behaviour SetSource = new Behaviour("Set source property (float)", typeof(SetSourceProperty));
        static Behaviour Loop = new Behaviour("Loop x times", typeof(Loop));
        static Behaviour AutoDestroy = new Behaviour("Auto Destroy", typeof(AutoDestroy));
        static Behaviour DestroyAt = new Behaviour("Destroy At", typeof(DestroyAt));
        static Behaviour ClipSerie = new Behaviour("Clip Serie", typeof(ClipSerie));
        static Behaviour Follower = new Behaviour("Follow", typeof(Follow));
        static Behaviour DistanceFade = new Behaviour("Distance Fade", typeof(DistanceFade));

        public static Behaviour[] startBehaviours = { AutoDestroy,  Delay, Follower,  RandomClip, SetSource};
        public static Behaviour[] playBehaviours = { AutoDestroy, ClipSerie, Delay, Fade, Follower, RandomClip, SetSource };
        public static Behaviour[] updateBehaviours = { DestroyAt, DistanceFade, FadeAt, Follower, SourceCurves};
        public static Behaviour[] endBehaviours = { AutoDestroy, ClipSerie, Delay, Follower,Loop, RandomClip, SetSource };

        public static string SecondsFormat(float secondes)
        {


            int min = Mathf.FloorToInt(secondes / 60f);
            secondes = secondes % 60;

            secondes = Mathf.RoundToInt(secondes);

            int hours = Mathf.FloorToInt(min / 60f);
            min = min % 60;

            string m = min < 10 ? "0" + min : min.ToString();
            string s = secondes < 10 ? "0" + secondes : secondes.ToString();


            if (hours > 0)
                return hours + ":" + m + ":" + s;
            else
                return m + ":" + s;

        }

    }

    public enum Categories
    {
        All = 1 << Start | Play | Update | End | Collider | Event,
        Start = 1 << 0,
        Play = 1 << 1,
        Update = 1 << 2,
        End = 1 << 3,
        Collider = 1 << 4,
        Event = 1 << 5
    }

    public class Behaviour
    {
        public string Name;
        public System.Type type;

        public Behaviour(string n, System.Type t)
        {
            Name = n;
            type = t;
        }
    }
}