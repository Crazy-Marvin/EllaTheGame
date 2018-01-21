using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class staticObstacles : MonoBehaviour {
    [SerializeField]
    private int healthEffect;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.transform.tag == "Player")
        {
            //1st line = changing player score &&& 2nd line adding current object to a layer that is ignored by player collision
            collision.gameObject.transform.GetComponent<PlayerController>().changeHealth(healthEffect);
            gameObject.layer = LayerMask.NameToLayer("ignoredObstacles");
            Destroy(gameObject, (float)2);
        }
    }
}
