using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TextEffectManager : MonoBehaviour
{
    public TMP_Text textElement;
    public TMP_Text talkingCharacter;
    public bool isDialoguing;
    public int showedCharIndex = 0;
    [SerializeField] private float speedOfShowing = 0.05f;
    [SerializeField] private float timeStamp;
    [SerializeField] private float[] charBasedHeight;
    public int dialogueIndex;
    public DialogueLine[] dialogue;
    public List<Color32> colors;
    
    public void ShowText()
    {
        Debug.Log("ShowText Void Launching");

        switch (GameManager.instance.language)
        {
            case GameManager.Language.FRENCH:
                textElement.text = dialogue[dialogueIndex].frenchText;
                break;
            
            case GameManager.Language.ENGLISH:
                textElement.text = dialogue[dialogueIndex].englishText;
                break;
        }
        
        textElement.ForceMeshUpdate();
        showedCharIndex = 0;
        talkingCharacter.text = dialogue[dialogueIndex].speakingName;
        Debug.Log("Before Error");
        charBasedHeight = new float[textElement.textInfo.characterCount];
        Debug.Log("4 Line Function ShowText");
        //Debug.Log(charBasedHeight);
        for (int i = 0; i < charBasedHeight.Length; i++)
        {
            charBasedHeight[i] = -1f;
        }
        UpdateColor();
        timeStamp = speedOfShowing;
    }
    public void NextText()
    {
        if (dialogueIndex < dialogue.Length-1)
        {
            dialogueIndex++;
            switch (GameManager.instance.language)
            {
                case GameManager.Language.FRENCH:
                    textElement.text = dialogue[dialogueIndex].frenchText;
                    break;
            
                case GameManager.Language.ENGLISH:
                    textElement.text = dialogue[dialogueIndex].englishText;
                    break;
            }
            textElement.ForceMeshUpdate();
            showedCharIndex = 0;
            talkingCharacter.text = dialogue[dialogueIndex].speakingName;
            charBasedHeight = new float[textElement.textInfo.characterCount];
            for (int i = 0; i < charBasedHeight.Length; i++)
            {
                charBasedHeight[i] = -1f;
            }
            UpdateColor();
            timeStamp = speedOfShowing;   
        }
    }
    void Update()
    {
        if (isDialoguing)
        {
            
            var textInfo = textElement.textInfo;

            if (showedCharIndex < textInfo.characterCount)
            {
                timeStamp -= Time.deltaTime;
                if (timeStamp <= 0)
                {
                    for (int i = 0; i < Mathf.FloorToInt(Mathf.Abs(timeStamp) / speedOfShowing) ; i++)
                    {
                        showedCharIndex++;
                        
                    }
                    timeStamp = speedOfShowing;
                }   
            }
            
            
            for (int i = 0; i < textInfo.characterCount; i++)
            {

                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                {
                    continue;
                }
                Color32[] newcolors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
                if (i < showedCharIndex)
                {
                    charBasedHeight[i] = Mathf.Lerp(charBasedHeight[i], 0, 5 * Time.deltaTime);
                }
                if (dialogue.Length > 0)
                {
                    for (int v = 0; v < 4; v++)
                    {
                        Color32 original = colors[charInfo.vertexIndex + v];
                        newcolors[charInfo.vertexIndex + v] = Color32.Lerp(new Color32(original.r, original.g, original.b, 0), original, charBasedHeight[i] + 1);
                    }
                }
            }
        
            textElement.UpdateVertexData();
        }
    }

    public void UpdateColor()
    {
        var textInfo = textElement.textInfo;
        colors.Clear();
        
        for (int j = 0; j < textInfo.characterCount *4; j++)
        {
            colors.Add(new Color32(255, 255, 255, 255));
        }

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
            {
                continue;
            }
            Color32[] newcolors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;

            if (dialogue.Length > 0)
            {
                for (int e = 0; e < dialogue[dialogueIndex].textEffects.Length; e++)
                {
                    if (i >= dialogue[dialogueIndex].textEffects[e].firstCharIndex && i <= dialogue[dialogueIndex].textEffects[e].lastCharIndex)
                    {
                        for (int v = 0; v < 4; v++)
                        {
                            colors[charInfo.vertexIndex + v] = dialogue[dialogueIndex].textEffects[e].firstColor;
                            
                            Debug.Log("OkFrero " + dialogue[dialogueIndex].textEffects[e].firstColor);
                        }
                        break;
                    }   
                }   
                
                for (int v = 0; v < 4; v++)
                {
                    Color32 original = colors[charInfo.vertexIndex + v];
                    newcolors[charInfo.vertexIndex + v] = Color32.Lerp(new Color32(original.r, original.g, original.b, 0), original, charBasedHeight[i] + 1);
                }
            }
        }
        
        
        textElement.UpdateVertexData();
        
    }

    public Vector3 GetVertex(Vector3 original,int charIndex)
    {
        Vector3 product = new Vector3(original.x, original.y + charBasedHeight[charIndex], original.z);
        if (charIndex < showedCharIndex)
        {
            charBasedHeight[charIndex] = Mathf.Lerp(charBasedHeight[charIndex], 0, 0.02f);
        }
        return product;
    }
    
    public Vector3 ApplyEffectToVertex(Vector3 original,EffectTypeEnum effect,Vector3 center,float speed,float width,float height)
    {
        Vector3 product = original;
        switch (effect)
        {
            case EffectTypeEnum.WAVE:
                product = original + new Vector3(0, Mathf.Sin(Time.time * speed + original.x * width) * height, 0);
                break;
            case EffectTypeEnum.RANDOM:
                product = original + new Vector3(Random.Range(-width,width),Random.Range(-width,width),0);
                break;
            case EffectTypeEnum.ZOOM:
                product = (center + (product - center) * Mathf.Sin(Time.time * speed + center.x * width)* 0.2f)+ (product - center)*1.1f;
                break;
            case EffectTypeEnum.NONE:
                product = original;
                break;
        }
        return product;
    }
    
    public Color32 ApplyColorEffectToVertex(ColorEffectTypeEnum effect,Color32 first,Color32 second,float speed,float width,Vector3 original)
    {
        Color32 product = first;
        switch (effect)
        {
            case ColorEffectTypeEnum.HORIZONTALGRADIANT:
                product = Color32.Lerp(first, second, Mathf.Sin(Time.time * speed + original.x * width) * 0.5f + 0.5f);
                break;
            case ColorEffectTypeEnum.VERTICALGRADIANT:
                product = Color32.Lerp(first, second, Mathf.Sin(Time.time * speed + original.y * width) * 0.5f + 0.5f);
                break;
            case ColorEffectTypeEnum.RAINBOW:
                product = Color.HSVToRGB(Mathf.Lerp(0, 1, Time.time * speed % 1), 1, 1);
                break;
            case ColorEffectTypeEnum.NONE:
                product = first;
                break;
        }
        return product;
    }
}

