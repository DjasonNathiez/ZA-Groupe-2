using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace BetterAudio
{
    [CustomEditor(typeof(BetterAudioSource))]
    [CanEditMultipleObjects]
    public partial class BetterAudioSourceEditor : Editor
    {
        BetterAudioSourceResources ressources;

        BetterAudioStyle ThemeBackground;
        BetterAudioStyle ThemeForeground;

        BetterAudioSource comp;

        List<GUIContent> menuItems;
        List<BetterAudioGUI.Menu> menus;
        

        partial void PlusMenuItems();

        bool isPlus = false;

        partial void GetPlus();

        void GetMenuItems()
        {

            menuItems = new List<GUIContent>();
            menus = new List<BetterAudioGUI.Menu>();

            menuItems.Add(new GUIContent(ressources.IconTools, "Show Audio Source properties"));
            menus.Add(new BetterAudioGUI.Menu("Audio Source properties", DrawControlsMenu, ThemeBackground, ThemeForeground));

            menuItems.Add(new GUIContent(ressources.IconStart, "Show Start behaviours"));
            menus.Add(new BetterAudioGUI.Menu("Start Behaviours", DrawBehavioursMenu, ressources.StartBackground, ressources.StartForeground,
                i: new BetterAudioGUI.MenuInfo(ressources.StartBackground, ressources.StartForeground, BetterAudio.startBehaviours, comp.StartBehaviours)));

            menuItems.Add(new GUIContent(ressources.IconPlay, "Show Play behaviours"));
            menus.Add(new BetterAudioGUI.Menu("Play Behaviours", DrawBehavioursMenu, ressources.PlayBackground, ressources.PlayForeground,
                i: new BetterAudioGUI.MenuInfo(ressources.PlayBackground, ressources.PlayForeground, BetterAudio.playBehaviours, comp.PlayBehaviours)));

            menuItems.Add(new GUIContent(ressources.IconUpdate, "Show Update behaviours"));
            menus.Add(new BetterAudioGUI.Menu("Update Behaviours", DrawBehavioursMenu, ressources.UpdateBackground, ressources.UpdateForeground,
                i: new BetterAudioGUI.MenuInfo(ressources.UpdateBackground, ressources.UpdateForeground, BetterAudio.updateBehaviours, comp.UpdateBehaviours)));

            menuItems.Add(new GUIContent(ressources.IconEnd, "Show End behaviours"));
            menus.Add(new BetterAudioGUI.Menu("End Behaviours", DrawBehavioursMenu, ressources.EndBackground, ressources.EndForeground,
                i: new BetterAudioGUI.MenuInfo(ressources.EndBackground, ressources.EndForeground, BetterAudio.endBehaviours, comp.EndBehaviours)));

            PlusMenuItems();
        }

        private void OnEnable()
        {
            ressources = BetterAudioGUI.Styles;

            comp = (BetterAudioSource)target;
            GetPlus();

            if (EditorGUIUtility.isProSkin)
            {
                ThemeBackground = ressources.DarkThemeBackground;
                ThemeForeground = ressources.DarkThemeForeground;
            }
            else
            {
                ThemeBackground = ressources.LightThemeBackground;
                ThemeForeground = ressources.LightThemeForeground;
            }


            GetMenuItems();
        }

        Rect FullRect;
        Rect ControlRect;
        float width;


        public override void OnInspectorGUI()
        {
            width = EditorGUIUtility.currentViewWidth - 60;

            Rect CurrentRect;
            CurrentRect = EditorGUILayout.BeginHorizontal(ThemeBackground.toGUI(), GUILayout.Width(width));
            {
                CurrentRect = EditorGUILayout.BeginVertical();
                {
                    DrawHead();

                    if (comp.Error != "")
                    {
                        EditorGUILayout.BeginVertical(ThemeForeground.toGUI());
                        {
                            GUIStyle er = new GUIStyle(EditorStyles.boldLabel);
                            er.normal.textColor = Color.red;
                            er.alignment = TextAnchor.MiddleCenter;
                            er.fontSize = 16;

                            
                            EditorGUILayout.LabelField(comp.Error, er);
                            
                            if (GUILayout.Button("Clear"))
                            {
                                comp.Error = "";
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }


                    EditorGUILayout.BeginHorizontal(ThemeForeground.toGUI());
                    {
                        GUISkin skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
                        int tmp = comp.selectedMenu;
                        comp.selectedMenu = GUILayout.Toolbar(comp.selectedMenu, menuItems.ToArray(), skin.button, GUILayout.Height(35), GUILayout.Width(width));
                        if (tmp != comp.selectedMenu)
                            SelectedBehaviour = 0;
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginVertical(ThemeForeground.toGUI());
                    {
                        DrawMenu(menus[comp.selectedMenu]);
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical(ThemeForeground.toGUI());
                    {
                        DrawCredits();
                    }
                    EditorGUILayout.EndVertical();

                }
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndHorizontal();
            if (CurrentRect.width > 0)
                FullRect = CurrentRect;
        }

        float HeadWidth = 0;
        public void DrawHead()
        {
            GUIStyle style = new GUIStyle();
            style.normal.background = (Texture2D)ressources.BetterAudioSourceLogo;

            HeadWidth = FullRect.width == 0 ? HeadWidth : (FullRect.width);
            float LogoWidth = Mathf.Min(HeadWidth, 200);

            style.margin = new RectOffset((int)(HeadWidth / 2f - LogoWidth / 2.75f), 0, (int)(0.05f * LogoWidth), 0);

            EditorGUILayout.BeginVertical(ThemeForeground.toGUI(), GUILayout.Height(LogoWidth * 0.5f*0.75f));
            EditorGUILayout.BeginVertical(style, GUILayout.Height(LogoWidth * 0.4f*0.75f), GUILayout.Width(LogoWidth*0.75f));
            {
                EditorGUILayout.Space();

                GUIStyle s = new GUIStyle();
                s.normal.background = ThemeForeground.style.normal.background;
                s.active.background = ThemeBackground.style.normal.background;
                s.margin = new RectOffset(5, 5, 5, 5);

            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        void DrawMenu(BetterAudioGUI.Menu menu)
        {
            GUIStyle background = menu.BackgroundStyle.toGUI();
            GUIStyle foreground = menu.ForegroundStyle.toGUI();

            EditorGUILayout.BeginHorizontal(background);
            {
                GUIStyle centered = new GUIStyle(EditorStyles.boldLabel);
                centered.alignment = TextAnchor.MiddleCenter;
                centered.fontSize = (int)(centered.fontSize * 1.3f);


                EditorGUILayout.LabelField(menu.Name, centered);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(background);
            {
                menu.BodyFunc.Invoke(menu.info);
            }
            EditorGUILayout.EndVertical();
        }

        bool bypass = false;
        bool properties = false;
        void DrawControlsMenu(BetterAudioGUI.MenuInfo info)
        {

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("AudioClip:", EditorStyles.boldLabel);
            comp.source.clip = (AudioClip)EditorGUILayout.ObjectField(comp.source.clip, typeof(AudioClip), false);
            EditorGUILayout.LabelField("AudioMixer Output:", EditorStyles.boldLabel);
            comp.source.outputAudioMixerGroup = (UnityEngine.Audio.AudioMixerGroup)EditorGUILayout.ObjectField(comp.source.outputAudioMixerGroup, typeof(UnityEngine.Audio.AudioMixerGroup), false);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Volume", EditorStyles.boldLabel);
            comp.source.volume = EditorGUILayout.Slider(comp.source.volume, 0, 1);

            EditorGUILayout.Space();
            comp.source.mute = GUILayout.Toggle(comp.source.mute, "Mute");
            comp.source.loop = GUILayout.Toggle(comp.source.loop, "Loop endlessly");
            EditorGUILayout.Space();

            bypass = EditorGUILayout.BeginFoldoutHeaderGroup(bypass, "Bypasses", BetterAudioGUI.Foldout(width * 0.9f, 20));
            if (bypass)
            {
                EditorGUILayout.BeginVertical(ThemeForeground.toGUI());
                comp.source.bypassEffects = GUILayout.Toggle(comp.source.bypassEffects, "Bypass Effects");
                comp.source.bypassListenerEffects = GUILayout.Toggle(comp.source.bypassListenerEffects, "Bypass Listener Effects");
                comp.source.bypassReverbZones = GUILayout.Toggle(comp.source.bypassReverbZones, "Bypass Reverb Zones");
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();

            comp.source.playOnAwake = GUILayout.Toggle(comp.source.playOnAwake, "Play On Awake");
            comp.EditorPlay = GUILayout.Toggle(comp.EditorPlay, "Play On Editor Awake");

            EditorGUILayout.Space();

            if (comp.source.clip)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical(ThemeForeground.toGUI());
                EditorGUILayout.Space();
                EditorGUILayout.Space();



                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                {
                    GUIStyle lore = new GUIStyle();
                    lore.fontSize = (int)(lore.fontSize * 0.6f);
                    lore.padding = new RectOffset(10, 10, 0, 0);

                    if (EditorGUIUtility.isProSkin)
                        lore.normal.textColor = Color.white;
                    else
                        lore.normal.textColor = Color.black;

                    GUILayout.Label(BetterAudio.SecondsFormat(comp.source.time), lore);
                    lore.alignment = TextAnchor.MiddleRight;
                    GUILayout.Label(BetterAudio.SecondsFormat(comp.source.clip.length), lore);

                }
                EditorGUILayout.EndHorizontal();


                Rect TimeLineRect = EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(45), GUILayout.ExpandHeight(true));
                {
                    EditorGUILayout.Space();

                    GUIStyle back = ressources.SliderBackground.toGUI();
                    back.fixedWidth = TimeLineRect.width - 10;
                    Rect pos = TimeLineRect;
                    pos.width = width * 0.75f;
                    pos.x += 0.1f * width;

                    float t = comp.source.time;

                    t = GUI.HorizontalSlider(pos, comp.source.time, 0, comp.source.clip.length);
                    if (t > 0 && t < comp.source.clip.length && comp.source.pitch == 1)
                        comp.source.time = t;

                    EditorGUILayout.Space();

                }
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                {
                    if (!comp.source.isPlaying)
                    {
                        if (GUILayout.Button("Play"))
                            comp.source.Play();
                    }
                    else
                    {

                        if (GUILayout.Button("Pause"))
                            comp.source.Pause();
                    }

                    if (GUILayout.Button("Stop"))
                        comp.source.Stop();
                }
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.Space();



                EditorGUILayout.EndVertical();
            }
            else if (!comp.source.clip)
            {
                EditorGUILayout.LabelField("No clip selected", EditorStyles.centeredGreyMiniLabel);
            }

            EditorGUILayout.Space();
            properties = EditorGUILayout.BeginFoldoutHeaderGroup(properties, "Advanced properties", BetterAudioGUI.Foldout(width * 0.9f, 20));
            if (properties)
            {
                EditorGUILayout.BeginVertical(ThemeForeground.toGUI());

                EditorGUILayout.LabelField("Priority", EditorStyles.boldLabel);
                comp.source.priority = EditorGUILayout.IntSlider(comp.source.priority, 0, 256);
                BetterAudioGUI.SliderLores("High", "Low");

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Pitch", EditorStyles.boldLabel);
                comp.source.pitch = EditorGUILayout.Slider(comp.source.pitch, -3, 2);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Stereon Pan", EditorStyles.boldLabel);
                comp.source.panStereo = EditorGUILayout.Slider(comp.source.panStereo, -1, 1);
                BetterAudioGUI.SliderLores("Left", "Right");

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Spatial Blend", EditorStyles.boldLabel);
                comp.source.spatialBlend = EditorGUILayout.Slider(comp.source.spatialBlend, 0, 1);
                BetterAudioGUI.SliderLores("2D", "3D");

                EditorGUILayout.LabelField("Reverb Zone Mix", EditorStyles.boldLabel);
                comp.source.reverbZoneMix = EditorGUILayout.Slider(comp.source.reverbZoneMix, 0, 1.1f);


                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space();
        }


        int SelectedBehaviour = 0;
        void DrawBehavioursMenu(BetterAudioGUI.MenuInfo info)
        {
            GUIStyle Background;
            GUIStyle Foreground;

            
                Background = info.background.toGUI();
                Foreground = info.foreground.toGUI();
            


            Behaviour[] sourceListe = info.behavioursList;
            BehavioursGroup group = info.group;

            


            foreach (BetterAudioBehaviour behaviour in group.items)
            {
                EditorGUILayout.BeginVertical(Foreground, GUILayout.MinHeight(25));
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(0.025f * width);
                        GUIStyle fold = new GUIStyle(EditorStyles.foldout);
                        fold.margin = new RectOffset(10, 0, 3, 5);
                        fold.fontSize = 13;
                        fold.normal.textColor = Foreground.normal.textColor;
                        fold.onNormal.textColor = Foreground.normal.textColor;
                        fold.fontStyle = FontStyle.Bold;


                        behaviour.IsShowed = EditorGUILayout.Foldout(behaviour.IsShowed, behaviour.GetName(), true, fold);
                        EditorGUILayout.Space();

                        GUIStyle DeleteButton = new GUIStyle(EditorStyles.miniTextField);

                        DeleteButton.margin = new RectOffset(0, 5, 5, 5);
                        DeleteButton.normal.textColor = Color.red;

                        if (GUILayout.Button("Remove", DeleteButton))
                        {
                            group.Remove(behaviour);
                            EditorUtility.SetDirty(comp);
                            break;
                        }

                    }
                    EditorGUILayout.EndHorizontal();

                    if (behaviour.IsShowed)
                    {
                        Rect CurrentRect = EditorGUILayout.BeginVertical(Background);
                        {
                            GUILayout.Space(0.025f * width);
                            EditorGUILayout.BeginVertical(BetterAudioGUI.MakeGUIStyle(Color.clear, Color.clear, margin: ((int)(0.025f * width))));
                            behaviour.GUI(CurrentRect);
                            EditorGUILayout.EndVertical();
                            GUILayout.Space(0.025f * width);
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginHorizontal(Background);
            {
                List<string> AvailableBehaviours = new List<string>();
                foreach (Behaviour b in sourceListe)
                {
                    AvailableBehaviours.Add(b.Name);
                }

                SelectedBehaviour = EditorGUILayout.Popup(SelectedBehaviour, AvailableBehaviours.ToArray());
                if (GUILayout.Button("Add Behaviour"))
                {
                    BetterAudioBehaviour newBehaviour = (BetterAudioBehaviour)System.Activator.CreateInstance(sourceListe[SelectedBehaviour].type);
                    newBehaviour.Init(comp);
                    if (group.Add(newBehaviour))
                    {
                        EditorUtility.SetDirty(comp);
                    }
                    else
                    {
                        comp.Error = "Behaviour already present";
                    }
                }

            }
            EditorGUILayout.EndHorizontal();

        }


        
        void DrawCredits()
        {
            EditorGUILayout.LabelField("All rights reserved to Raphaël Horion", EditorStyles.centeredGreyMiniLabel);
            if (!isPlus)
            {
                if(GUILayout.Button("Upgrade to Plus version !"))
                    Application.OpenURL("https://assetstore.unity.com/packages/tools/audio/better-audio-source-plus-186483");
            }
            else
            {
                EditorGUILayout.LabelField("Thanks for buying Plus version !", EditorStyles.centeredGreyMiniLabel);
            }
            
        }

    }//Endclass

}//EndNamespace

