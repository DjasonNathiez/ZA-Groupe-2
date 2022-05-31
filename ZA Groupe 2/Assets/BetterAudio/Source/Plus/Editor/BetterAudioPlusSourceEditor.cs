using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BetterAudio
{
    partial class BetterAudioSourceEditor
    {
        partial void PlusMenuItems()
        {
            menuItems.Add(new GUIContent(ressources.IconCollider, "Show Collider behaviours"));
            menus.Add(new BetterAudioGUI.Menu("Collider Behaviours", DrawPlusBehavioursMenu, ressources.ColliderBackground, ressources.ColliderForeground,
                i: new BetterAudioGUI.MenuInfo(ressources.ColliderBackground, ressources.ColliderForeground, BetterAudio.colliderBehaviours, comp.ColliderBehaviours, col:true)));

            menuItems.Add(new GUIContent(ressources.IconEvent, "Show Event behaviours"));
            menus.Add(new BetterAudioGUI.Menu("Event Behaviours", DrawPlusBehavioursMenu, ressources.EventBackground, ressources.EventForeground,
                i: new BetterAudioGUI.MenuInfo(ressources.EventBackground, ressources.EventForeground, BetterAudio.eventBehaviours, comp.EventBehaviours, ev:true)));
        }


        void DrawPlusBehavioursMenu(BetterAudioGUI.MenuInfo info)
        {
            GUIStyle Background;
            GUIStyle Foreground;


            Background = info.background.toGUI();
            Foreground = info.foreground.toGUI();



            Behaviour[] sourceListe = info.behavioursList;
            BehavioursGroup group = info.group;

            if (info.isCollider)
            {
                Collider col;
                Collider2D col2D;
                if (!comp.TryGetComponent<Collider>(out col) && !comp.TryGetComponent<Collider2D>(out col2D))
                {
                    EditorGUILayout.BeginVertical(Background);
                    {
                        EditorGUILayout.LabelField("Collider Behaviours need a collider to work");
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add collider"))
                            comp.gameObject.AddComponent<SphereCollider>();

                        if (GUILayout.Button("Add collider2D"))
                            comp.gameObject.AddComponent<CircleCollider2D>();

                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                }
            }


            foreach (BetterAudioBehaviour behaviour in group.items)
            {
                EditorGUILayout.BeginVertical(Foreground, GUILayout.MinHeight(25));
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            GUILayout.Space(0.025f * width);
                            GUIStyle fold = new GUIStyle(EditorStyles.foldout);
                            fold.margin = new RectOffset(10, 0, 3, 5);
                            fold.fontSize = 13;
                            fold.normal.textColor = Foreground.normal.textColor;
                            fold.onNormal.textColor = Foreground.normal.textColor;
                            fold.fontStyle = FontStyle.Bold;


                            behaviour.IsShowed = EditorGUILayout.Foldout(behaviour.IsShowed, behaviour.GetName(), true, fold);
                            
                            if (info.isCollider)
                                behaviour.mode = (ColliderMode)EditorGUILayout.EnumFlagsField("Collider Detection", behaviour.mode);

                            if(info.isEvent)
                                behaviour.EventName = EditorGUILayout.TextField("Event Name", behaviour.EventName);
                            EditorGUILayout.Space();

                        }
                        EditorGUILayout.EndVertical();

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

        partial void GetPlus()
        {
            isPlus = true;
        }
    }
}