using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngineObject = UnityEngine.Object;

#pragma warning disable 4014

public class SoundManager
{
    public enum SoundManagerType
    {
        Effect,
        Fx,
        Music,
        OneTime
    }

    public static AudioMixerGroup AudioMixer;

    public static AudioMixerGroup EffectAudioMixer;
    public static AudioMixerGroup FxAudioMixer;
    public static AudioMixerGroup MusicAudioMixer;

    private static readonly Dictionary<int, SoundManagerAudio> EffectAudioCache;
    private static readonly Dictionary<int, SoundManagerAudio> FxAudioCache;
    private static readonly Dictionary<int, SoundManagerAudio> MusicAudioCache;
    private static readonly GameObject GameObject;

    private static SoundManagerAudio _currentMusic;
    private static float _musicTransitionTime;
    private static bool _advancePauseCheck;

    static SoundManager()
    {
        EffectAudioCache = new Dictionary<int, SoundManagerAudio>();
        FxAudioCache = new Dictionary<int, SoundManagerAudio>();
        MusicAudioCache = new Dictionary<int, SoundManagerAudio>();

        GameObject = new GameObject("SoundManager");
        UnityEngineObject.DontDestroyOnLoad(GameObject);
    }

    /// <summary>
    ///     Music transition time
    /// </summary>
    private static float MusicTransitionTime
    {
        get => _musicTransitionTime;
        set => _musicTransitionTime = Mathf.Max(value, 0.0f);
    }

    /// <summary>
    ///     SoundManager audio object
    /// </summary>
    public class SoundManagerAudio
    {
        private readonly AudioClip _audioClip;
        private readonly bool _doNotDestroy;
        private readonly float _duration;
        private readonly Func<float> _mainVolumeGroup;
        private readonly AudioMixerGroup _mixer;
        private readonly SoundManagerType _type;

        private AudioSource _audioSource;

        public AudioClip AudioClip => _audioClip;
        public bool DoNotDestroy => _doNotDestroy;
        public float Duration => _duration;

        private bool StopExist => _audioSource == null || Application.isPlaying == false;

        private bool _loop;
        private float _volume;

        private CancellationTokenSource _playingToken;
        private CancellationTokenSource _transitionToken;

        /// <summary>
        /// SoundManagerAudio constructor
        /// </summary>
        /// <param name="audioClip">The audio clip to use</param>
        /// <param name="volume">The volume of the sound to play (between 0 and 1)</param>
        /// <param name="loop">Loop the clip</param>
        /// <param name="doNotDestroy">Keep the AudioSource object after playing it</param>
        /// <param name="type">The type of the SoundManagerAudio</param>
        /// <param name="mixer">The sound mixer to use for the clip</param>
        /// <exception cref="ArgumentOutOfRangeException">Error thrown if the type is unknown</exception>
        internal SoundManagerAudio(AudioClip audioClip, float volume, bool loop, bool doNotDestroy,
            SoundManagerType type, AudioMixerGroup mixer)
        {
            _audioClip = audioClip;
            _volume = Mathf.Clamp(volume, 0f, 1.0f);
            _loop = loop;
            _doNotDestroy = doNotDestroy;
            _type = type;
            _mixer = mixer;
            _duration = audioClip.length;

            switch (type)
            {
                case SoundManagerType.Effect:
                    _mainVolumeGroup = () => MainEffectVolume;
                    break;
                case SoundManagerType.Fx:
                    _mainVolumeGroup = () => MainFxVolume;
                    break;
                case SoundManagerType.Music:
                    _mainVolumeGroup = () => MainMusicVolume;
                    break;
                case SoundManagerType.OneTime:
                    _mainVolumeGroup = () => 1f;
                    break;
                default:
                    Debug.LogError($"{type} is unknown.");
                    break;
            }
        }

        /// <summary>
        ///     If the SoundManagerAudio is playing
        /// </summary>
        public bool IsPLaying => _audioSource != null && _audioSource.isPlaying;

        /// <summary>
        ///     The AudioSource GameObject
        /// </summary>
        private AudioSource AudioSource
        {
            get
            {
                if (Application.isPlaying == false) return null;

                if (_audioSource == null)
                {
                    // Create a new one
                    _audioSource = GameObject.AddComponent<AudioSource>();

                    _audioSource.clip = _audioClip;
                    _audioSource.volume = _volume * _mainVolumeGroup() * MainVolume;
                    _audioSource.loop = _loop;
                    _audioSource.outputAudioMixerGroup = _mixer;
                    _audioSource.playOnAwake = false;
                }

                return _audioSource;
            }
        }

        /// <summary>
        ///     Loop the SoundManagerAudio clip
        /// </summary>
        internal bool Loop
        {
            get => _loop;
            set
            {
                if (StopExist) return;

                _audioSource.loop = _loop = value;
            }
        }

        /// <summary>
        ///     Volume of the SoundManagerAudio
        /// </summary>
        internal float Volume
        {
            get => _volume;
            set
            {
                if (StopExist) return;

                _volume = value;
                _audioSource.volume = value * _mainVolumeGroup() * MainVolume;
            }
        }

        /// <summary>
        ///     Stop the SoundManagerAudio
        /// </summary>
        internal void Stop()
        {
            if (StopExist) return;

            AudioSource.Stop();

            if (_doNotDestroy) return;

            DestroyAudioSource();

            if (_currentMusic == this) _currentMusic = null;
        }

        /// <summary>
        ///     Pause the SoundManagerAudio
        /// </summary>
        internal void Pause()
        {
            if (StopExist) return;

            _playingToken?.Cancel();

            AudioSource.Pause();
        }

