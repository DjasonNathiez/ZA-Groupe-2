using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BetterAudio
{
    public static class BetterAudioGUI
    {
        public class Menu
        {
            public System.Action<MenuInfo> BodyFunc;
            public string Name;
            public BetterAudioStyle BackgroundStyle;
            public BetterAudioStyle ForegroundStyle;
            public MenuInfo info;



            public Menu(string n, System.Action<MenuInfo> ac, BetterAudioStyle back, BetterAudioStyle fore, MenuInfo i = null)
            {
                Name = n;
                BodyFunc = ac;
                BackgroundStyle = back;
                ForegroundStyle = fore;
                info = i;

            }
        }

        public class MenuInfo
        {
            public Behaviour[] behavioursList;
            public BehavioursGroup group;
            public BetterAudioStyle background;
            public BetterAudioStyle foreground;

            public bool isCollider;
            public bool isEvent;

            public MenuInfo(BetterAudioStyle back, BetterAudioStyle fore, Behaviour[] l, BehavioursGroup g, bool col = false, bool ev = false)
            {
                background = back;
                foreground = fore;
                behavioursList = l;
                group = g;
                isCollider = col;
                isEvent = ev;
            }

        }

        public static Texture2D MakeTexture(int width, int height, Color color, bool fade = true, int fadeDist = 1)
        {
            Color[] pix = new Color[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color c = color;

                    if (fade)
                    {
                        if (x < fadeDist || x >= width - fadeDist || y < fadeDist || x >= height - fadeDist)
                            c.a *= 0.5f;
                    }

                    pix[x + y * width] = c;
                }
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        public static Texture2D ClearTexture = MakeTexture(100, 100, Color.clear, fade: false);

        public static BetterAudioSourceResources Styles
        {
            get
            {
                var ressource = Resources.Load<BetterAudioSourceResources>("BetterAudioSourceResources");
                return ressource;
            }
        }

        public static GUIStyle Foldout(float w, int m)
        {
            GUIStyle s = new GUIStyle(EditorStyles.foldoutHeader);
            // s.fixedWidth = w;
            s.margin = new RectOffset(m, m, 0, 0);

            return s;
        }

        public static void SliderLores(GUIContent left, GUIContent right)
        {
            EditorGUILayout.BeginHorizontal();

            GUIStyle lore = new GUIStyle(EditorStyles.miniLabel);
            lore.margin = new RectOffset(1, 50, 1, 1);
            GUILayout.Label(left, lore);

            lore.alignment = TextAnchor.UpperRight;
            GUILayout.Label(right, lore);
            EditorGUILayout.EndHorizontal();
        }

        public static void SliderLores(string left, string right)
        {
            EditorGUILayout.BeginHorizontal();

            GUIStyle lore = new GUIStyle(EditorStyles.miniLabel);
            lore.margin = new RectOffset(1, 50, 1, 1);
            GUILayout.Label(left, lore);

            lore.alignment = TextAnchor.UpperRight;
            GUILayout.Label(right, lore);
            EditorGUILayout.EndHorizontal();
        }

        

        public static GUIStyle MakeGUIStyle(Color TextColor, Color BackgroundColor, TextAnchor aligment = TextAnchor.MiddleLeft, int fontSize = 14, int margin = 5, RectOffset marginRect = null)
        {
            GUIStyle gui = new GUIStyle();

            gui.normal.textColor = TextColor;
            gui.normal.background = MakeTexture(100, 100, BackgroundColor);
            gui.alignment = aligment;
            gui.fontSize = fontSize;
            if (marginRect != null)
                gui.margin = marginRect;
            else
                gui.margin = new RectOffset(margin, margin, margin, margin);

            return gui;
        }


    }

    [System.Serializable]
    public partial class BetterAudioStyle
    {
        [SerializeField]
        public Color backgroundColor = Color.clear;
        [SerializeField]
        public Color textColor = Color.black;

        [SerializeField]
        public int fontSize = 16;

        [SerializeField]
        public RectOffset margins;
        [SerializeField]
        public RectOffset paddins;

        [SerializeField]
        public bool border = false;

        [SerializeField]
        public int FadeDist = 1;

        public GUIStyle style;

        public GUIStyle toGUI()
        {

            if (style == null || true)
            {
                GUIStyle s = new GUIStyle();
                s.normal.background = BetterAudioGUI.MakeTexture(100, 100, backgroundColor, fade: border, fadeDist: FadeDist);
                if (border)
                    s.border = new RectOffset(2, 2, 2, 2);

                s.margin = margins;
                s.padding = paddins;

                s.normal.textColor = textColor;
                s.fontSize = fontSize;
                style = s;
            }

            return style;
        }
    }


}