using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour {
    Vector3 lastCameraPosition;
    public float speed = 0.001f;
	// Use this for initialization
	void Start () {
        lastCameraPosition = Camera.main.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 shift = Camera.main.transform.position - lastCameraPosition;
        lastCameraPosition = Camera.main.transform.position;

        Vector2 offset = GetComponent<Renderer>().material.mainTextureOffset;
        offset.x += shift.x * speed;
        offset.y += shift.y * speed;
        GetComponent<Renderer>().material.mainTextureOffset = offset;
    }
}