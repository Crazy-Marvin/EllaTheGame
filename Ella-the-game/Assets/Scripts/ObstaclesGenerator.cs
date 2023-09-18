using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesGenerator : MonoBehaviour {
    [SerializeField]
    private GameObject[] obstacles;
    private GameObject generationPoint;
    private int indexOfSpawnedObstacle;
    List<int> spawnedObstacles;
    [SerializeField]
    private float timeBetweenObstaclesSpawn = 10.0f;
	// Use this for initialization
	void Start () {
        generationPoint = GameObject.FindGameObjectWithTag("ObstaclesGenerationPos");
        InvokeRepeating("spawnObstacles", 10.0f, (float)timeBetweenObstaclesSpawn);
        spawnedObstacles = new List<int>();
        indexOfSpawnedObstacle = 0;
    }
    private void spawnObstacles()
    {
        if (GameManager.Instance.gameState == GameManager.GameState.GameRunning)
        {
            Vector3 position = new Vector3(generationPoint.transform.position.x, generationPoint.transform.position.y, 0);
            if (spawnedObstacles.Count == obstacles.Length) spawnedObstacles = new List<int>();
            int tmpIndex = Random.Range(0, obstacles.Length);
            while (spawnedObstacles.Contains(tmpIndex))
            {
                tmpIndex = Random.Range(0, obstacles.Length);
            }
            spawnedObstacles.Add(tmpIndex);
            Instantiate(obstacles[tmpIndex], position, generationPoint.transform.rotation);

        }
    }
}
