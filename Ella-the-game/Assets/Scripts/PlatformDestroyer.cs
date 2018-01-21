using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDestroyer : MonoBehaviour {
    private GameObject platformDestroyPos;
	// Use this for initialization
	void Start () {
        platformDestroyPos = GameObject.Find("PlatformDestroyPos");

    }
	
	// Update is called once per frame
	void Update () {
        if (transform.position.x < platformDestroyPos.transform.position.x)
        {
            Destroy(gameObject);
        }
	}
}
