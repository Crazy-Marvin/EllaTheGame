using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour {
    public Transform generationPoint;
    public GameObject thePlatform;
    private float platformWidth;

	// Use this for initialization
	void Start () {
        foreach (BoxCollider2D BC2D in thePlatform.GetComponents<BoxCollider2D>())
        {
           /* if (BC2D.isTrigger)
            {*/
                platformWidth = BC2D.size.x;
                break;
            //} 
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.Instance.gameState == GameManager.GameState.GameRunning)
        {
            if (transform.position.x < generationPoint.position.x)
            {
                transform.position = new Vector3(transform.position.x + platformWidth, transform.position.y, transform.position.z);
                Instantiate(thePlatform, transform.position, transform.rotation);
            }
        }
	}
}