        /// <summary>
        ///     Play the SoundManagerAudio
        /// </summary>
        /// <param name="fadeinTime">The volume fadein in seconds</param>
        internal void Play(float fadeinTime = 0.0f)
        {
            if (Application.isPlaying == false) return;

            fadeinTime = Mathf.Clamp(fadeinTime, 0.0f, _duration);

            _transitionToken?.Cancel();

            // Music specific section
            if (_type == SoundManagerType.Music)
            {
                MusicTransitionTime = fadeinTime;

                if (_currentMusic != null && _currentMusic != this)
                {
                    if (_currentMusic.IsPLaying)
                    {
                        // Transition to the next music
                        _currentMusic.MusicTransition(this);
                        return;
                    }

                    _currentMusic.Stop();
                }

                // Set the new music
                _currentMusic = this;
            }

            if (_doNotDestroy || _loop)
            {
                // Play and loop or keep
                if (fadeinTime > 0.0f)
                    Fadein(fadeinTime);
                else
                    AudioSource.Play();
            }
            else
            {
                _playingToken?.Cancel();
                _playingToken = new CancellationTokenSource();

                if (fadeinTime > 0.0f)
                    Fadein(fadeinTime);
                else
                    AudioSource.Play();

                DestroyAudioSourceAsync();
            }
        }

        /// <summary>
        ///     Fadeout the SoundManagerAudio
        /// </summary>
        /// <param name="fadeoutTime">Fadeout fadeinTime in seconds</param>
        /// <param name="nextAction">The action to execute next</param>
        internal async void Fadeout(float fadeoutTime, [CanBeNull] Action nextAction = null)
        {
            if (StopExist) return;

            fadeoutTime = Mathf.Clamp(fadeoutTime, 0.0f, _duration);

            float startTransitionTime = Time.time;
            float endTransitionTime = startTransitionTime + fadeoutTime;

            float initialVolume = _volume * MainVolume * _mainVolumeGroup();

            _transitionToken?.Cancel();

            AudioSource.volume = initialVolume;

            _transitionToken = new CancellationTokenSource();

            CancellationToken ct = _transitionToken.Token;

            while (Time.time < endTransitionTime)
            {
                await Task.Delay(TimeSpan.FromSeconds(.05f), CancellationToken.None);
                if (StopExist || ct.IsCancellationRequested) return;

                AudioSource.volume = (initialVolume - initialVolume * (Mathf.Min((Time.time - startTransitionTime), fadeoutTime) / fadeoutTime)) * MainVolume * _mainVolumeGroup();
            }

            AudioSource.volume = initialVolume * MainVolume * _mainVolumeGroup();

            nextAction?.Invoke();
        }

        /// <summary>
        ///     Fadein the SoundManagerAudio
        /// </summary>
        /// <param name="fadeinTime">Fadein fadeinTime in seconds</param>
        private async void Fadein(float fadeinTime)
        {
            // The audio source mai not exist at this step, and it's normal
            if (Application.isPlaying == false) return;

            fadeinTime = Mathf.Clamp(fadeinTime, 0.0f, _duration);

            _transitionToken?.Cancel();
            AudioSource.volume = 0.0f;
            _audioSource.Play();

            float startTransitionTime = Time.time;
            float endTransitionTime = startTransitionTime + fadeinTime;

            _transitionToken = new CancellationTokenSource();

            CancellationToken ct = _transitionToken.Token;

            while (Time.time < endTransitionTime)
            {
                await Task.Delay(TimeSpan.FromSeconds(.05f), CancellationToken.None);
                if (StopExist || ct.IsCancellationRequested) return;

                AudioSource.volume = (Mathf.Min((Time.time - startTransitionTime), fadeinTime) / fadeinTime) * MainVolume * _mainVolumeGroup();
            }

            AudioSource.volume = _volume * MainVolume * _mainVolumeGroup();
        }

        /// <summary>
        ///     Transition between two musics audio
        /// </summary>
        /// <param name="playingNext">The music to play next</param>
        private void MusicTransition(SoundManagerAudio playingNext)
        {
            if (StopExist) return;

            _transitionToken?.Cancel();

            playingNext.Fadein(MusicTransitionTime);
            Fadeout(MusicTransitionTime, Stop);
            
            _currentMusic = playingNext;
        }

        /// <summary>
        ///     Destroy the AudioSource GameObject property asynchronously
        /// </summary>
        private async Task DestroyAudioSourceAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(_duration - _audioSource.time), _playingToken.Token);

            if (StopExist) return;

