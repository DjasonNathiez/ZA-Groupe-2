using UnityEngine;

public class PuzzleBoutton : MonoBehaviour
{
    public bool isGripped;
    public bool isActivated;
    public bool multiactivate;
    public Door door;

    [SerializeField] private CameraShakeScriptable sucessShakeScriptable;
    
    [Header("Enigme Donjon")] 
    public bool isEnigmeLustre;
    public Rigidbody lustreRb;
    
    private void Update()
    {

        if (isGripped && !isActivated)
        {
            isActivated = true;
            GameManager.instance.RumblePulse(.2f, .4f, .1f, .6f);
            CameraShake.Instance.AddShakeEvent(sucessShakeScriptable);
            door.keysValid++;
            //Destroy(this);
        }
        
        

        if (isGripped && multiactivate)
        {
            if (PlayerManager.instance.rope.rewinding)
            {
                isActivated = false;
                isGripped = false;
                door.keysValid--;
            }
        }
    }
}
