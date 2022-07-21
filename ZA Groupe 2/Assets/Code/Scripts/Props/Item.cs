using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public float valuePercentage;
    public string affectedValue;
    public bool distributed;

    public float rotationSpeed = 20f;
    public float amplitude = 0.2f;
    public float floatSpeed = 2f;
    public float height;
    public bool goToPos;
    public Vector3 pos;

    
    public Vector3 posOffset;
    private Vector3 tempPos;
    public PnjDialoguesManager PnjDialoguesManager;

    void Awake()
    {
        if(!goToPos) posOffset = transform.position;
        else posOffset = pos;
        if(PnjDialoguesManager) PnjDialoguesManager.dialogue[0].positionCamera = posOffset;
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0f, Time.unscaledDeltaTime * rotationSpeed, 0f), Space.World);
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedUnscaledTime * Mathf.PI * floatSpeed) * amplitude;
        transform.position = Vector3.Lerp(transform.position, posOffset + Vector3.up * height, 5 * Time.deltaTime);
    }
}
