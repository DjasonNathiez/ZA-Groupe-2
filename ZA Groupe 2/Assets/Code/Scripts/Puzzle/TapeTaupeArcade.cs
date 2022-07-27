using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TapeTaupeArcade : MonoBehaviour
{
    [SerializeField] private GameObject[] taupes;
    [SerializeField] private Light[] taupesLights;
    [SerializeField] private bool[] activated;
    public bool active;
    public GameObject grille;
    public float taupeTimer;
    public float taupeDelay;
    public GameObject light;
    public GameObject directionalOn;
    public GameObject directionalOff;
    public float timer = 50;
    public TextMeshPro[] timerText;
    public int score;
    public AnimationCurve rate;
    public int goal;
    public GameObject reward;
    public Door doorToOpen;
    public int storyState;

    public ParticleSystem VictoryFX;

    private void Start()
    {
        for (int i = 0; i < taupes.Length; i++)
        {
            taupes[i].GetComponent<Taupe>().TapeTaupeArcade = this;
            taupes[i].GetComponent<Taupe>().number = i;
            taupesLights[i].enabled = false;
        }
    }

    private void Update()
    {
        for (int i = 0; i < taupes.Length; i++)
        {
            if (activated[i] == true)
            {
                taupes[i].transform.localPosition = Vector3.Lerp(taupes[i].transform.localPosition,new Vector3(taupes[i].transform.localPosition.x, taupes[i].transform.localPosition.y, -0.1f),5*Time.deltaTime);
                taupesLights[i].enabled = true;
            }
            else
            {
                taupes[i].transform.localPosition = Vector3.Lerp(taupes[i].transform.localPosition,new Vector3(taupes[i].transform.localPosition.x, taupes[i].transform.localPosition.y, 5f),5*Time.deltaTime);
                taupesLights[i].enabled = false;
            }
        }

        if (active)
        {
            grille.transform.localPosition = Vector3.Lerp(grille.transform.localPosition, Vector3.zero, 5 * Time.deltaTime);
        }
        else
        {
            grille.transform.localPosition = Vector3.Lerp(grille.transform.localPosition, new Vector3(0,-1.75f,0), 5 * Time.deltaTime);
        }

        if (active)
        {
            if (taupeTimer < 0)
            {
                int taupeUnactivated = 0;
                foreach (bool activation in activated)
                {
                    if (!activation) taupeUnactivated++;
                }

                if (taupeUnactivated > 0)
                {
                    if (taupeUnactivated < 7)
                    {
                        int rngbis = Random.Range(0, activated.Length);
                        activated[rngbis] = false;
                    }
                    int rng = Random.Range(0, activated.Length);
                    for (int i = 0; i < activated.Length; i++)
                    {
                        if (activated[rng])
                        {
                            rng = (rng + 1) % activated.Length;
                        }
                        else
                        {
                            break;
                        }
                    }

                    activated[rng] = true;
                    taupes[rng].GetComponent<Taupe>().appearPs.Play();
                }
            
                taupeTimer = rate.Evaluate(1 - timer/50);
            }
            else
            {
                taupeTimer -= Time.deltaTime;
            }
            
            if (timer <= 0)
            {
                StopTapeTaupe();
            }
            else
            {
                timer -= Time.deltaTime;
            }

            for (int i = 0; i < timerText.Length; i++)
            {
                timerText[i].text = Mathf.CeilToInt(timer).ToString();
            }
        }
    }

    public void StartTapeTaupe()
    {
        if (!active)
        {
            active = true;
            light.SetActive(true);
            directionalOff.SetActive(true);
            directionalOn.SetActive(false);
            timer = 50;
            score = 0;
            AudioManager.instance.SetMusic("Arcade");

            timerText[0].transform.GetComponent<Animation>().Play();
            if(timerText.Length > 1) timerText[1].transform.GetComponent<Animation>().Play();
            
        }
    }
    
    public void StopTapeTaupe()
    {
        if (active)
        {
            for (int i = 0; i < activated.Length; i++)
            {
                activated[i] = false;
            }
            active = false;
            light.SetActive(false);
            directionalOff.SetActive(false);
            directionalOn.SetActive(true);
            if (score >= goal)
            {
                VictoryFX.Play();
                if(PlayerManager.instance.storyState < storyState) PlayerManager.instance.storyState = storyState;
                if(reward) reward.SetActive(true);
                if(doorToOpen) doorToOpen.keysValid++;
            }
            
            AudioManager.instance.SetMusic(GameManager.instance.dungeonEnded ? "Parc_2" : "Parc_1");
        }
    }
    
    public void TaupeIsTaped(int number)
    {
        if (activated[number])
        {
            activated[number] = false;
            score++;
        }
    }
}
