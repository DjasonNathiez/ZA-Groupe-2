using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Taupe : MonoBehaviour
{
    public TapeTaupeArcade TapeTaupeArcade;
    public int number;
    public GameObject t_prefab;
    public Transform t_pos;
    public bool grappable;

    public ParticleSystem hitPs;
    public ParticleSystem appearPs;
    
    public void TaupeHit(bool grapped)
    {
        if (grapped && grappable || !grapped)
        {
            TapeTaupeArcade.TaupeIsTaped(number);
            Quaternion t_rot = Quaternion.Euler(25,-45,0);
            GameObject gameObject = Instantiate(t_prefab, t_pos.position, t_rot);

            GameManager.instance.RumbleConstant(.1f, .1f, .5f);

            gameObject.GetComponentInChildren<TextMeshPro>().text = TapeTaupeArcade.score + " / " + TapeTaupeArcade.goal;;
        
            Destroy(gameObject, 5);
            hitPs.Play();   
        }
    }
}
