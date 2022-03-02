using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class SpawnArea : MonoBehaviour
{
    public float rangeArea;
    private Collider[] area;
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    [Header("EnemySpawner")] 
    public int spawnNumber;
    public GameObject[] spawnableList;


    void Start()
    {
        SetArea();
        SpawnEnemy();
    }

    void SetArea()
    {
        area = Physics.OverlapSphere(transform.position, rangeArea);

        minX = transform.position.x - rangeArea;
        minZ = transform.position.z - rangeArea;
        maxX = transform.position.x + rangeArea;
        maxZ = transform.position.z + rangeArea;
        
    }

    void SpawnEnemy()
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            Vector3 spawnPoint = new Vector3(UnityEngine.Random.Range(minX, maxX), 0, UnityEngine.Random.Range(minZ, maxZ));

            GameObject newEnemy = Instantiate(spawnableList[UnityEngine.Random.Range(0, spawnableList.Length)], spawnPoint, Quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, rangeArea);
    }
}