            DestroyAudioSource();
        }

        /// <summary>
        ///     Destroy the AudioSource GameObject property
        /// </summary>
        internal void DestroyAudioSource()
        {
            if (StopExist) return;

            UnityEngineObject.Destroy(_audioSource);
            _audioSource = null;
        }
    }

    #region Prepare

    /// <summary>
    ///     Prepare an effect by id
    /// </summary>
    /// <param name="id">The id of the effect to Prepare</param>
    /// <param name="volume">The volume of the sound to Prepare</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PrepareEffect(int id, float volume = 1.0f) =>
        PreparerBase(GetEffect(id), volume, false);

    /// <summary>
    ///     Prepare an effect by SoundManagerObject
    /// </summary>
    /// <param name="soundManagerAudio">The SoundManagerAudio effect to Prepare</param>
    /// <param name="volume">The volume of the sound to Prepare</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PrepareEffect(SoundManagerAudio soundManagerAudio, float volume = 1.0f) =>
        PreparerBase(soundManagerAudio, volume, false);

    /// <summary>
    ///     Add the AudioClip to the effect list and Prepare it
    /// </summary>
    /// <param name="audioClip">The audio clip to Prepare</param>
    /// <param name="volume">The volume of the sound to Prepare</param>
    /// <param name="doNotDestroy">Keep the AudioSource object after Preparing it</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PrepareEffect(AudioClip audioClip, float volume = 1.0f, bool doNotDestroy = false) =>
        PreparerBaseBuild(audioClip, EffectAudioCache, EffectAudioMixer, SoundManagerType.Effect, volume, false, doNotDestroy);

    /// <summary>
    ///     Prepare an FX by id
    /// </summary>
    /// <param name="id">The id of the FX to Prepare</param>
    /// <param name="volume">The volume of the sound to Prepare</param>
    /// <param name="loop">Loop the FX clip</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PrepareFx(int id, float volume = 1.0f, bool loop = false) =>
        PreparerBase(GetFx(id), volume, loop);

    /// <summary>
    ///     Prepare an FX by SoundManagerObject
    /// </summary>
    /// <param name="soundManagerAudio">The SoundManagerAudio FX to Prepare</param>
    /// <param name="volume">The volume of the sound to Prepare</param>
    /// <param name="loop">Loop the FX clip</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PrepareFx(SoundManagerAudio soundManagerAudio, float volume = 1.0f, bool loop = false) =>
        PreparerBase(soundManagerAudio, volume, loop);

    /// <summary>
    ///     Add the AudioClip to the FX list and Prepare it
    /// </summary>
    /// <param name="audioClip">The audio clip to Prepare</param>
    /// <param name="volume">The volume of the sound to Prepare</param>
    /// <param name="loop">Loop the FX clip</param>
    /// <param name="doNotDestroy">Keep the AudioSource object after Preparing it</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PrepareFx(AudioClip audioClip, float volume = 1.0f, bool loop = false, bool doNotDestroy = true) =>
        PreparerBaseBuild(audioClip, FxAudioCache, FxAudioMixer, SoundManagerType.Fx, volume, loop, doNotDestroy);

    /// <summary>
    ///     Prepare a music by id
    /// </summary>
    /// <param name="id">The id of the music to Prepare</param>
    /// <param name="volume">The volume of the music to Prepare</param>
    /// <param name="loop">Loop the music clip</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PrepareMusic(int id, float volume = 1.0f, bool loop = false) =>
        PreparerBase(GetMusic(id), volume, loop);

    /// <summary>
    ///     Prepare music by SoundManagerObject
    /// </summary>
    /// <param name="soundManagerAudio">The SoundManagerAudio music to Prepare</param>
    /// <param name="volume">The volume of the music to Prepare</param>
    /// <param name="loop">Loop the music clip</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PrepareMusic(SoundManagerAudio soundManagerAudio, float volume = 1.0f, bool loop = false) =>
        PreparerBase(soundManagerAudio, volume, loop);

    /// <summary>
    ///     Add the AudioClip to the music list and Prepare it
    /// </summary>
    /// <param name="audioClip">The audio clip to Prepare</param>
    /// <param name="volume">The volume of the music to Prepare</param>
    /// <param name="loop">Loop the music clip</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PrepareMusic(AudioClip audioClip, float volume = 1.0f, bool loop = false) =>
        PreparerBaseBuild(audioClip, MusicAudioCache, MusicAudioMixer, SoundManagerType.Music, volume, loop, false);

    /// <summary>
    ///     Generic method to centralize share Preparing logic for SoundManagerAudio base method
    /// </summary>
    /// <param name="soundManagerAudio"></param>
    /// <param name="volume">The volume of the clip to Prepare</param>
    /// <param name="loop">Loop the clip</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    private static SoundManagerAudio PreparerBase(SoundManagerAudio soundManagerAudio, float volume, bool loop)
    {
        soundManagerAudio.Volume = volume;
        soundManagerAudio.Loop = loop;

        return soundManagerAudio;
    }

    /// <summary>
    ///     Generic method to centralize share Preparing logic for AudioClip base method
    /// </summary>
    /// <param name="audioClip">The audio clip to Prepare</param>
    /// <param name="cache">The dictionary to add the SoundManagerAudio once created</param>
    /// <param name="audioMixerGroup">The audio mixer group to use for Preparing the clip</param>
    /// <param name="type">The type of the SoundManagerAudio</param>
    /// <param name="volume">The volume of the clip</param>
    /// <param name="loop">Loop the clip</param>
    /// <param name="doNotDestroy">Keep the AudioSource object after Preparing it</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    private static SoundManagerAudio PreparerBaseBuild(AudioClip audioClip, IDictionary<int, SoundManagerAudio> cache,
        AudioMixerGroup audioMixerGroup, SoundManagerType type, float volume, bool loop, bool doNotDestroy)
    {
        int id = audioClip.GetInstanceID();
        cache.TryGetValue(audioClip.GetInstanceID(), out SoundManagerAudio soundManagerAudio);

        if (soundManagerAudio != null) return soundManagerAudio;

        if (MusicAudioMixer != null)
            soundManagerAudio = new SoundManagerAudio(audioClip, volume, loop, doNotDestroy, type, audioMixerGroup);
        else if (AudioMixer != null)
            soundManagerAudio = new SoundManagerAudio(audioClip, volume, loop, doNotDestroy, type, AudioMixer);
        else
            soundManagerAudio = new SoundManagerAudio(audioClip, volume, loop, doNotDestroy, type, null);

        cache.Add(id, soundManagerAudio);

        return soundManagerAudio;
    }

    #endregion

    #region Play

    /// <summary>
    ///     Play an effect by id
    /// </summary>
    /// <param name="id">The id of the effect to play</param>
    /// <param name="volume">The volume of the sound to play</param>
    /// <param name="fadeinTime">The sound fadein in seconds</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PlayEffect(int id, float volume = 1.0f, float fadeinTime = 0.0f) =>
        PlayerBase(GetEffect(id), volume, false, fadeinTime);

    /// <summary>
    ///     Play an effect by SoundManagerObject
    /// </summary>
    /// <param name="soundManagerAudio">The SoundManagerAudio effect to play</param>
    /// <param name="volume">The volume of the sound to play</param>
    /// <param name="fadeinTime">The sound fadein in seconds</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PlayEffect(SoundManagerAudio soundManagerAudio, float volume = 1.0f, float fadeinTime = 0.0f) =>
        PlayerBase(soundManagerAudio, volume, false, fadeinTime);

    /// <summary>
    ///     Add the AudioClip to the effect list and play it
    /// </summary>
    /// <param name="audioClip">The audio clip to play</param>
    /// <param name="volume">The volume of the sound to play</param>
    /// <param name="doNotDestroy">Keep the AudioSource object after playing it</param>
    /// <param name="fadeinTime">The sound fadein in seconds</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PlayEffect(AudioClip audioClip, float volume = 1.0f, bool doNotDestroy = false, float fadeinTime = 0.0f) =>
        PlayerBaseBuild(audioClip, EffectAudioCache, EffectAudioMixer, SoundManagerType.Effect, volume, false, doNotDestroy, fadeinTime);

    /// <summary>
    ///     Play an FX by id
    /// </summary>
    /// <param name="id">The id of the FX to play</param>
    /// <param name="volume">The volume of the sound to play</param>
    /// <param name="loop">Loop the FX clip</param>
    /// <param name="fadeinTime">The sound fadein in seconds</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PlayFx(int id, float volume = 1.0f, bool loop = false, float fadeinTime = 0.0f) =>
        PlayerBase(GetFx(id), volume, loop, fadeinTime);

    /// <summary>
    ///     Play an FX by SoundManagerObject
    /// </summary>
    /// <param name="soundManagerAudio">The SoundManagerAudio FX to play</param>
    /// <param name="volume">The volume of the sound to play</param>
    /// <param name="loop">Loop the FX clip</param>
    /// <param name="fadeinTime">The sound fadein in seconds</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PlayFx(SoundManagerAudio soundManagerAudio, float volume = 1.0f, bool loop = false, float fadeinTime = 0.0f) =>
        PlayerBase(soundManagerAudio, volume, loop, fadeinTime);

    /// <summary>
    ///     Add the AudioClip to the FX list and play it
    /// </summary>
    /// <param name="audioClip">The audio clip to play</param>
    /// <param name="volume">The volume of the sound to play</param>
    /// <param name="loop">Loop the FX clip</param>
    /// <param name="doNotDestroy">Keep the AudioSource object after playing it</param>
    /// <param name="fadeinTime">The sound fadein in seconds</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PlayFx(AudioClip audioClip, float volume = 1.0f, bool loop = false, bool doNotDestroy = true, float fadeinTime = 0.0f) =>
        PlayerBaseBuild(audioClip, FxAudioCache, FxAudioMixer, SoundManagerType.Fx, volume, loop, doNotDestroy, fadeinTime);

    /// <summary>
    ///     Play a music by id
    /// </summary>
    /// <param name="id">The id of the music to play</param>
    /// <param name="volume">The volume of the music to play</param>
    /// <param name="loop">Loop the music clip</param>
    /// <param name="fadeinTime">The music transition in seconds</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PlayMusic(int id, float volume = 1.0f, bool loop = false, float fadeinTime = 0.0f) =>
        PlayerBase(GetMusic(id), volume, loop, fadeinTime);

    /// <summary>
    ///     Play music by SoundManagerObject
    /// </summary>
    /// <param name="soundManagerAudio">The SoundManagerAudio music to play</param>
    /// <param name="volume">The volume of the music to play</param>
    /// <param name="loop">Loop the music clip</param>
    /// <param name="fadeinTime">The music transition in seconds</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PlayMusic(SoundManagerAudio soundManagerAudio, float volume = 1.0f, bool loop = false, float fadeinTime = 0.0f) =>
        PlayerBase(soundManagerAudio, volume, loop, fadeinTime);

    /// <summary>
    ///     Add the AudioClip to the music list and play it
    /// </summary>
    /// <param name="audioClip">The audio clip to play</param>
    /// <param name="volume">The volume of the music to play</param>
    /// <param name="loop">Loop the music clip</param>
    /// <param name="fadeinTime">The music transition in seconds</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    public static SoundManagerAudio PlayMusic(AudioClip audioClip, float volume = 1.0f, bool loop = false, float fadeinTime = 0.0f) =>
        PlayerBaseBuild(audioClip, MusicAudioCache, MusicAudioMixer, SoundManagerType.Music, volume, loop, false, fadeinTime);

    /// <summary>
    ///     Play an AudioClip once and destroy the AudioSource object after done playing
    /// </summary>
    /// <param name="audioClip">The audio clip to play</param>
    /// <param name="volume">The volume of the clip to play</param>
    /// <param name="mixer">The sound mixer to use for the clip</param>
    public static void PlayOnce(AudioClip audioClip, float volume = 1.0f, AudioMixerGroup mixer = null)
    {
        SoundManagerAudio soundManagerAudio = new SoundManagerAudio(audioClip, volume, false, false, SoundManagerType.OneTime, mixer);

        soundManagerAudio.Play();
    }

    /// <summary>
    ///     Generic method to centralize share playing logic for SoundManagerAudio base method
    /// </summary>
    /// <param name="soundManagerAudio"></param>
    /// <param name="volume">The volume of the clip to play</param>
    /// <param name="loop">Loop the clip</param>
    /// <param name="fadeinTime">The sound fadein in seconds</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    private static SoundManagerAudio PlayerBase(SoundManagerAudio soundManagerAudio, float volume, bool loop, float fadeinTime)
    {
        soundManagerAudio.Volume = volume;
        soundManagerAudio.Loop = loop;

        soundManagerAudio.Play(fadeinTime);

        return soundManagerAudio;
    }

    /// <summary>
    ///     Generic method to centralize share playing logic for AudioClip base method
    /// </summary>
    /// <param name="audioClip">The audio clip to play</param>
    /// <param name="cache">The dictionary to add the SoundManagerAudio once created</param>
    /// <param name="audioMixerGroup">The audio mixer group to use for playing the clip</param>
    /// <param name="type">The type of the SoundManagerAudio</param>
    /// <param name="volume">The volume of the clip</param>
    /// <param name="loop">Loop the clip</param>
    /// <param name="doNotDestroy">Keep the AudioSource object after playing it</param>
    /// <param name="fadeinTime">The sound fadein in seconds</param>
    /// <returns>The resulting SoundManagerAudio object</returns>
    private static SoundManagerAudio PlayerBaseBuild(AudioClip audioClip, IDictionary<int, SoundManagerAudio> cache, AudioMixerGroup audioMixerGroup, SoundManagerType type, float volume, bool loop, bool doNotDestroy, float fadeinTime)
    {
        // Re-use the PreparerBaseBuild method from the precedent section
        SoundManagerAudio soundManagerAudio = PreparerBaseBuild(audioClip, cache, audioMixerGroup, type, volume, loop, doNotDestroy);

        soundManagerAudio.Play(fadeinTime);

        return soundManagerAudio;
    }

    #endregion

    #region Pause All

    /// <summary>
    ///     Pause all playing elements
    /// </summary>
    /// <param name="fadeoutTime">Fadeout in seconds</param>
    /// <returns>The list of paused SoundManagerAudio</returns>
    public static List<SoundManagerAudio> PauseAll(float fadeoutTime = 0.0f)
    {
        List<SoundManagerAudio> pausedList = new List<SoundManagerAudio>();

        pausedList.AddRange(PauseAllEffect(fadeoutTime));
        pausedList.AddRange(PauseAllFx(fadeoutTime));
        pausedList.AddRange(PauseAllMusic(fadeoutTime));

        return pausedList;
    }

    /// <summary>
    ///     Pause all playing effects
    /// </summary>
    /// <param name="fadeoutTime">Fadeout in seconds</param>
    /// <returns>The list of paused SoundManagerAudio</returns>
    public static List<SoundManagerAudio> PauseAllEffect(float fadeoutTime = 0.0f) =>
        PauseAllBase(EffectAudioCache, fadeoutTime);

    /// <summary>
    ///     Pause all playing FX
    /// </summary>
    /// <param name="fadeoutTime">Fadeout in seconds</param>
    /// <returns>The list of paused SoundManagerAudio</returns>
    public static List<SoundManagerAudio> PauseAllFx(float fadeoutTime = 0.0f) =>
        PauseAllBase(FxAudioCache, fadeoutTime);

    /// <summary>
    ///     Pause the playing music
    /// </summary>
    /// <param name="fadeoutTime">Fadeout in seconds</param>
    /// <returns>The list of paused SoundManagerAudio</returns>
    public static List<SoundManagerAudio> PauseAllMusic(float fadeoutTime = 0.0f) =>
        PauseAllBase(MusicAudioCache, fadeoutTime);

    /// <summary>
    ///     Generic method to pause playing SoundManagerAudio playing in the specified list
    /// </summary>
    /// <param name="cache">The dictionary to gather the playing element list from</param>
    /// <param name="fadeout">Fadeout in seconds</param>
    /// <returns>The list of paused SoundManagerAudio</returns>
    private static List<SoundManagerAudio> PauseAllBase(Dictionary<int, SoundManagerAudio> cache, float fadeout)
    {
        List<SoundManagerAudio> pausedList = new List<SoundManagerAudio>();

        foreach (KeyValuePair<int, SoundManagerAudio> entry in cache.Where(entry => entry.Value.IsPLaying))
        {
            pausedList.Add(entry.Value);

            if (fadeout > 0.0f)
            {
                entry.Value.Fadeout(fadeout, entry.Value.Pause);
            }
            else entry.Value.Pause();
        }

        return pausedList;
    }

    #endregion

    #region Pause

    /// <summary>
    ///     Pause a playing effect by id
    /// </summary>
    /// <param name="id">The id of the effect</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static SoundManagerAudio PauseEffect(int id, float fadeoutTime = 0.0f) =>
        PauseBase(GetEffect(id), fadeoutTime);

    /// <summary>
    ///     Pause a playing effect by AudioClip
    /// </summary>
    /// <param name="audioClip">The AudioClip to pause</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static SoundManagerAudio PauseEffect(AudioClip audioClip, float fadeoutTime = 0.0f) =>
        PauseBase(FindEffect(audioClip), fadeoutTime);

    /// <summary>
    ///     Pause a playing effect by SoundManagerAudio
    /// </summary>
    /// <param name="soundManagerAudio">The SoundManagerAudio to pause</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static SoundManagerAudio PauseEffect(SoundManagerAudio soundManagerAudio, float fadeoutTime = 0.0f) =>
        PauseBase(soundManagerAudio, fadeoutTime);

    /// <summary>
    ///     Pause a playing FX by id
    /// </summary>
    /// <param name="id">The id of the effect</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static SoundManagerAudio PauseFx(int id, float fadeoutTime = 0.0f) =>
        PauseBase(GetFx(id), fadeoutTime);

    /// <summary>
    ///     Pause a playing FX by AudioClip
    /// </summary>
    /// <param name="audioClip">The AudioClip to pause</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static SoundManagerAudio PauseFx(AudioClip audioClip, float fadeoutTime = 0.0f) =>
        PauseBase(FindFx(audioClip), fadeoutTime);

    /// <summary>
    ///     Pause a playing FX by SoundManagerAudio
    /// </summary>
    /// <param name="soundManagerAudio">The SoundManagerAudio to pause</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static SoundManagerAudio PauseFx(SoundManagerAudio soundManagerAudio, float fadeoutTime = 0.0f) =>
        PauseBase(soundManagerAudio, fadeoutTime);

    /// <summary>
    ///     Pause the playing music by id
    /// </summary>
    /// <param name="id">The id of the effect</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static SoundManagerAudio PauseMusic(int id, float fadeoutTime = 0.0f) =>
        PauseBase(GetMusic(id), fadeoutTime);

    /// <summary>
    ///     Pause the playing music by AudioClip
    /// </summary>
    /// <param name="audioClip">The AudioClip to pause</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static SoundManagerAudio PauseMusic(AudioClip audioClip, float fadeoutTime = 0.0f) =>
        PauseBase(FindMusic(audioClip), fadeoutTime);

    /// <summary>
    ///     Pause the playing music by SoundManagerAudio
    /// </summary>
    /// <param name="soundManagerAudio">The SoundManagerAudio to pause</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static SoundManagerAudio PauseMusic(SoundManagerAudio soundManagerAudio, float fadeoutTime = 0.0f) =>
        PauseBase(soundManagerAudio, fadeoutTime);

    /// <summary>
    ///     Generic method to share pause logic for SoundManagerAudio base method
    /// </summary>
    /// <param name="soundManagerAudio">The SoundManagerAudio to pause</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    private static SoundManagerAudio PauseBase(SoundManagerAudio soundManagerAudio, float fadeoutTime)
    {
        if (fadeoutTime > 0.0f)
            soundManagerAudio.Fadeout(fadeoutTime, soundManagerAudio.Pause);
        else
            soundManagerAudio.Pause();

        return soundManagerAudio;
    }

    #endregion

    #region Stop All

    /// <summary>
    ///     Stop all playing elements
    /// </summary>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static void StopAll(float fadeoutTime = 0.0f)
    {
        StopAllEffect(fadeoutTime);
        StopAllFx(fadeoutTime);
        StopAllMusic(fadeoutTime);
    }

    /// <summary>
    ///     Stop all playing effects
    /// </summary>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static void StopAllEffect(float fadeoutTime = 0.0f) =>
        StopAllBase(EffectAudioCache, fadeoutTime);

    /// <summary>
    ///     Stop all playing FX
    /// </summary>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static void StopAllFx(float fadeoutTime = 0.0f) =>
        StopAllBase(FxAudioCache, fadeoutTime);

    /// <summary>
    ///     Stop the playing music
    /// </summary>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static void StopAllMusic(float fadeoutTime = 0.0f) =>
        StopAllBase(MusicAudioCache, fadeoutTime);

    /// <summary>
    ///     Generic method with share logic to stop playing elements in cache
    /// </summary>
    /// <param name="cache">The dictionary to find the SoundManagerAudio from</param>
    /// <param name="fadeout">Fadeout time in seconds</param>
    private static void StopAllBase(Dictionary<int, SoundManagerAudio> cache, float fadeout)
    {
        foreach (KeyValuePair<int, SoundManagerAudio> entry in cache)
        {
            if (!entry.Value.IsPLaying) continue;

            if (fadeout > 0.0f) entry.Value.Fadeout(fadeout, entry.Value.Stop);
            else entry.Value.Stop();
        }
    }

    #endregion

    #region Stop

    /// <summary>
    ///     Stop an effect by id
    /// </summary>
    /// <param name="id">The id of the playing effect</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static void StopEffect(int id, float fadeoutTime = 0.0f) =>
        StopBase(GetEffect(id), fadeoutTime);

    /// <summary>
    ///     Stop an effect by AudioClip
    /// </summary>
    /// <param name="audioClip">The AudioClip of the playing effect</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static void StopEffect(AudioClip audioClip, float fadeoutTime = 0.0f) =>
        StopBase(FindEffect(audioClip), fadeoutTime);

    /// <summary>
    ///     Stop an effect by SoundManagerAudio
    /// </summary>
    /// <param name="soundManagerAudio">The SoundManagerAudio to stop</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static void StopEffect(SoundManagerAudio soundManagerAudio, float fadeoutTime = 0.0f) =>
        StopBase(soundManagerAudio, fadeoutTime);

    /// <summary>
    ///     Stop an FX by id
    /// </summary>
    /// <param name="id">The id of the playing FX</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static void StopFx(int id, float fadeoutTime = 0.0f) =>
        StopBase(GetFx(id), fadeoutTime);

    /// <summary>
    ///     Stop an FX by AudioClip
    /// </summary>
    /// <param name="audioClip">The AudioClip of the playing FX</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static void StopFx(AudioClip audioClip, float fadeoutTime = 0.0f) =>
        StopBase(FindFx(audioClip), fadeoutTime);

    /// <summary>
    ///     Stop an FX by SoundManagerAudio
    /// </summary>
    /// <param name="soundManagerAudio">The SoundManagerAudio to stop</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static void StopFx(SoundManagerAudio soundManagerAudio, float fadeoutTime = 0.0f) =>
        StopBase(soundManagerAudio, fadeoutTime);

    /// <summary>
    ///     Stop music by id
    /// </summary>
    /// <param name="id">The id of the playing music</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static void StopMusic(int id, float fadeoutTime = 0.0f) =>
        StopBase(GetMusic(id), fadeoutTime);

    /// <summary>
    ///     Stop music by AudioClip
    /// </summary>
    /// <param name="audioClip">The AudioClip of the playing music</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static void StopMusic(AudioClip audioClip, float fadeoutTime = 0.0f) =>
        StopBase(FindMusic(audioClip), fadeoutTime);

    /// <summary>
    ///     Stop music by SoundManagerAudio
    /// </summary>
    /// <param name="soundManagerAudio">The SoundManagerAudio to stop</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    public static void StopMusic(SoundManagerAudio soundManagerAudio, float fadeoutTime = 0.0f) =>
        StopBase(soundManagerAudio, fadeoutTime);

    /// <summary>
    ///     Generic method to share stop logic for SoundManagerAudio base method
    /// </summary>
    /// <param name="soundManagerAudio">The SoundManagerAudio to stop</param>
    /// <param name="fadeoutTime">Fadeout time in seconds</param>
    private static void StopBase(SoundManagerAudio soundManagerAudio, float fadeoutTime)
    {
        if (soundManagerAudio == null) return;
        
        if (fadeoutTime > 0.0f)
            soundManagerAudio.Fadeout(fadeoutTime, soundManagerAudio.Stop);
        else
            soundManagerAudio.Stop();
    }

    #endregion

    #region Get

    /// <summary>
    ///     Return the SoundManagerAudio from the effect cache
    /// </summary>
    /// <param name="id">The id of the SoundManagerAudio</param>
    /// <returns></returns>
    public static SoundManagerAudio GetEffect(int id) =>
        GetterBase(FindEffect(id), id.ToString());

    /// <summary>
    ///     Return the SoundManagerAudio from the effect cache
    /// </summary>
    /// <param name="audioClip">The AudioClip to get in cache</param>
    /// <returns></returns>
    public static SoundManagerAudio GetEffect(AudioClip audioClip) =>
        GetterBase(FindEffect(audioClip), audioClip.name);

    /// <summary>
    ///     Return the SoundManagerAudio from the FX cache
    /// </summary>
    /// <param name="id">The id of the SoundManagerAudio</param>
    /// <returns></returns>
    public static SoundManagerAudio GetFx(int id) =>
        GetterBase(FindFx(id), id.ToString());

    /// <summary>
    ///     Return the SoundManagerAudio from the FX cache
    /// </summary>
    /// <param name="audioClip">The AudioClip to get in cache</param>
    /// <returns></returns>
    public static SoundManagerAudio GetFx(AudioClip audioClip) =>
        GetterBase(FindFx(audioClip), audioClip.name);

    /// <summary>
    ///     Return the SoundManagerAudio from the music cache
    /// </summary>
    /// <param name="id">The id of the SoundManagerAudio</param>
    /// <returns></returns>
    public static SoundManagerAudio GetMusic(int id) =>
        GetterBase(FindMusic(id), id.ToString());

    /// <summary>
    ///     Return the SoundManagerAudio from the music cache
    /// </summary>
    /// <param name="audioClip">The AudioClip to get in cache</param>
    /// <returns></returns>
    public static SoundManagerAudio GetMusic(AudioClip audioClip) =>
        GetterBase(FindMusic(audioClip), audioClip.name);

    /// <summary>
    ///     Generic method to share getter logic between AudioClip getters
    /// </summary>
    /// <param name="audio">The SoundManagerAudio</param>
    /// <param name="name">The name or id of the element searched</param>
    /// <returns></returns>
    /// <exception cref="GetAudioException"></exception>
    private static SoundManagerAudio GetterBase(SoundManagerAudio audio, string name)
    {
        if (audio != null)
            return audio;

        throw new GetAudioException($"Can't get {name}");
    }

    #endregion

    #region Find

    /// <summary>
    ///     Return the SoundManagerAudio from the effect cache
    /// </summary>
    /// <param name="id">The id of the SoundManagerAudio</param>
    /// <returns></returns>
    [CanBeNull]
    public static SoundManagerAudio FindEffect(int id) =>
        EffectAudioCache.TryGetValue(id, out SoundManagerAudio value) ? value : null;

    /// <summary>
    ///     Return the SoundManagerAudio from the effect cache
    /// </summary>
    /// <param name="audioClip">The AudioClip to find in cache</param>
    /// <returns></returns>
    [CanBeNull]
    public static SoundManagerAudio FindEffect(AudioClip audioClip) =>
        FinderBase(audioClip, FindEffect);

    /// <summary>
    ///     Return the SoundManagerAudio from the FX cache
    /// </summary>
    /// <param name="id">The id of the SoundManagerAudio</param>
    /// <returns></returns>
    [CanBeNull]
    public static SoundManagerAudio FindFx(int id) =>
        FxAudioCache.TryGetValue(id, out SoundManagerAudio value) ? value : null;

    /// <summary>
    ///     Return the SoundManagerAudio from the FX cache
    /// </summary>
    /// <param name="audioClip">The AudioClip to find in cache</param>
    /// <returns></returns>
    [CanBeNull]
    public static SoundManagerAudio FindFx(AudioClip audioClip) =>
        FinderBase(audioClip, FindFx);

    /// <summary>
    ///     Return the SoundManagerAudio from the music cache
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The id of the SoundManagerAudio</returns>
    [CanBeNull]
    public static SoundManagerAudio FindMusic(int id) =>
        MusicAudioCache.TryGetValue(id, out SoundManagerAudio value) ? value : null;

    /// <summary>
    ///     Return the SoundManagerAudio from the music cache
    /// </summary>
    /// <param name="audioClip">The AudioClip to find in cache</param>
    /// <returns></returns>
    [CanBeNull]
    public static SoundManagerAudio FindMusic(AudioClip audioClip) =>
        FinderBase(audioClip, FindMusic);

    /// <summary>
    ///     Generic method to share find logic between AudioClip finders
    /// </summary>
    /// <param name="audioClip">The AudioClip to find in cache</param>
    /// <param name="cache">The dictionary to find the SoundManagerAudio from</param>
    /// <returns></returns>
    /// <exception cref="GetAudioException"></exception>
    private static SoundManagerAudio FinderBase(AudioClip audioClip, Func<int, SoundManagerAudio> cache)
    {
        int id = audioClip.GetInstanceID();

        return cache(id);
    }

    #endregion

    #region Clear

    public static void ClearAll(bool includePlaying = false, bool force = false)
    {
        ClearEffect(includePlaying, force);
        ClearFx(includePlaying, force);
        ClearMusic(includePlaying, force);
    }

    public static void ClearEffect(bool includePlaying = false, bool force = false)
    {
        Dictionary<int, SoundManagerAudio> localCopy = EffectAudioCache;

        foreach (KeyValuePair<int, SoundManagerAudio> soundManagerAudio in localCopy
            .Where(soundManagerAudio => force || includePlaying || !soundManagerAudio.Value.IsPLaying)
            .Where(soundManagerAudio => force || !soundManagerAudio.Value.DoNotDestroy))
        {
            soundManagerAudio.Value.DestroyAudioSource();
        }
    }
    
    public static void ClearFx(bool includePlaying = false, bool force = false)
    {
        Dictionary<int, SoundManagerAudio> localCopy = FxAudioCache;

        foreach (KeyValuePair<int, SoundManagerAudio> soundManagerAudio in localCopy
            .Where(soundManagerAudio => force || includePlaying || !soundManagerAudio.Value.IsPLaying)
            .Where(soundManagerAudio => force || !soundManagerAudio.Value.DoNotDestroy))
        {
            soundManagerAudio.Value.DestroyAudioSource();
        }
    }
    
    public static void ClearMusic(bool includePlaying = false, bool force = false)
    {
        Dictionary<int, SoundManagerAudio> localCopy = MusicAudioCache;

        foreach (KeyValuePair<int, SoundManagerAudio> soundManagerAudio in localCopy
            .Where(soundManagerAudio => force || includePlaying || !soundManagerAudio.Value.IsPLaying)
            .Where(soundManagerAudio => force || !soundManagerAudio.Value.DoNotDestroy))
        {
            soundManagerAudio.Value.DestroyAudioSource();
        }
    }

    #endregion

    #region Volume

    private static float _mainVolume = 1.0f;
    private static float _mainEffectVolume = 1.0f;
    private static float _mainFxVolume = 1.0f;
    private static float _mainMusicVolume = 1.0f;

    /// <summary>
    ///     Main volume control
    /// </summary>
    public static float MainVolume
    {
        get => _mainVolume;
        set
        {
            _mainVolume = Mathf.Clamp(value, 0f, 1f);

            MainEffectVolume = MainEffectVolume;
            MainFxVolume = MainFxVolume;
            MainMusicVolume = MainMusicVolume;
        }
    }

    /// <summary>
    ///     Main effect volume
    /// </summary>
    public static float MainEffectVolume
    {
        get => _mainEffectVolume;
        set
        {
            _mainEffectVolume = Mathf.Clamp(value, 0f, 1f);

            foreach (KeyValuePair<int, SoundManagerAudio> entry in EffectAudioCache)
                entry.Value.Volume = entry.Value.Volume;
        }
    }

    /// <summary>
    ///     Main FX volume
    /// </summary>
    public static float MainFxVolume
    {
        get => _mainFxVolume;
        set
        {
            _mainFxVolume = Mathf.Clamp(value, 0f, 1f);

            foreach (KeyValuePair<int, SoundManagerAudio> entry in FxAudioCache)
                entry.Value.Volume = entry.Value.Volume;
        }
    }

    /// <summary>
    ///     Main music volume
    /// </summary>
    public static float MainMusicVolume
    {
        get => _mainMusicVolume;
        set
        {
            _mainMusicVolume = Mathf.Clamp(value, 0f, 1f);

            foreach (KeyValuePair<int, SoundManagerAudio> entry in MusicAudioCache)
                entry.Value.Volume = entry.Value.Volume;
        }
    }

    #endregion
}