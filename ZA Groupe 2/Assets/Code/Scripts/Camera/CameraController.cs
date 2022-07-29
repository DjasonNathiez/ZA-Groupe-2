using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    
    private GameObject m_player;
    public Camera camera;
    public bool playerFocused;
    public Transform cameraPos;
    public float cameraZoom;
    public float panSpeed = 0.5f;
    public Vector3 baseCamRot;
    public float baseCamZoom;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        
        InitializeCamera();
    }

    public void InitializeCamera()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position,cameraPos.position, Time.deltaTime*5*panSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation,cameraPos.rotation,Time.deltaTime*5*panSpeed);
        camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, new Vector3(0,0,cameraZoom), Time.deltaTime*5*panSpeed);
        camera.nearClipPlane = Mathf.Lerp(1, 15, (-camera.transform.localPosition.z - 25) / 15);

        if (playerFocused && m_player)
        {
            if (PlayerManager.instance.rope.pinnedValueTrack && PlayerManager.instance.rope.pinnedValueTrack.moveCam)
            {
                cameraPos.position = PlayerManager.instance.rope.pinnedValueTrack.posCam;
                cameraPos.rotation = Quaternion.Euler(PlayerManager.instance.rope.pinnedValueTrack.rotCam);
                cameraZoom = PlayerManager.instance.rope.pinnedValueTrack.zoomCam;
            }
            else
            {
                cameraPos.position = m_player.transform.position;   
                cameraPos.rotation = Quaternion.Euler(baseCamRot); 
                cameraZoom = baseCamZoom;
            }
        }
    }
}
