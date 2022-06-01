using UnityEngine;

public class FloorRender : MonoBehaviour
{
    public GameObject floor;
    public float yHigh;
    public float yLow;
    public bool etage;

    private void Update()
    {
        if (PlayerManager.instance.transform.position.y > yHigh && !etage)
        {
            floor.SetActive(true);
            etage = true;
        }
        else if (PlayerManager.instance.transform.position.y < yLow && etage)
        {
            floor.SetActive(false);
            etage = false;
        }
    }
}