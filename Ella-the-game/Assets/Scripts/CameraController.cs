using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public PlayerController thePlayer;
    private Vector3 lastPlayerPosition;
    private float distanceToMove;
    public float transitionSpeed = 2;
	// Use this for initialization
	void Start () {
        thePlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        lastPlayerPosition = thePlayer.transform.position;

    }
	
	// Update is called once per frame
	void Update () {
       
	}
    private void LateUpdate()
    {
        distanceToMove = thePlayer.transform.position.x - lastPlayerPosition.x;
        transform.position = new Vector3(transform.position.x + distanceToMove, transform.position.y, transform.position.z);
        lastPlayerPosition = thePlayer.transform.position;
    }
}
