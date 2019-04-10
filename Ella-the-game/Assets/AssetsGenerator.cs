using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsGenerator : MonoBehaviour {
    public float[] itemsSpawningTimer;
    public GameObject[] items;
    public GameObject[] itemsSpawnPositions;
	// Use this for initialization
	void Awake () {
        GameManager.GameStartEvent += GameStartEventExecuted;  
    }

    private void GameStartEventExecuted()
    {
        if (items != null)
        {
            for (int i = 0; i < items.Length; i++)
            {
                StartCoroutine(SpawnItems(i));
            }
        }
    }

    // Update is called once per frame
    void Update () {
    }
    IEnumerator SpawnItems(int index)
    {
        while (GameManager.Instance.gameState == GameManager.GameState.GameRunning)
        {
            yield return new WaitForSeconds(itemsSpawningTimer[index]);
            Debug.Log("hhhhhhh");
            Instantiate(items[index], itemsSpawnPositions[index].transform.position, itemsSpawnPositions[index].transform.rotation);
        }
    }
    void OnDestroy()
    {
        GameManager.GameStartEvent -= GameStartEventExecuted;
    }
}
