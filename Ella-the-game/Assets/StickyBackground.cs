using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBackground : MonoBehaviour {
    private Vector3 offset = new Vector3(0,0,0);
    private GameObject player;
    // Use this for initialization

    void Start () {
       player = GameObject.FindGameObjectWithTag("Player");
       offset.x = transform.position.x - player.transform.position.x;
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = new Vector3(player.transform.position.x + offset.x, transform.position.y, transform.position.z);
        transform.position = pos;
    }
}
