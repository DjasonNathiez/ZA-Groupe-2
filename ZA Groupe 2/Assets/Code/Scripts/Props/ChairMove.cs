using UnityEngine;

public class ChairMove : MonoBehaviour
{
    public GameObject go;
    public float speed = 8f;

    public Vector2 minMaxRotation;

//private value
    public float rotationYchair;

    private void Start()
    {
        rotationYchair = go.transform.rotation.y;
    }

    void ChairRotation()
    {
        Quaternion rotationObj = go.transform.rotation;
    }
}