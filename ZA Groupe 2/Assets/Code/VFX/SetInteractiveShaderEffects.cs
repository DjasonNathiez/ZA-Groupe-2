using UnityEngine;

public class SetInteractiveShaderEffects : MonoBehaviour
{
    [SerializeField] private RenderTexture rt;

    [SerializeField] private Transform target;

    // Start is called before the first frame update
    private void Awake()
    {
        Shader.SetGlobalTexture("_GlobalEffectRT", rt);
        Shader.SetGlobalFloat("_OrthographicCamSize", GetComponent<Camera>().orthographicSize);
    }

    private void Update()
    {
        transform.position =
            new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        Shader.SetGlobalVector("_Position", transform.position);
    }
}