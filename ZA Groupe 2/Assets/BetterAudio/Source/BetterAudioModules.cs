using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BetterAudio
{
    [System.Serializable]
    public partial class BehavioursGroup
    {
        [SerializeField]
        [SerializeReference]
        public List<BetterAudioBehaviour> items;

        public BehavioursGroup()
        {
            items = new List<BetterAudioBehaviour>();
        }

        public List<string> names
        {
            get
            {
                List<string> result = new List<string>();
                for (int i = 0; i < items.Count; i++)
                {
                    result.Add(items[i].GetName());
                }
                return result;
            }
        }

        public bool Add(BetterAudioBehaviour behaviour)
        {
            if (!names.Contains(behaviour.Name) || behaviour.canMultipleInstances())
            {
                items.Add(behaviour);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Remove(BetterAudioBehaviour behaviour)
        {
            if (items.Contains(behaviour))
            {
                items.Remove(behaviour);
            }
        }

        public bool Find<T>(string name, out T result) where T : BetterAudioBehaviour
        {
            foreach (BetterAudioBehaviour b in items)
            {
                if (b.Name == name && b.GetType() == typeof(T))
                {
                    result = (T)b;
                    return true;
                }
            }
            result = null;
            return false;
        }
    }


    [System.Serializable]
    public class InheritVolume
    {
        [SerializeField]
        public bool isInherited;
        [SerializeField]
        public bool isSelfTarget = true;
        [SerializeField]
        [SerializeReference]
        public GameObject Target;
        [SerializeField]
        string name;

        [SerializeField]
        [SerializeReference]
        RandomFloat _value;

        public GameObject baseObj;

        public InheritVolume(string n)
        {
            name = n;
            _value = new RandomFloat("", true);
        }

        public void SetCaller(GameObject caller)
        {
            baseObj = caller;
        }

        public float Value
        {
            get
            {
                if (isInherited)
                {
                    if (isSelfTarget)
                        Target = baseObj;

                    AudioSource source;
                    if (Target.TryGetComponent<AudioSource>(out source))
                    {
                        return source.volume;
                    }
                    else
                    {
                        Debug.LogError("Unable to find a compatible object to inherit volume");
                        return 0;
                    }
                }
                else
                {
                    return _value.Value;
                }
            }
        }

        public void DrawGUI()
        {
#if UNITY_EDITOR
            GUIStyle module = new GUIStyle();

            module.margin = new RectOffset(15, 15, 0, 0);

            EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(module);
            {

                isInherited = EditorGUILayout.Toggle("Inherit", isInherited);

                if (isInherited)
                {
                    isSelfTarget = EditorGUILayout.Toggle("From self", isSelfTarget);
                    if (!isSelfTarget)
                    {
                        Target = (GameObject)EditorGUILayout.ObjectField("Target", Target, typeof(GameObject), true);

                        if (Target)
                        {
                            AudioSource source;
                            if (!Target.TryGetComponent<AudioSource>(out source))
                            {
                                Debug.LogError("[BetterAudio] Selected object do not have an audio source, cannot inherit volume.", baseObj);
                                Target = baseObj;
                            }
                        }
                    }
                    else
                    {
                        Target = baseObj;

                    }
                    if (Target)
                        EditorGUILayout.LabelField("Value: " + Value.ToString(), EditorStyles.miniLabel);
                }
                else
                {
                    Rect rect = EditorGUILayout.GetControlRect(false, 1);

                    rect.height = 1;

                    EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

                    _value.DrawGUI();
                }
            }
            EditorGUILayout.EndVertical();
#endif
        }
    }

    [System.Serializable]
    public class SceneTransform
    {
        public enum SearchMode
        {
            Reference,
            FindByName,
            FindByTag
        }

        [SerializeField]
        string name;

        [SerializeField]
        Transform _target;

        [SerializeField]
        public Transform target
        {
            set
            {
                _target = value;
            }
            get
            {
                if (searchMode == SearchMode.FindByName)
                {
                    _target = GameObject.Find(targetName).transform;
                }else if (searchMode == SearchMode.FindByTag)
                {
                    _target = GameObject.FindGameObjectWithTag(targetName).transform;
                }

                

                return _target;
            }
        }

        [SerializeField]
        public string targetName;

        [SerializeField]
        public SearchMode searchMode = SearchMode.Reference;

        public SceneTransform(string n)
        {
            
            name = n;
        }


        public void DrawGUI()
        {
#if UNITY_EDITOR

            GUIStyle module = new GUIStyle();

            module.margin = new RectOffset(15, 15, 0, 0);
            if (name != "")
                EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(module);
            {

                searchMode = (SearchMode)EditorGUILayout.EnumPopup("Mode", searchMode);

                if (searchMode == SearchMode.Reference)
                {
                    target = (Transform)EditorGUILayout.ObjectField("Target", target, typeof(Transform), true);
                }
                else if (searchMode == SearchMode.FindByName)
                {
                    targetName = EditorGUILayout.TextField("Target Name", targetName);
                }
                else if (searchMode == SearchMode.FindByTag)
                {
                    targetName = EditorGUILayout.TextField("Target Tag", targetName);
                }
            }
            EditorGUILayout.EndVertical();

#endif
        }
    }

    [System.Serializable]
    public class RandomFloat
    {
        [SerializeField]
        public float min;
        [SerializeField]
        public float max;
        [SerializeField]
        float _value;
        [SerializeField]
        string name;
        [SerializeField]
        public bool isRandom;

        [SerializeField]
        bool isSlider;
        [SerializeField]
        float sliderMin;
        [SerializeField]
        float sliderMax;

        public RandomFloat(string n, bool slider = false, float sMin = 0, float sMax = 1, float value = 0)
        {
            name = n;
            isSlider = slider;
            sliderMin = sMin;
            min = sMin;
            sliderMax = sMax;
            max = sMax;
            _value = value;
        }

        public float Value
        {
            get
            {
                if (isRandom)
                {
                    _value = Random.Range(min, max);
                    return _value;
                }
                else
                {
                    return _value;
                }
            }
            set
            {
                _value = value;
            }
        }

        public void DrawGUI()
        {
#if UNITY_EDITOR

            GUIStyle module = new GUIStyle();

            module.margin = new RectOffset(15, 15, 0, 0);
            if (name != "")
                EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(module);
            {

                isRandom = EditorGUILayout.Toggle("Is random", isRandom);

                if (isRandom)
                {
                    min = EditorGUILayout.FloatField("Min", Mathf.Clamp(min, sliderMin, sliderMax));
                    max = EditorGUILayout.FloatField("Max", Mathf.Clamp(max, sliderMin, sliderMax));
                    EditorGUILayout.LabelField("Value: " + _value.ToString(), EditorStyles.miniLabel);
                }
                else
                {
                    if (isSlider)
                    {
                        EditorGUILayout.LabelField("Value");
                        _value = EditorGUILayout.Slider(_value, sliderMin, sliderMax);
                    } else
                    {
                        _value = EditorGUILayout.FloatField("Value", _value);
                    }
                }
            } EditorGUILayout.EndVertical();

#endif
        }
    }


    [System.Serializable]
    public class RandomInt
    {
        [SerializeField]
        int min;
        [SerializeField]
        int max;
        [SerializeField]
        int _value;
        [SerializeField]
        string name;
        [SerializeField]
        public bool isRandom;

        [SerializeField]
        bool isSlider;
        [SerializeField]
        int sliderMin;
        [SerializeField]
        int sliderMax;

        public RandomInt(string n, bool slider = false, int sMin = 0, int sMax = 1, int value = 0)
        {
            name = n;
            isSlider = slider;
            sliderMin = sMin;
            min = sMin;
            sliderMax = sMax;
            max = sMax;
            _value = value;
        }

        public int Value
        {
            get
            {
                if (isRandom)
                {
                    _value = Random.Range(min, max);
                    return _value;
                }
                else
                {
                    return _value;
                }
            }
        }

        public void DrawGUI()
        {
#if UNITY_EDITOR

            GUIStyle module = new GUIStyle();

            module.margin = new RectOffset(15, 15, 0, 0);
            if (name != "")
                EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(module);
            {

                isRandom = EditorGUILayout.Toggle("Is random", isRandom);

                if (isRandom)
                {
                    min = EditorGUILayout.IntField("Min", Mathf.Clamp(min, sliderMin, sliderMax));
                    max = EditorGUILayout.IntField("Max", Mathf.Clamp(max, sliderMin, sliderMax));
                    EditorGUILayout.LabelField("Value: " + _value.ToString(), EditorStyles.miniLabel);
                }
                else
                {
                    if (isSlider)
                    {
                        EditorGUILayout.LabelField("Value");
                        _value = EditorGUILayout.IntSlider(_value, sliderMin, sliderMax);
                    }
                    else
                    {
                        _value = EditorGUILayout.IntField("Value", _value);
                    }
                }
            }
            EditorGUILayout.EndVertical();

#endif
        }
    }

    [SerializeField]
    [System.Serializable]
    public class BetterArrayAudioClip
    {
        [SerializeField]
        [SerializeReference]
        public List<AudioClip> items;

        [SerializeField]
        bool sceneObj = false;

        public BetterArrayAudioClip(bool scene) {
            items = new List<AudioClip>();
            sceneObj = scene;
        }

        AudioClip o;

        public void OnGUI(BetterAudioSource caller)
        {
#if UNITY_EDITOR
            GUIStyle element = new GUIStyle();
            element.margin = new RectOffset(3, 3, 3, 3);

            GUIStyle frame = new GUIStyle();
            frame.margin = new RectOffset(10, 10, 10, 10);
            EditorGUILayout.BeginVertical(frame);
            {


                foreach (AudioClip obj in items)
                {
                    EditorGUILayout.BeginHorizontal(element);
                    EditorGUILayout.LabelField(obj.name);
                    if (GUILayout.Button("Remove"))
                    {
                        items.Remove(obj);
                        if (caller)
                            EditorUtility.SetDirty(caller);
                        break;
                    }

                    EditorGUILayout.EndHorizontal();
                    Rect rect = EditorGUILayout.GetControlRect(false, 1);

                    rect.height = 1;

                    EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
                }
            }
            EditorGUILayout.EndVertical();
            

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();

            o = (AudioClip)EditorGUILayout.ObjectField(o, typeof(AudioClip), sceneObj);

            if (o != null)
            {
                if (GUILayout.Button("Add"))
                {
                    items.Add(o);
                    if (caller)
                        EditorUtility.SetDirty(caller);
                }
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                GUILayout.Button("Add");
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
#endif
        }
    }


    public struct sourceProperty
    {
        public float value;
        public float min;
        public float max;
        public AnimationCurve defaultCurve;

        public sourceProperty(float val, float mi, float ma, AnimationCurve c)
        {
            value = val;
            min = mi;
            max = ma;
            defaultCurve = c;
        }
    }

    public static class sourceProperties{
        public enum sourcePropertiesList
        {
            Time,
            ClipDuration,
            Volume,
            Pitch,
            Stereo_Pan,
            Spatial_Blend,
            Reverb_Zone,
            Doppler_Level,
            Spread,
            Min_Distance,
            Max_Distance

        }

        static sourceProperty Volume = new sourceProperty(0.8f, 0, 1, MakeTwoPointsCurve(0, 1));
        static sourceProperty Pitch = new sourceProperty(1, -3, 3, MakeTwoPointsCurve(-3, 3));
        static sourceProperty Stereo_Pan = new sourceProperty(0, -1, 1, MakeTwoPointsCurve(-1, 1));
        static sourceProperty Spatial_Blend = new sourceProperty(0, 0, 1, MakeTwoPointsCurve(0, 1));
        static sourceProperty Reverb_Zone = new sourceProperty(0, 0, 1, MakeTwoPointsCurve(0, 1));
        static sourceProperty Doppler_Level = new sourceProperty(1, 0, 5, MakeTwoPointsCurve(0, 5));
        static sourceProperty Spread = new sourceProperty(0, 0, 360, MakeTwoPointsCurve(0, 360));
        static sourceProperty Min_Distance = new sourceProperty(1, 0, 99999, MakeTwoPointsCurve(0, 10));
        static sourceProperty Max_Distance = new sourceProperty(500, 0, 99999, MakeTwoPointsCurve(10, 500));

        public static sourceProperty[] properties = { Volume, Pitch, Stereo_Pan, Spatial_Blend, Reverb_Zone, Doppler_Level, Spread, Min_Distance, Max_Distance };

        public static AnimationCurve MakeTwoPointsCurve(float start, float end)
        {
            AnimationCurve c = new AnimationCurve();
            c.AddKey(new Keyframe(0, start));
            c.AddKey(new Keyframe(1, end));
            return c;
        }
    }

    public  class TimeProperty
    {
        [SerializeField]
        public float percent = 0;

        [SerializeField]
        BetterAudioSource main;

        [SerializeField]
        public string name;

        public TimeProperty(string n, BetterAudioSource caller)
        {
            name = n;
            main = caller;
        }

        public void SetCaller(BetterAudioSource caller)
        {
            main = caller;
        }

        public void DrawGUI()
        {
#if UNITY_EDITOR
            EditorGUILayout.LabelField(name);
            EditorGUILayout.LabelField("Clip time percent");
            percent = EditorGUILayout.Slider(percent, 0, 1);

            if (main && main.source.clip)
            {
                EditorGUILayout.LabelField("Clip time: " + BetterAudio.SecondsFormat(percent * main.source.clip.length));
            }

#endif
        }
    }

    [System.Serializable]
    public class LocalVariable
    {
        [SerializeField]
        public System.Type type;
        [SerializeField]
        public string Name;
        [SerializeField]
        public object value;

        public LocalVariable(string _name, object _value, System.Type _type)
        {
            Name = _name;
            value = _value;
            type = _type;
        }
    }

    [System.Serializable]
    public class BetterValue
    {
        [SerializeField]
        Component value;

        [SerializeField]
        System.Type type;

        [SerializeField]
        GameObject source;

        [SerializeField]
        string localVariableName;

        enum valueOrigins
        {
            Fixed,
            FindByName,
            FindByTag,
            LocalVariable
        }

        valueOrigins valueOrigin;

        [SerializeField]
        BetterAudioSource main;

        [SerializeField]
        public string name;


        public void SetCaller(BetterAudioSource caller)
        {
            main = caller;
        }

        public BetterValue(string n, BetterAudioSource caller, Component defaultValue)
        {
            value = defaultValue;
            name = n;
            main = caller;
            type = defaultValue.GetType();

        }


        public T GetValue<T>() where T : Component
        {
            switch (valueOrigin)
            {
                case valueOrigins.Fixed:
                    return (T)value;
                    break;
                case valueOrigins.FindByName:
                    value = GameObject.Find(localVariableName).GetComponent<T>();
                    return (T)value;
                    break;
                case valueOrigins.FindByTag:
                    value = GameObject.FindGameObjectWithTag(localVariableName).GetComponent<T>();
                    return (T)value;
                    break;
                case valueOrigins.LocalVariable:
                    LocalVariable variable;

                    if (main.localVariables.TryGetValue(localVariableName, out variable))
                    {
                        if (variable.type == typeof(T))
                        {
                            value = (T)variable.value;
                            return (T)value;
                        }
                        else
                        {
                            Debug.LogError("[BetterAudio M700] Localvariable is not the expected type");
                            return (T)value;
                        }
                    }
                    else
                    {
                        Debug.LogError("[BetterAudio M705] Cannot find local variable");
                        return (T)value;
                    }
                    break;
                default:
                    Debug.LogError("[BetterAudio M711] value's origins unexpected");
                    return (T)value;
                    break;
            }
        }

#if UNITY_EDITOR
        public void DrawGUI()
        {
            GUIStyle element = new GUIStyle();
            element.margin = new RectOffset(3, 3, 3, 3);

            GUIStyle frame = new GUIStyle();
            frame.margin = new RectOffset(10, 10, 10, 10);
            EditorGUILayout.BeginVertical(frame);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(name);
                    valueOrigin = (valueOrigins)EditorGUILayout.EnumPopup(valueOrigin);
                }
                EditorGUILayout.EndHorizontal();

                switch (valueOrigin)
                {
                    case valueOrigins.Fixed:
                        value = (Component)EditorGUILayout.ObjectField(value, type, true);

                        break;
                    case valueOrigins.FindByName:
                        localVariableName = EditorGUILayout.TextField("Name", localVariableName);
                        break;
                    case valueOrigins.FindByTag:
                        localVariableName = EditorGUILayout.TextField("Tag", localVariableName);
                        break;
                    case valueOrigins.LocalVariable:
                        localVariableName = EditorGUILayout.TextField("Local Variable Name", localVariableName);
                        break;
                    default:
                        break;
                }
            }
            EditorGUILayout.EndVertical();
        }
#endif

    }

    public class BetterValueNumber
    {
        [SerializeField]
        float value;
        [SerializeField]
        bool isInt;
        [SerializeField]
        float randomMin;
        [SerializeField]
        float randomMax;
        [SerializeField]
        BetterAudioSource sourceReference;
        [SerializeField]
        string localVariableName;

        enum valueOrigins
        {
            Fixed,
            Random,
            Reference,
            LocalVariable
        }
        [SerializeField]
        valueOrigins valueOrigin;
        [SerializeField]
        sourceProperties.sourcePropertiesList referenceProperty;

        [SerializeField]
        BetterAudioSource main;

        [SerializeField]
        public string name;


        public void SetCaller(BetterAudioSource caller)
        {
            main = caller;
        }



        public BetterValueNumber(string n, BetterAudioSource caller, float defaultValue, float _randomMin, float _randomMax, bool _isInt = false)
        {
            value = defaultValue;
            name = n;
            main = caller;
            isInt = _isInt;

            randomMin = _randomMin;
            randomMax = _randomMax;

        }

        public float GetValue()
        {
            switch (valueOrigin)
            {
                case valueOrigins.Fixed:
                    return value;
                    break;
                case valueOrigins.Random:
                     value = GetValueRandom();
                    return value;
                    break;
                case valueOrigins.Reference:
                    value = getValueByRef();
                    return value;
                    break;
                case valueOrigins.LocalVariable:
                    LocalVariable variable;

                    if (main.localVariables.TryGetValue(localVariableName, out variable))
                    {
                        if (variable.type == typeof(float) || variable.type == typeof(int))
                        {
                            value = (float)variable.value;
                            return value;
                        }
                        else
                        {
                            Debug.LogError("[BetterAudio M796] Localvariable is not the expected type");
                            return value;
                        }
                    }
                    else
                    {
                        Debug.LogError("[BetterAudio M802] Cannot find local variable");
                        return value;
                    }

                    break;
                default:
                    Debug.LogError("[BetterAudio M808] value's origins unexpected");
                    return value;
                    break;
            }
        }

        float GetValueRandom()
        {
            if (isInt)
            {
                return (int)Random.Range(randomMin, randomMax);
            }
            else
            {
                return Random.Range(randomMin, randomMax);
            }
        }

        float getValueByRef()
        {
            switch (referenceProperty)
            {
                case sourceProperties.sourcePropertiesList.Time:
                    return sourceReference.source.time;
                    break;
                case sourceProperties.sourcePropertiesList.ClipDuration:
                    return sourceReference.source.clip.length;
                    break;
                case sourceProperties.sourcePropertiesList.Volume:
                    return sourceReference.source.volume;
                    break;
                case sourceProperties.sourcePropertiesList.Pitch:
                    return sourceReference.source.pitch;
                    break;
                case sourceProperties.sourcePropertiesList.Stereo_Pan:
                    return sourceReference.source.panStereo;
                    break;
                case sourceProperties.sourcePropertiesList.Spatial_Blend:
                    return sourceReference.source.spatialBlend;
                    break;
                case sourceProperties.sourcePropertiesList.Reverb_Zone:
                    return sourceReference.source.reverbZoneMix;
                    break;
                case sourceProperties.sourcePropertiesList.Doppler_Level:
                    return sourceReference.source.dopplerLevel;
                    break;
                case sourceProperties.sourcePropertiesList.Spread:
                    return sourceReference.source.spread;
                    break;
                case sourceProperties.sourcePropertiesList.Min_Distance:
                    return sourceReference.source.minDistance;
                    break;
                case sourceProperties.sourcePropertiesList.Max_Distance:
                    return sourceReference.source.maxDistance;
                    break;
                default:
                    return value;
                    break;
            }
        }

    }


}
