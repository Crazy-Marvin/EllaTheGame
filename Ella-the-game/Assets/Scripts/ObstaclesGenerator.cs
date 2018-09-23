using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesGenerator : MonoBehaviour {
    [SerializeField]
    private GameObject[] obstacles;
    private GameObject generationPoint;
    private int indexOfSpawnedObstacle;
    [SerializeField]
    private float timeBetweenObstaclesSpawn = 10.0f;
	// Use this for initialization
	void Start () {
        generationPoint = GameObject.FindGameObjectWithTag("ObstaclesGenerationPos");
        InvokeRepeating("spawnObstacles", 10.0f, (float)timeBetweenObstaclesSpawn);
        indexOfSpawnedObstacle = 0;
    }
	
	// Update is called once per frame
	void Update () {
        
    }
    private void spawnObstacles()
    {
        if (GameManager.Instance.gameState == GameManager.GameState.GameRunning)
        {
            Vector3 position = new Vector3(generationPoint.transform.position.x, generationPoint.transform.position.y, 0);
            Instantiate(obstacles[indexOfSpawnedObstacle], position, generationPoint.transform.rotation);
            if (indexOfSpawnedObstacle >= obstacles.Length - 1)
            {

                indexOfSpawnedObstacle = 0;
            }
            else
            {
                indexOfSpawnedObstacle++;
            }

        }
    }
}
