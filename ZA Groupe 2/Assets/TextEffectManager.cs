using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class TextEffectManager : MonoBehaviour
{
    public TMP_Text textElement;
    public bool reset;
    public bool next;
    public int showedCharIndex = 0;
    [SerializeField] private float speedOfShowing = 0.05f;
    [SerializeField] private float timeStamp;
    [SerializeField] private float[] charBasedHeight;
    [SerializeField] private int dialogueIndex;
    public DialogueLine[] dialogue;
    
    public void ShowText()
    {
        textElement.text = dialogue[dialogueIndex].text;
        textElement.ForceMeshUpdate();
        showedCharIndex = 0;
        charBasedHeight = new float[textElement.textInfo.characterCount];
        Debug.Log(textElement.textInfo.characterCount);
        for (int i = 0; i < charBasedHeight.Length; i++)
        {
            charBasedHeight[i] = -1f;
        }
        timeStamp = speedOfShowing;
    }
    public void NextText()
    {
        dialogueIndex++;
        textElement.text = dialogue[dialogueIndex].text;
        textElement.ForceMeshUpdate();
        showedCharIndex = 0;
        charBasedHeight = new float[textElement.textInfo.characterCount];
        Debug.Log(textElement.textInfo.characterCount);
        for (int i = 0; i < charBasedHeight.Length; i++)
        {
            charBasedHeight[i] = -1f;
        }
        timeStamp = speedOfShowing;
    }
    void Update()
    {

        var textInfo = textElement.textInfo;
        textElement.ForceMeshUpdate();
        
        Debug.Log("OK C " + textInfo.characterInfo[0].scale);

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
            {
                continue;
            }
            
            Vector3[] verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            Color32[] colors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;

            if (dialogue.Length > 0)
            {
                for (int e = 0; e < dialogue[dialogueIndex].m_textEffects.Length; e++)
                {
                    if (i >= dialogue[dialogueIndex].m_textEffects[e].firstCharIndex && i <= dialogue[dialogueIndex].m_textEffects[e].lastCharIndex)
                    {
                        Vector3 center = (verts[charInfo.vertexIndex + 0] + verts[charInfo.vertexIndex + 2])/2;
                        for (int v = 0; v < 4; v++)
                        {
                            var original = verts[charInfo.vertexIndex + v];
                            verts[charInfo.vertexIndex + v] = ApplyEffectToVertex(original,dialogue[dialogueIndex].m_textEffects[e].effectType,center,dialogue[dialogueIndex].m_textEffects[e].speed,dialogue[dialogueIndex].m_textEffects[e].width);
                            colors[charInfo.vertexIndex + v] = ApplyColorEffectToVertex(
                                dialogue[dialogueIndex].m_textEffects[e].colorEffectType,
                                dialogue[dialogueIndex].m_textEffects[e].firstColor,
                                dialogue[dialogueIndex].m_textEffects[e].secondColor,
                                dialogue[dialogueIndex].m_textEffects[e].colorSpeed,
                                dialogue[dialogueIndex].m_textEffects[e].colorWidth,original);
                        }
                        break;
                    }   
                }   
                
                for (int v = 0; v < 4; v++)
                {
                    verts[charInfo.vertexIndex + v] = GetVertex(verts[charInfo.vertexIndex + v],i);
                    Color32 original = colors[charInfo.vertexIndex + v];
                    colors[charInfo.vertexIndex + v] = Color32.Lerp(new Color32(original.r, original.g, original.b, 0), original, charBasedHeight[i] + 1);
                }
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textElement.UpdateGeometry(meshInfo.mesh,i);
            textElement.UpdateVertexData();
        }

        if (showedCharIndex < textInfo.characterCount)
        {
            timeStamp -= Time.deltaTime;
            if (timeStamp <= 0)
            {
                timeStamp = speedOfShowing;
                showedCharIndex++;
            }   
        }
    }

    public Vector3 GetVertex(Vector3 original,int CharIndex)
    {
        Vector3 product = new Vector3(original.x, original.y + charBasedHeight[CharIndex], original.z);
        if (CharIndex < showedCharIndex)
        {
            charBasedHeight[CharIndex] = Mathf.Lerp(charBasedHeight[CharIndex], 0, 0.02f);
        }
        return product;
    }
    
    public Vector3 ApplyEffectToVertex(Vector3 original,effectTypeEnum effect,Vector3 center,float speed,float width)
    {
        Vector3 product = original;
        switch (effect)
        {
            case effectTypeEnum.WAVE:
                product = original + new Vector3(0, Mathf.Sin(Time.time * speed + original.x * width) * 0.15f, 0);
                break;
            case effectTypeEnum.RANDOM:
                product = original + new Vector3(Random.Range(-width,width),Random.Range(-width,width),0);
                break;
            case effectTypeEnum.ZOOM:
                product = (center + (product - center) * Mathf.Sin(Time.time * speed + center.x * width)* 0.2f)+ (product - center)*1.1f;
                break;
            case effectTypeEnum.NONE:
                product = original;
                break;
        }
        return product;
    }
    
    public Color32 ApplyColorEffectToVertex(colorEffectTypeEnum effect,Color32 first,Color32 second,float speed,float width,Vector3 original)
    {
        Color32 product = first;
        switch (effect)
        {
            case colorEffectTypeEnum.HORIZONTALGRADIANT:
                product = Color32.Lerp(first, second, Mathf.Sin(Time.time * speed + original.x * width) * 0.5f + 0.5f);
                break;
            case colorEffectTypeEnum.VERTICALGRADIANT:
                product = Color32.Lerp(first, second, Mathf.Sin(Time.time * speed + original.y * width) * 0.5f + 0.5f);
                break;
            case colorEffectTypeEnum.RAINBOW:
                product = Color.HSVToRGB(Mathf.Lerp(0, 1, Time.time * speed % 1), 1, 1);
                break;
            case colorEffectTypeEnum.NONE:
                product = first;
                break;
        }
        return product;
    }
}

[Serializable] public class TextEffect
{
    public effectTypeEnum effectType;
    public float speed = 1;
    public float width = 1;
    public colorEffectTypeEnum colorEffectType;
    public float colorSpeed = 1;
    public float colorWidth = 1;
    public Color32 firstColor = Color.white;
    public Color32 secondColor = Color.white;
    public int firstCharIndex;
    public int lastCharIndex;

}

[Serializable] public class DialogueLine
{
    [TextArea] public string text;
    public TextEffect[] m_textEffects;
    public bool modifyCameraPosition;
    public Vector3 positionCamera;
    public Vector3 angleCamera;
    public float zoom;
}

[Serializable]
public enum effectTypeEnum
{
    WAVE,
    RANDOM,
    ZOOM,
    NONE,
}

[Serializable]
public enum colorEffectTypeEnum
{
    NONE,
    HORIZONTALGRADIANT,
    VERTICALGRADIANT,
    RAINBOW,
}