using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    [Header("Setup")]
    public AreaType areaType;
    public enum AreaType{CIRCLE, SQUARE}
    
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    
    private Collider[] area;
    
    //for a circle
    [Header("Circle")]
    [Range(0, 50)] public float rangeArea;
    
     

    //for a square
    [Header("Square")]
    [Range(0, 50)] public float xAxisRange;
    [Range(0, 50)] public float zAxisRange;
    
    [Header("EnemySpawner")] 
    public int spawnNumber;
    public GameObject[] spawnableList;

    private Vector3 spawnPoint;
    public Vector3 pathPoint;

    private void OnValidate()
    {
        SetArea();
    }

    private void Awake()
    {
        SetArea();
        SpawnEnemy();
    }

    void SetArea()
    {

        switch (areaType)
        {
            case AreaType.CIRCLE:
                area = Physics.OverlapSphere(transform.position, rangeArea);
                
                minX = transform.position.x - rangeArea;
                minZ = transform.position.z - rangeArea;
                maxX = transform.position.x + rangeArea;
                maxZ = transform.position.z + rangeArea;
                break;
            
            case AreaType.SQUARE:
                Vector3 square = new Vector3(xAxisRange, 0, zAxisRange);
                area = Physics.OverlapBox(transform.position, square);

                minX = transform.position.x - xAxisRange;
                minZ = transform.position.z - zAxisRange;
                maxX = transform.position.x + xAxisRange;
                maxZ = transform.position.z + zAxisRange;
                break;
            
        }
    }

    public void GeneratePositionInArea()
    {
        pathPoint = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));
    }

    void SpawnEnemy()
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            spawnPoint = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));
            GameObject selectEnemy = spawnableList[Random.Range(0, spawnableList.Length)];
            GameObject newEnemy = Instantiate(selectEnemy, new Vector3(spawnPoint.x, selectEnemy.transform.hierarchyCapacity, spawnPoint.z), Quaternion.identity);
            newEnemy.GetComponent<AIBrain>().SetSpawnPoint(this);
        }
    }

    private void OnDrawGizmos()
    {
        switch (areaType)
        {
            case AreaType.CIRCLE:
                Gizmos.DrawWireSphere(transform.position, rangeArea);
                break;
            
            case AreaType.SQUARE:
                Vector3 drawCube = new Vector3(maxX - minX, 0, maxZ - minZ);
                Gizmos.DrawWireCube(transform.position, drawCube);
                break;
        }
    }
}
