using UnityEngine;

public class PuzzleBoutton : MonoBehaviour
{
    public bool isGripped;
    public bool isActivated;
    public Door door;

    [SerializeField] private CameraShakeScriptable sucessShakeScriptable;
    
    [Header("Enigme Donjon")] 
    public bool isEnigmeLustre;
    public Rigidbody lustreRb;
    
    private void Update()
    {
        if (isEnigmeLustre && isGripped)
        {
            lustreRb.isKinematic = false;
        }

        if (isGripped && !isActivated)
        {
            isActivated = true;
            GameManager.instance.RumblePulse(.2f, .4f, .1f, .6f);
            CameraShake.Instance.AddShakeEvent(sucessShakeScriptable);
            door.keysValid++;
            //Destroy(this);
        }
    }
}