[Serializable] public class TextEffect
{
    public EffectTypeEnum effectType;
    public float speed = 1;
    public float width = 1;
    public float height = 1;
    public ColorEffectTypeEnum colorEffectType;
    public float colorSpeed = 1;
    public float colorWidth = 1;
    public Color32 firstColor = Color.white;
    public Color32 secondColor = Color.white;
    public int firstCharIndex;
    public int lastCharIndex;

}

[Serializable] public class DialogueLine
{
    public string speakingName;
    [FormerlySerializedAs("text")] [TextArea] public string frenchText;
    [TextArea] public string englishText;
    [FormerlySerializedAs("m_textEffects")] public TextEffect[] textEffects;
    public bool modifyCameraPosition;
    public Vector3 positionCamera;
    public Vector3 angleCamera;
    public float zoom;
    public float speedOfPan;
    public bool cinematicAngleOnly;
    public float durationIfAuto;
    public GameObject toActivate;
    public Animator[] animations;
    public string[] clips;
    public float durationBetweenVoiceSound;
    public int voiceCombo;
}

[Serializable]
public enum EffectTypeEnum
{
    WAVE,
    RANDOM,
    ZOOM,
    NONE,
}

[Serializable]
public enum ColorEffectTypeEnum
{
    NONE,
    HORIZONTALGRADIANT,
    VERTICALGRADIANT,
    RAINBOW,
}