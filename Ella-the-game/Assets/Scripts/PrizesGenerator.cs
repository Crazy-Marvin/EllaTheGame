using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrizesGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject[] prizes;
    private GameObject generationPoint;
    private int indexOfSpawnedPrize;
    [SerializeField]
    private float timeBetweenObstaclesSpawn = 5.0f;
    // Use this for initialization
    void Start()
    {
        generationPoint = GameObject.FindGameObjectWithTag("PrizesGenerationPos");
        InvokeRepeating("spawnObstacles", 10.0f, (float)timeBetweenObstaclesSpawn);
        indexOfSpawnedPrize = 0;
    }
    private void spawnObstacles()
    {
        if (GameManager.Instance.gameState == GameManager.GameState.GameRunning)
        {
            Vector3 position = new Vector3(generationPoint.transform.position.x, generationPoint.transform.position.y, 0);
            Instantiate(prizes[indexOfSpawnedPrize], position, generationPoint.transform.rotation);

            if (indexOfSpawnedPrize >= prizes.Length - 1)
            {

                indexOfSpawnedPrize = 0;
            }
            else
            {
                indexOfSpawnedPrize++;
            }

        }
    }
}

